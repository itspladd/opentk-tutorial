using GameSpace;
using LoggerUtil;

namespace OpenTKTutorial
{
    class Program
    {
        static void Main(string[] args)
        {
          Logger logger = new Logger();
          logger.Log("Welcome to the future of video game development");
          logger.Log("This is: Amateur Hour");

          using (Game game = new Game(800, 600, "Amateur Hour", logger))
          {
              game.Run();
          }
        }
    }
}