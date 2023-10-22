using System.Security;

namespace LoggerUtil
{
  /**
  * LogLevel behavior:
  * ERROR: Log only errors.
  * WARN: Log warnings and errors.
  * INFO: Log all logs.
  * DEBUG: Log with extra information (like timestamp deltas)
  */ 
  public enum LogLevel {
    ERROR,
    WARNING,
    INFO,
    DEBUG,
  }
  public class Logger {
    public Logger(LogLevel mode = LogLevel.INFO) {
      Mode = LogLevel.DEBUG;
      Debug("Testing log outputs...");
      Debug("DEBUG message");
      Info("INFO message");
      Warn("WARN message");
      Error("ERROR message");
      Mode = mode;
      Info($"Logger initialized in {mode} mode");
    }

    private DateTime lastLog;

    private LogLevel Mode;

    private const string FORMAT_STRING = "yyyy/MM/dd HH:mm:ss.ff";

    private void pinTime(DateTime newTime) {
      lastLog = newTime;
    }

    public void Log(string message, LogLevel level) {
      if (!ShouldLog(level)) {
        return;
      }
      // Save timestamp to pin later
      DateTime thisLogTime = DateTime.Now;

      string logTimeString = thisLogTime.ToString(FORMAT_STRING);
      string logDelta = "";
      string levelString = string.Format("{0,-9}", $"[{level}]");
      if (Mode >= LogLevel.DEBUG) {
        logDelta = LogDeltaString();
      }
      
      string logString = $"{logTimeString} {logDelta} {levelString} {message}";
      
      Console.WriteLine(logString);
      pinTime(thisLogTime);
    }

    public void Error(string message) {
      Console.ForegroundColor = ConsoleColor.Red;
      Log(message, LogLevel.ERROR);
      Console.ForegroundColor = ConsoleColor.Gray;
    }

    public void Warn(string message) {
      Console.ForegroundColor = ConsoleColor.Yellow;
      Log(message, LogLevel.WARNING);
      Console.ForegroundColor = ConsoleColor.Gray;
    }
    public void Info(string message) {
      Log(message, LogLevel.INFO);
    }

    public void Debug(string message) {
      Console.ForegroundColor = ConsoleColor.Green;
      Log(message, LogLevel.DEBUG);
      Console.ForegroundColor = ConsoleColor.Gray;
    }

    private bool ShouldLog(LogLevel level) {
      return Mode >= level;
    }

    /**
    * Returns a string representation of the timespan since the last log.
    */
    private string LogDeltaString() {
      DateTime thisLogTime = DateTime.Now;
      TimeSpan delta = thisLogTime - lastLog;
      string seconds = delta.Seconds.ToString();
      string milliseconds = delta.Milliseconds.ToString();
      string deltaString = string.Format("[delta: {0, 3}s {1, 3}ms]", seconds, milliseconds);
      return deltaString;
    }
  }



}