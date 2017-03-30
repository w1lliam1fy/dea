using Discord.WebSocket;
using System;
using System.Timers;
using DEA.SQLite.Repository;
using DEA.SQLite.Models;
using System.Linq;
using Discord;
using System.Threading.Tasks;
using DEA.SQLite.Models.Submodels;
using System.Collections.Generic;

namespace DEA.Services
{
    public class RecurringFunctions
    {

        private DiscordSocketClient _client;

        public RecurringFunctions(DiscordSocketClient client)
        {
            _client = client;
            ResetTemporaryMultiplier();
            AutoUnmute();
            ApplyInterestRate();
        }

        private void ResetTemporaryMultiplier()
        {
            Timer t = new Timer(TimeSpan.FromSeconds(30).TotalMilliseconds);
            t.AutoReset = true;
            t.Elapsed += new ElapsedEventHandler(OnTimedTempMultiplierReset);
            t.Start();
        }

        private void OnTimedTempMultiplierReset(object source, ElapsedEventArgs e)
        {
            IEnumerable<User> users = UserRepository.FetchAll();
            foreach (User user in users)
            {
                //if (user.TemporaryMultiplier != 1)
                //{
                    user.TemporaryMultiplier = 1;
                    UserRepository.UpdateUser(user);
                    PrettyConsole.NewLine("One got updated!");
                //}
            }
        }

        private void ApplyInterestRate()
        {
            Timer t = new Timer(TimeSpan.FromHours(1).TotalMilliseconds);
            t.AutoReset = true;
            t.Elapsed += new ElapsedEventHandler(OnTimedApplyInterest);
            t.Start();
        }

        private void OnTimedApplyInterest(object source, ElapsedEventArgs e)
        {
            IEnumerable<Gang> gangs = GangRepository.FetchAll();
            foreach (var gang in gangs)
            {
                var InterestRate = 0.025f + ((gang.Wealth / 100) * .000075f);
                if (InterestRate > 0.1) InterestRate = 0.1f;
                gang.Wealth *= 1 + InterestRate;
                GangRepository.UpdateGang(gang);
            }
        }

        private void AutoUnmute()
        {
            Timer t = new Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);
            t.AutoReset = true;
            t.Elapsed += new ElapsedEventHandler(OnTimedAutoUnmute);
            t.Start();
        }

        private async void OnTimedAutoUnmute(object source, ElapsedEventArgs e)
        {
            IEnumerable<Guild> guilds = GuildRepository.FetchAll();
            foreach (Guild dbGuild in guilds)
            {
                foreach (Mute mute in dbGuild.Mutes)
                {
                    if (.UtcNow.Subtract(mute.MutedAt).TotalMilliseconds > mute.MuteLength.TotalMilliseconds)
                    {
                        var guild = _client.GetGuild(dbGuild.Id);
                        if (guild != null && guild.GetUser(mute.UserId) != null)
                        {
                            var guildData = GuildRepository.FetchGuild(guild.Id);
                            var mutedRole = guild.GetRole(guildData.Roles.MutedRoleId);
                            if (mutedRole != null && guild.GetUser(mute.UserId).Roles.Any(x => x.Id == mutedRole.Id))
                            {
                                var channel = guild.GetTextChannel(guildData.Channels.ModLogId);
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
                                    var builder = new EmbedBuilder()
                                    {
                                        Color = new Color(12, 255, 129),
                                        Description = $"**Action:** Automatic Unmute\n**User:** {guild.GetUser(mute.UserId)} ({guild.GetUser(mute.UserId).Id})",
                                        Footer = footer
                                    }.WithCurrentTimestamp();
                                    GuildRepository.Modify(x => x.CaseNumber++, guild.Id);
                                    await channel.SendMessageAsync("", embed: builder);
                                }
                            }
                        }
                        MuteRepository.RemoveMute(mute.UserId, dbGuild.Id);
                    }
                }
            }
        }

    }
}
