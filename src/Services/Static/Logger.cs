using Discord;
using System;

namespace DEA.Services.Static
{
    internal static class Logger
    {
        public static void Log(LogSeverity severity, string source, string message)
        {
            Append($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.DarkGray);
            Append($"[{severity}] ", ConsoleColor.Red);
            Append($"{source}: ", ConsoleColor.DarkGreen);
            NewLine(message);
        }

        public static void NewLine(string text = "", ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.WriteLine(text);
        }

        public static void Append(string text, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.Write(text);
        }
    }
}
