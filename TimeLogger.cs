namespace LoggerUtil
{
  public class Logger {
    public Logger() {
      Log("Logger initialized");
    }

    private DateTime lastLog;

    private const string FORMAT_STRING = "yyyy/MM/dd HH:mm:ss.ff";

    private void pinTime(DateTime newTime) {
      lastLog = newTime;
    }

    public void Log(string message) {
      DateTime thisLogTime = DateTime.Now;
      string logString = thisLogTime.ToString(FORMAT_STRING) + " " + message;
      Console.WriteLine(logString);
      pinTime(thisLogTime);
    }

    public void DeltaLog(string message) {
      DateTime thisLogTime = DateTime.Now;
      TimeSpan delta = thisLogTime - lastLog;
      string deltaString = delta.Seconds.ToString() + "s " + delta.Milliseconds.ToString() + "ms";
      Log("[Delta " + deltaString + "] " + message);
    }
  }



}