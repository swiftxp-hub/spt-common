using System;
using System.Linq;
using BepInEx.Logging;

namespace SwiftXP.SPT.Common.Loggers;

public class SimpleSptLogger
{
    private ManualLogSource logger;

    public SimpleSptLogger(string pluginGuid, string pluginVersion)
    {
        logger = Logger.CreateLogSource($"{pluginGuid} (v{pluginVersion})");
    }

    public void LogDebug(object data)
    {
        Log(LogLevel.Debug, data);
    }

    public void LogError(object data)
    {
        Log(LogLevel.Error, data);
    }

    public void LogException(Exception exception)
    {
        string? firstLineOfStackTrace = exception
            .StackTrace
            ?.Split(Environment.NewLine)
            ?.FirstOrDefault();

        LogError($"An unexpected error occured. Message: {exception.Message}. f.l.o.s.t.: {firstLineOfStackTrace}");
    }

    public void LogInfo(object data)
    {
        Log(LogLevel.Info, data);
    }

    public void Log(LogLevel logLevel, object data)
    {
        logger.Log(logLevel, data);
    }
}