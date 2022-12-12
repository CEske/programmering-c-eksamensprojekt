using System;
using System.Collections.Generic;

namespace MathInvaders
{
    /// <summary>
    /// Spilleren som brugeren styrer
    /// </summary>
    public static class Player
    {
        private static float[] pos;
        private const int width = 8;
        private const int height = 3;
        private const float moveSpeed = 1f; //hastigheden spilleren bevæger sig med
        private const char drawChar = 'I'; //tegnet spilleren tegnes med
        private const ConsoleColor drawCol = ConsoleColor.White;
        private static Drawer.Rect rect; //firkanten som visuelt repræsenterer spilleren

        private static Drawer.TextBox shotText; //viser spillerens nuværende tal
        private static float[] shotTextPos { get => new float[] { pos[0], pos[1] - 2f / Drawer.windowSize[1] }; }
        private const int shotTextWidth = 8;
        private const int shotTextHeight = 3;

        private static int shot = 0; //holder tallet der skydes afsted
        private const int shotMaxLength = 4; //maksimale antal cifre for et skud
        private const float shotLifeTime = 0.3f; //antal sekunder et skud eksisterer
        private static Dictionary<Drawer.Rect, float> shots = new Dictionary<Drawer.Rect, float>(); //spillerens aktive skud

        /// <summary>
        /// Kaldes ved opstart
        /// </summary>
        public static void Init()
        {
            pos = new float[] { 0.5f - (float)width / (2 * Drawer.windowSize[0]), 1f - (float)height / Drawer.windowSize[1] }; //starter i midten af bunden af skærmen 
            rect = new Drawer.Rect(width, height, pos, true, drawChar, drawCol); //spillerens firkant instantieres
            shotText = new Drawer.TextBox(new string[] { shot.ToString("0") }, shotTextWidth, shotTextHeight, shotTextPos, drawChar, drawCol);
        }

        /// <summary>
        /// Kaldes en gang hver opdatering
        /// </summary>
        public static void Update()
        {
            if (Console.KeyAvailable) //sørger for at vi ikke blokerer program eksekveringen
            {
                //tager input fra brugeren
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        ChangePos(-moveSpeed * GameManager.deltaTime);
                        break;
                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        ChangePos(moveSpeed * GameManager.deltaTime);
                        break;
                    case ConsoleKey n when (n >= ConsoleKey.D0 && n <= ConsoleKey.D9):
                        NumberPressed(key, (int)ConsoleKey.D0);
                        break;
                    case ConsoleKey n when (n >= ConsoleKey.NumPad0 && n <= ConsoleKey.NumPad9):
                        NumberPressed(key, (int)ConsoleKey.NumPad0);
                        break;
                    case ConsoleKey.Backspace:
                        DeleteNumber();
                        break;
                    case ConsoleKey.Spacebar:
                    case ConsoleKey.Enter:
                        Shoot();
                        break;
                    default:
                        break;
                }
            }

            //behandler aktive skuds livstid
            List<Drawer.Rect> finishedShots = new List<Drawer.Rect>(); //skud der skal slettes
            List<Drawer.Rect> onGoingShots = new List<Drawer.Rect>(); //skud der forbliver endnu
            foreach (Drawer.Rect r in shots.Keys) //vi kan ikke ændre samlingen vi iterer over, derfor gemmer vi valgte elementer til ovenstående lister
            {
                if (shots[r] - GameManager.deltaTime > 0) onGoingShots.Add(r);
                else finishedShots.Add(r);
            }
            foreach(Drawer.Rect r in finishedShots) { r.Remove(); shots.Remove(r); }
            foreach (Drawer.Rect r in onGoingShots) shots[r] -= GameManager.deltaTime;
        }

        /// <summary>
        /// Ændrer spillerens position horisontalt
        /// </summary>
        /// <param name="change">Ændringen</param>
        private static void ChangePos(float change)
        {
            float newPos = pos[0] + change;
            if(newPos >= 0 && newPos <= 1 - width / (float)Drawer.windowSize[0])
            {
                Drawer.Erase(Drawer.GetScreenPos(pos), new int[] { width, height }); //sletter den gamle position
                Drawer.Erase(Drawer.GetScreenPos(shotTextPos), new int[] { shotTextWidth, shotTextHeight });
                pos[0] = newPos;
                rect.SetPos(pos);
                shotText.SetPos(shotTextPos);
            }
        }

        /// <summary>
        /// Brugeren har trykket på en tast
        /// </summary>
        /// <param name="key">Tasten</param>
        /// <param name="baseNum">Tallet måling starter fra</param>
        private static void NumberPressed(ConsoleKey key, int baseNum)
        {
            if(shot < MathF.Pow(10, shotMaxLength))
            {
                shot = shot * 10 + ((int)key - baseNum); //tilføjer det nye tal bagerst i talrækken
                SetShotText();
            }
        }

        /// <summary>
        /// Slet det bagerste tal i det forberende skud
        /// </summary>
        private static void DeleteNumber()
        {
            shot = (int)MathF.Floor(shot / 10f);
            SetShotText();
        }

        /// <summary>
        /// Skyd spillerens forberedte skud
        /// </summary>
        private static void Shoot()
        {
            int posX = Drawer.GetScreenPos(pos)[0];
            Spawner.CheckForHitEnemy(new int[] { posX, posX + width }, shot);
            shot = 0;
            SetShotText();
            shots.Add(new Drawer.Rect(width, Drawer.windowSize[1] - height, new float[] { pos[0], 0 }, true, '-', drawCol), shotLifeTime);
        }

        private static void SetShotText() { shotText.SetText(new string[] { shot.ToString("0") }); }
    }
}
