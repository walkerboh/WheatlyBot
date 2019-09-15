using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace WheatlyBot.Common.Setup
{
    public static class LoggerSetup
    {
        public static void SetupLog()
        {
            var configuration = new LoggingConfiguration();

            var console = new ConsoleTarget
            {
                Layout = @"${date}|${level:uppercase=true}|${message} ${exception}|${logger}|${all-event-properties}"
            };

            var file = new FileTarget
            {
                FileName = Path.Combine(Directory.GetCurrentDirectory(), "logs", "log.log"),
                Layout = @"${date}|${level:uppercase=true}|${message} ${exception}|${logger}|${all-event-properties}",
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveFileName = Path.Combine(Directory.GetCurrentDirectory(), "logs", DateTime.Now.ToString("yyyyMMdd") + ".log")
            };

            configuration.AddTarget("Console", console);
            configuration.AddTarget("File", file);

            configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, console));
            configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, file));

            LogManager.Configuration = configuration;
        }
    }
}
