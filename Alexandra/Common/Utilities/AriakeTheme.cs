using System.Collections.Generic;
using Serilog.Sinks.SystemConsole.Themes;

namespace Alexandra.Common.Utilities
{
    public static class AriakeTheme
    {
        public static AnsiConsoleTheme Ariake { get; } = new AnsiConsoleTheme(
            new Dictionary<ConsoleThemeStyle, string> 
            {
                [ConsoleThemeStyle.Text] = "\x1b[38;2;245;250;255m",
                [ConsoleThemeStyle.SecondaryText] = "\x1b[38;2;157;238;233m",
                [ConsoleThemeStyle.TertiaryText] = "\x1b[38;2;133;179;221m",
                [ConsoleThemeStyle.Invalid] = "\x1b[38;5;0011m",
                [ConsoleThemeStyle.Null] = "\x1b[38;5;0027m",
                [ConsoleThemeStyle.Name] = "\x1b[38;5;0007m",
                [ConsoleThemeStyle.String] = "\x1b[38;5;0045m",
                [ConsoleThemeStyle.Number] = "\x1b[38;5;0200m",
                [ConsoleThemeStyle.Boolean] = "\x1b[38;5;0027m",
                [ConsoleThemeStyle.Scalar] = "\x1b[38;5;0085m",
                [ConsoleThemeStyle.LevelVerbose] = "\x1b[38;2;124;131;216m",
                [ConsoleThemeStyle.LevelDebug] = "\x1b[38;2;133;179;221m",
                [ConsoleThemeStyle.LevelInformation] = "\x1b[38;2;161;121;239m",
                [ConsoleThemeStyle.LevelWarning] = "\x1b[38;2;157;238;233m",
                [ConsoleThemeStyle.LevelError] = "\x1b[48;2;42;45;54m\x1b[38;2;218;165;241m",
                [ConsoleThemeStyle.LevelFatal] = "\x1b[48;2;42;45;54m\x1b[38;2;133;179;221m",
            }
        );
    }
}