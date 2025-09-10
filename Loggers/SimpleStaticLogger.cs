using BepInEx.Logging;

namespace SwiftXP.SPT.Common.Loggers;

public class SimpleSptLogger
{
    private ManualLogSource logger;

    public SimpleSptLogger(string pluginGuid)
    {
        logger = Logger.CreateLogSource(pluginGuid);
    }

    public void LogDebug(object data)
    {
        Log(LogLevel.Debug, data);
    }

    public void LogError(object data)
    {
        Log(LogLevel.Error, data);
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