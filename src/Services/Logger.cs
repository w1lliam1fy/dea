using DEA.Database.Repository;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace DEA.Services
{
    public static class Logger
    {
        public static async Task ModLog(SocketCommandContext context, string action, Color color, string reason, IUser subject = null, string extra = "")
        {
            var guild = GuildRepository.FetchGuild(context.Guild.Id);
            EmbedFooterBuilder footer = new EmbedFooterBuilder()
            {
                IconUrl = "http://i.imgur.com/BQZJAqT.png",
                Text = $"Case #{guild.CaseNumber}"
            };
            EmbedAuthorBuilder author = new EmbedAuthorBuilder()
            {
                IconUrl = context.User.GetAvatarUrl(),
                Name = $"{context.User.Username}#{context.User.Discriminator}"
            };

            string userText = null;
            if (subject != null) userText = $"\n** User:** { subject} ({ subject.Id})";
            var builder = new EmbedBuilder()
            {
                Author = author,
                Color = color,
                Description = $"**Action:** {action}{extra}{userText}\n**Reason:** {reason}",
                Footer = footer
            }.WithCurrentTimestamp();

            if (context.Guild.GetTextChannel(guild.ModLogId) != null)
            {
                await context.Guild.GetTextChannel(guild.ModLogId).SendMessageAsync(string.Empty, embed: builder);
                GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.CaseNumber, ++guild.CaseNumber), context.Guild.Id);
            }
        }
        

        public static async Task DetailedLog(SocketGuild guild, string actionType, string action, string objectType, string objectName, ulong id, Color color, bool incrementCaseNumber = true)
        {
            var guildData = GuildRepository.FetchGuild(guild.Id);
            if (guild.GetTextChannel(guildData.DetailedLogsId) != null)
            {
                var channel = guild.GetTextChannel(guildData.DetailedLogsId);
                if (guild.CurrentUser.GuildPermissions.EmbedLinks && (guild.CurrentUser as IGuildUser).GetPermissions(channel as SocketTextChannel).SendMessages
                    && (guild.CurrentUser as IGuildUser).GetPermissions(channel as SocketTextChannel).EmbedLinks)
                {
                    string caseText = $"Case #{guildData.CaseNumber}";
                    if (!incrementCaseNumber) caseText = id.ToString();
                    EmbedFooterBuilder footer = new EmbedFooterBuilder()
                    {
                        IconUrl = "http://i.imgur.com/BQZJAqT.png",
                        Text = caseText
                    };

                    string idText = null;
                    if (incrementCaseNumber) idText = $"\n**Id:** {id}";
                    var builder = new EmbedBuilder()
                    {
                        Color = color,
                        Description = $"**{actionType}:** {action}\n**{objectType}:** {objectName}{idText}",
                        Footer = footer
                    }.WithCurrentTimestamp();

                    await guild.GetTextChannel(guildData.DetailedLogsId).SendMessageAsync(string.Empty, embed: builder);
                    if (incrementCaseNumber) GuildRepository.Modify(DEABot.GuildUpdateBuilder.Set(x => x.CaseNumber, ++guildData.CaseNumber), guild.Id);
                }
            }
        }

        public static async Task Cooldown(SocketCommandContext context, string command, TimeSpan timeSpan)
        {
            await ResponseMethods.Send(context, 
                $"Hours: {timeSpan.Hours}\nMinutes: {timeSpan.Minutes}\nSeconds: {timeSpan.Seconds}",
                $"{command} cooldown for {context.User}");
        }
    }
}
