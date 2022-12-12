using System;
using System.Diagnostics;

namespace MathInvaders
{
    /// <summary>
    /// Styrer spillets primære spilcyklus
    /// </summary>
    public static class GameManager
    {
        public static float deltaTime = 0; //bruges til timere og til at basere bevægelse/hastighed på tid og ikke fps - måler i sekunder
        public static bool gameOver = false; //er spillet slut?

        private static void Main(string[] args)
        {
            //startop kode
            Console.CursorVisible = false; //skaber en mere spilagtig følelse
            Player.Init();
            Drawer.Init();
            Drawer.Draw();

            Stopwatch watch = new Stopwatch();

            while (!gameOver) //primære spilcyklus
            {
                watch.Restart();
                Spawner.Update();
                Player.Update();
                Drawer.Draw(); //kaldt sidst i hver opdatering
                watch.Stop();
                deltaTime = watch.ElapsedMilliseconds / 1000f;
            }

            Console.ReadKey();
        }
    }
}
