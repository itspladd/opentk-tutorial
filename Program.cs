using GameSpace;

namespace OpenTKTutorial
{
    class Program
    {
        static void Main(string[] args)
        {

          Console.WriteLine("Welcome to the future of video game development");
          Console.WriteLine("This is: Amateur Hour");

          using (Game game = new Game(800, 600, "Amateur Hour"))
          {
              game.Run();
          }
        }
    }
}