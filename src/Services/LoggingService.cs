using DEA.Common;
using DEA.Common.Extensions.DiscordExtensions;
using DEA.Database.Repository;
using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace DEA.Services
{
    public class LoggingService
    {
        private readonly GuildRepository _guildRepo;

        public LoggingService(GuildRepository guildRepo)
        {
            _guildRepo = guildRepo;
        }

        public async Task ModLogAsync(DEAContext context, string action, Color color, string reason, IUser subject = null, string extra = "")
        {
            var channel = context.Guild.GetTextChannel(context.DbGuild.ModLogId);

            if (channel == null) return;

            EmbedFooterBuilder footer = new EmbedFooterBuilder()
            {
                IconUrl = "http://i.imgur.com/BQZJAqT.png",
                Text = $"Case #{context.DbGuild.CaseNumber}"
            };
            EmbedAuthorBuilder author = new EmbedAuthorBuilder()
            {
                IconUrl = context.User.GetAvatarUrl(),
                Name = $"{context.User.Username}#{context.User.Discriminator}"
            };

            string userText = string.Empty;
            if (subject != null) userText = $"\n** User:** { subject} ({ subject.Id})";
            var builder = new EmbedBuilder()
            {
                Author = author,
                Color = color,
                Description = $"**Action:** {action}{extra}{userText}\n**Reason:** {reason}",
                Footer = footer
            }.WithCurrentTimestamp();

            await channel.SendMessageAsync(string.Empty, embed: builder);
            await _guildRepo.ModifyAsync(context.Guild.Id, x => x.CaseNumber, ++context.DbGuild.CaseNumber);
        }
        

        public Task DetailedLogAsync(SocketGuild guild, string actionType, string action, string objectType, string objectName, ulong id, Color color, bool incrementCaseNumber = true)
            => Task.Run(async () =>
            {
                var dbGuild = await _guildRepo.FetchGuildAsync(guild.Id);
                var channel = guild.GetTextChannel(dbGuild.DetailedLogsId);
                if (channel != null)
                {
                    if (guild.CurrentUser.GuildPermissions.EmbedLinks && (guild.CurrentUser as IGuildUser).GetPermissions(channel as SocketTextChannel).SendMessages
                        && (guild.CurrentUser as IGuildUser).GetPermissions(channel as SocketTextChannel).EmbedLinks)
                    {
                        string caseText = $"Case #{dbGuild.CaseNumber}";
                        if (!incrementCaseNumber) caseText = id.ToString();
                        EmbedFooterBuilder footer = new EmbedFooterBuilder()
                        {
                            IconUrl = "http://i.imgur.com/BQZJAqT.png",
                            Text = caseText
                        };

                        string idText = string.Empty;
                        if (incrementCaseNumber) idText = $"\n**Id:** {id}";
                        var builder = new EmbedBuilder()
                        {
                            Color = color,
                            Description = $"**{actionType}:** {action}\n**{objectType}:** {objectName}{idText}",
                            Footer = footer
                        }.WithCurrentTimestamp();

                        await channel.SendMessageAsync(string.Empty, embed: builder);
                        if (incrementCaseNumber) await _guildRepo.ModifyAsync(guild.Id, x => x.CaseNumber, ++dbGuild.CaseNumber);
                    }
                }
            });

        public Task DetailedLogsForChannel(SocketChannel socketChannel, string action, Color color)
            => Task.Run(async () =>
            {
                var textChannel = socketChannel as SocketTextChannel;
                var voiceChannel = socketChannel as SocketVoiceChannel;
                if (textChannel == null && voiceChannel == null) return;
                string channelType;
                string channelName;
                SocketGuild guild;
                if (textChannel != null)
                {
                    channelType = "Text Channel";
                    channelName = (socketChannel as SocketTextChannel).Name;
                    guild = (socketChannel as SocketTextChannel).Guild;
                }
                else
                {
                    channelType = "Voice Channel";
                    channelName = (socketChannel as SocketVoiceChannel).Name;
                    guild = (socketChannel as SocketVoiceChannel).Guild;
                }
                await DetailedLogAsync(guild, "Action", action, channelType, channelName, socketChannel.Id, color);
            });

        public Task CooldownAsync(DEAContext context, string command, TimeSpan timeSpan)
        {
            var user = command.ToLower() == "raid" ? context.Gang.Name : context.User.ToString();
            return context.Channel.SendAsync($"Hours: {timeSpan.Hours}\nMinutes: {timeSpan.Minutes}\nSeconds: {timeSpan.Seconds}",$"{command} cooldown for {user}");
        }
    }
}
