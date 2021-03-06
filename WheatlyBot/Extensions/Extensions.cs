﻿using Discord;
using NLog;

namespace WheatlyBot.Extensions
{
    public static class Extensions
    {
        public static LogLevel ToNLogLevel(this LogSeverity logSeverity)
        {
            switch (logSeverity)
            {
                case LogSeverity.Critical:
                    return LogLevel.Fatal;
                case LogSeverity.Debug:
                    return LogLevel.Debug;
                case LogSeverity.Error:
                    return LogLevel.Error;
                case LogSeverity.Info:
                    return LogLevel.Info;
                case LogSeverity.Verbose:
                    return LogLevel.Trace;
                case LogSeverity.Warning:
                    return LogLevel.Warn;
                default:
                    return LogLevel.Warn;
            }
        }
    }
}
