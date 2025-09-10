using System;
using System.Reflection;
using BepInEx.Logging;

namespace SwiftXP.SPT.Common.Loggers;

public class SimpleStaticLogger
{
    public static SimpleStaticLogger Instance => instance.Value;

    private static readonly Lazy<SimpleStaticLogger> instance = new(() => new SimpleStaticLogger());

    private ManualLogSource logger;

    private SimpleStaticLogger()
    {
        Type type = Assembly.GetExecutingAssembly()
            .GetType("MyPluginInfo");

        FieldInfo fieldInfo = type.GetField("PLUGIN_GUID");
        string pluginGuid = (string) fieldInfo.GetValue(null);

        logger = Logger.CreateLogSource($"{pluginGuid}");
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