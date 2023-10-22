using GameSpace;
using LoggerUtil;

namespace OpenTKTutorial
{
    class Program
    {
        private static LogLevel logLevel = LogLevel.WARNING;
        static void Main(string[] args)
        {
          Logger logger = new Logger(logLevel);
          logger.Info("Welcome to the future of video game development");
          logger.Info("This is: Amateur Hour");

          GameOptions options = new GameOptions(logLevel);

          using (Game game = new Game(800, 600, "Amateur Hour", logger, options))
          {
              game.Run();
          }
        }
    }
}