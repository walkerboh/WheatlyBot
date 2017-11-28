using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace ChronoBot.Common.Setup
{
    public static class LoggerSetup
    {
        public static void SetupLog()
        {
            LoggingConfiguration configuration = new LoggingConfiguration();

            ConsoleTarget console = new ConsoleTarget()
            {
                Layout = @"${date}|${level:uppercase=true}|${message} ${exception}|${logger}|${all-event-properties}"
            };

            FileTarget file = new FileTarget()
            {
                FileName = Path.Combine(Directory.GetCurrentDirectory(), "logs", "log.log"),
                Layout = @"${date}|${level:uppercase=true}|${message} ${exception}|${logger}|${all-event-properties}",
                ArchiveEvery = FileArchivePeriod.Day
            };

            configuration.AddTarget("Console", console);
            configuration.AddTarget("File", file);

            configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, console));
            configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, file));

            LogManager.Configuration = configuration;
        }
    }
}
