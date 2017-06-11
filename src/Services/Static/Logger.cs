using Discord;
using System;

namespace DEA.Services.Static
{
    internal static class Logger
    {
        public static void Log(LogSeverity severity, string source, string message)
        {
            ConsoleColor centerColor;

            switch (severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    centerColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    centerColor = ConsoleColor.Yellow;
                    break;
                default:
                    centerColor = ConsoleColor.Magenta;
                    break;
            }

            Append($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.DarkBlue, ConsoleColor.White);
            Append($"[{severity}] ", centerColor);
            Append($"{source}: ", ConsoleColor.White);
            NewLine(message, ConsoleColor.Cyan);
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