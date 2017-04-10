using DEA.Database.Models;
using DEA.Database.Repository;
using Discord;
using Discord.WebSocket;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading;

namespace DEA.Services.Timers
{
    class AutoUnmute
    {
        private Timer _timer;

        public AutoUnmute()
        {
            ObjectState StateObj = new ObjectState();

            TimerCallback TimerDelegate = new TimerCallback(TimerTask);

            _timer = new Timer(TimerDelegate, StateObj, 0, Config.AUTO_UNMUTE_COOLDOWN);

            StateObj.TimerReference = _timer;
        }

        private async void TimerTask(object stateObj)
        {
            var builder = Builders<Mute>.Filter;
            foreach (var mute in await (await DEABot.Mutes.FindAsync(builder.Empty)).ToListAsync())
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
                                await channel.SendMessageAsync(string.Empty, embed: embedBuilder);
                            }
                        }
                    }
                    MuteRepository.RemoveMute(mute.UserId, mute.GuildId);
                }
            }
        }
    }
}
