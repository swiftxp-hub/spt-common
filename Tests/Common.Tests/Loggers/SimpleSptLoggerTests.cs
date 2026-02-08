using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using SwiftXP.SPT.Common.Loggers;
using Xunit;

namespace SwiftXP.SPT.Common.Tests.IO.Hashing;

[Collection("BepInEx Logging Tests")]
public class SimpleSptLoggerTests : IDisposable
{
    private readonly MockLogListener _listener;

    public SimpleSptLoggerTests()
    {
        _listener = new MockLogListener();

        Logger.Listeners.Add(_listener);
    }

    public void Dispose()
    {
        Logger.Listeners.Remove(_listener);

        GC.SuppressFinalize(this);
    }

    [Fact]
    public void ConstructorCreatesLogSourceWithCorrectName()
    {
        string guid = "com.test.mod";
        string version = "1.0.0";
        string expectedName = "com.test.mod (v1.0.0)";

        SimpleSptLogger logger = new(guid, version);

        logger.LogInfo("Test");

        LogEventArgs? logEvent = _listener.Logs.LastOrDefault();

        Assert.NotNull(logEvent);
        Assert.Equal(expectedName, logEvent.Source.SourceName);
    }

    [Fact]
    public void LogInfoSendsInfoLevelLog()
    {
        SimpleSptLogger logger = new("guid", "1.0");
        string message = "Information";

        logger.LogInfo(message);

        LogEventArgs logEvent = _listener.Logs.Single();

        Assert.Equal(LogLevel.Info, logEvent.Level);
        Assert.Equal(message, logEvent.Data);
    }

    [Fact]
    public void LogErrorSendsErrorLevelLog()
    {
        SimpleSptLogger logger = new("guid", "1.0");
        string message = "Critical Failure";

        logger.LogError(message);

        LogEventArgs logEvent = _listener.Logs.Single();

        Assert.Equal(LogLevel.Error, logEvent.Level);
        Assert.Equal(message, logEvent.Data);
    }

    [Fact]
    public void LogDebugSendsDebugLevelLog()
    {
        SimpleSptLogger logger = new("guid", "1.0");
        string message = "Debugging variable";

        logger.LogDebug(message);

        LogEventArgs logEvent = _listener.Logs.Single();

        Assert.Equal(LogLevel.Debug, logEvent.Level);
        Assert.Equal(message, logEvent.Data);
    }

    [Fact]
    public void LogExceptionFormatsMessageAndIncludeStackTrace()
    {
        SimpleSptLogger logger = new("guid", "1.0");
        Exception testException;

        try
        {
            ThrowTestException();

#pragma warning disable CA2201 // Do not raise reserved exception types

            throw new Exception("Should have thrown");
#pragma warning restore CA2201 // Do not raise reserved exception types

        }
        catch (InvalidOperationException ex)
        {
            testException = ex;
        }

        logger.LogException(testException);

        LogEventArgs logEvent = _listener.Logs.Single();

        Assert.Equal(LogLevel.Error, logEvent.Level);

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

        string logMessage = logEvent.Data.ToString();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        Assert.Contains("An unexpected error occured", logMessage);
        Assert.Contains("Message: This is a test error", logMessage);
        Assert.Contains("f.l.o.s.t.:", logMessage);
        Assert.Contains(nameof(ThrowTestException), logMessage);
    }

    [Fact]
    public void LogExceptionHandlesNullStackTrace()
    {
        SimpleSptLogger logger = new("guid", "1.0");
#pragma warning disable CA2201 // Do not raise reserved exception types

        Exception exceptionWithoutStack = new("No stack");
#pragma warning restore CA2201 // Do not raise reserved exception types

        logger.LogException(exceptionWithoutStack);

        LogEventArgs logEvent = _listener.Logs.Single();
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

        string logMessage = logEvent.Data.ToString();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        Assert.Contains("Message: No stack", logMessage);
    }

    private static void ThrowTestException()
    {
        throw new InvalidOperationException("This is a test error");
    }

    private sealed class MockLogListener : ILogListener
    {
        public List<LogEventArgs> Logs { get; } = new();

        public void LogEvent(object sender, LogEventArgs eventArgs)
        {
            Logs.Add(eventArgs);
        }

        public void Dispose()
        {
            Logs.Clear();
        }
    }
}