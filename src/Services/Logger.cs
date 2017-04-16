using DEA.Common;
using DEA.Database.Models;
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
        public static async Task LogAsync(LogSeverity severity, string source, string message)
        {
            await NewLine($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.DarkGray);
            await Append($"[{severity}] ", ConsoleColor.Red);
            await Append($"{source}: ", ConsoleColor.DarkGreen);
            await Append(message, ConsoleColor.White);
        }

        public static Task NewLine(string text = "", ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            if (foreground == null)
                foreground = ConsoleColor.White;
            if (background == null)
                background = ConsoleColor.Black;

            Console.ForegroundColor = (ConsoleColor)foreground;
            Console.BackgroundColor = (ConsoleColor)background;
            Console.Write(Environment.NewLine + text);
            return Task.CompletedTask;
        }

        private static Task Append(string text, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            if (foreground == null)
                foreground = ConsoleColor.White;
            if (background == null)
                background = ConsoleColor.Black;

            Console.ForegroundColor = (ConsoleColor)foreground;
            Console.BackgroundColor = (ConsoleColor)background;
            Console.Write(text);
            return Task.CompletedTask;
        }

        public static async Task ModLogAsync(DEAContext context, string action, Color color, string reason, IUser subject = null, string extra = "")
        {
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

            string userText = null;
            if (subject != null) userText = $"\n** User:** { subject} ({ subject.Id})";
            var builder = new EmbedBuilder()
            {
                Author = author,
                Color = color,
                Description = $"**Action:** {action}{extra}{userText}\n**Reason:** {reason}",
                Footer = footer
            }.WithCurrentTimestamp();

            if (context.Guild.GetTextChannel(context.DbGuild.ModLogId) != null)
            {
                await context.Guild.GetTextChannel(context.DbGuild.ModLogId).SendMessageAsync(string.Empty, embed: builder);
                await GuildRepository.ModifyAsync(context.Guild.Id, x => x.CaseNumber, ++context.DbGuild.CaseNumber);
            }
        }
        

        public static async Task DetailedLogAsync(SocketGuild guild, string actionType, string action, string objectType, string objectName, ulong id, Color color, bool incrementCaseNumber = true)
        {
            var dbGuild = await GuildRepository.FetchGuildAsync(guild.Id);
            if (guild.GetTextChannel(dbGuild.DetailedLogsId) != null)
            {
                var channel = guild.GetTextChannel(dbGuild.DetailedLogsId);
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

                    string idText = null;
                    if (incrementCaseNumber) idText = $"\n**Id:** {id}";
                    var builder = new EmbedBuilder()
                    {
                        Color = color,
                        Description = $"**{actionType}:** {action}\n**{objectType}:** {objectName}{idText}",
                        Footer = footer
                    }.WithCurrentTimestamp();

                    await guild.GetTextChannel(dbGuild.DetailedLogsId).SendMessageAsync(string.Empty, embed: builder);
                    if (incrementCaseNumber) await GuildRepository.ModifyAsync(guild.Id, x => x.CaseNumber, ++dbGuild.CaseNumber);
                }
            }
        }

        public static async Task CooldownAsync(DEAContext context, string command, TimeSpan timeSpan)
        {
            var user = command.ToLower() == "raid" ? context.Gang.Name : await ResponseMethods.TitleNameAsync(context.User as IGuildUser);
            await ResponseMethods.Send(context, 
                $"Hours: {timeSpan.Hours}\nMinutes: {timeSpan.Minutes}\nSeconds: {timeSpan.Seconds}",
                $"{command} cooldown for {user}");
        }
    }
}
