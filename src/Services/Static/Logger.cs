using Discord;
using System;
using System.Threading.Tasks;

namespace DEA.Services.Static
{
    public static class Logger
    {
        public static async Task LogAsync(LogSeverity severity, string source, string message)
        {
            await NewLineAsync($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.DarkGray);
            await AppendAsync($"[{severity}] ", ConsoleColor.Red);
            await AppendAsync($"{source}: ", ConsoleColor.DarkGreen);
            await AppendAsync(message, ConsoleColor.White);
        }

        public static Task NewLineAsync(string text = "", ConsoleColor? foreground = null, ConsoleColor? background = null)
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

        private static Task AppendAsync(string text, ConsoleColor? foreground = null, ConsoleColor? background = null)
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
    }
}
