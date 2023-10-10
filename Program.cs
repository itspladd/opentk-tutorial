using GameSpace;

namespace OpenTKTutorial
{
    class Program
    {
        static void Main(string[] args)
        {
          Console.WriteLine("Beans");
          using (Game game = new Game(800, 600, "LearnOpenTK"))
          {
              game.Run();
          }
        }
    }
}