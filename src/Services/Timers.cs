using Discord.WebSocket;
using System;
using DEA.Database.Repository;
using DEA.Database.Models;
using System.Linq;
using Discord;
using System.Threading;
using MongoDB.Driver;
using System.Collections.Generic;

namespace DEA.Services
{
    public class Timers
    {
        private List<Timer> _timers = new List<Timer>();

        public Timers()
        {
            ResetTemporaryMultiplier();
            AutoUnmute();
            ApplyInterestRate();
        }

        private void ResetTemporaryMultiplier()
        {
            _timers.Add(new Timer(async method =>
            {
                var builder = Builders<User>.Filter;
                await DEABot.Users.UpdateManyAsync(builder.Empty, DEABot.UserUpdateBuilder.Set(x => x.TemporaryMultiplier, 1));
            }, 
            null,
            Config.TEMP_MULTIPLIER_RESET_COOLDOWN, 
            Config.TEMP_MULTIPLIER_RESET_COOLDOWN));
        }

        private void ApplyInterestRate()
        {
            _timers.Add(new Timer(async method => 
            {
                var builder = Builders<Gang>.Filter;
                foreach (var gang in await (await DEABot.Gangs.FindAsync(builder.Empty)).ToListAsync())
                    await DEABot.Gangs.UpdateOneAsync(y => y.Id == gang.Id, 
                        DEABot.GangUpdateBuilder.Set(x => x.Wealth, Math.CalculateIntrestRate(gang.Wealth) * gang.Wealth + gang.Wealth));
            },
            null,
            Config.INTEREST_RATE_COOLDOWN,
            Config.INTEREST_RATE_COOLDOWN));
        }

        private void AutoUnmute()
        {
            _timers.Add(new Timer(async method => 
            {
                var builder = Builders<Mute>.Filter;
                foreach (Mute mute in await (await DEABot.Mutes.FindAsync(builder.Empty)).ToListAsync())
                {
                    if (DateTime.UtcNow.Subtract(mute.MutedAt).TotalMilliseconds > mute.MuteLength)
                    {
                        var guild = DEABot.Client.GetGuild(mute.GuildId);
                        if (guild != null && guild.GetUser(mute.UserId) != null)
                        {
                            var guildData = GuildRepository.FetchGuild(guild.Id);
                            var mutedRole = guild.GetRole(guildData.MutedRoleId);
                            if (mutedRole != null && guild.GetUser(mute.UserId).Roles.Any(x => x.Id == mutedRole.Id))
                            {
                                var channel = guild.GetTextChannel(guildData.ModLogId);
                                if (channel != null && guild.CurrentUser.GuildPermissions.EmbedLinks &&
                                    (guild.CurrentUser as IGuildUser).GetPermissions(channel as SocketTextChannel).SendMessages
                                    && (guild.CurrentUser as IGuildUser).GetPermissions(channel as SocketTextChannel).EmbedLinks)
                                {
                                    await guild.GetUser(mute.UserId).RemoveRoleAsync(mutedRole);
                                    var footer = new EmbedFooterBuilder()
                                    {
                                        IconUrl = "http://i.imgur.com/BQZJAqT.png",
                                        Text = $"Case #{guildData.CaseNumber}"
                                    };
                                    var embedBuilder = new EmbedBuilder()
                                    {
                                        Color = new Color(12, 255, 129),
                                        Description = $"**Action:** Automatic Unmute\n**User:** {guild.GetUser(mute.UserId)} ({guild.GetUser(mute.UserId).Id})",
                                        Footer = footer
                                    }.WithCurrentTimestamp();
                                    GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.CaseNumber, ++guildData.CaseNumber), guild.Id);
                                    await channel.SendMessageAsync("", embed: embedBuilder);
                                }
                            }
                        }
                        MuteRepository.RemoveMute(mute.UserId, mute.GuildId);
                    }
                }
            },
            null,
            Config.AUTO_UNMUTE_COOLDOWN,
            Config.AUTO_UNMUTE_COOLDOWN));
        }

    }
}
