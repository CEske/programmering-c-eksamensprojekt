using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace MathInvaders
{
    public static class Spawner
    {
        // sværhedsgraden sættes til 1 som default - difficulty assignes og erklæres
        private static int difficulty = 1;
        // den maksimale sværhedsgrad sættes til 20 - maxDifficulty assignes og erklæres
        private const int maxDifficulty = 20;

        // fartMultiplier assignes og erklæres til et float. "get" er sat, så den afhænger af flere variabler og derfor ikke kan assignes senere hen,
        public static float fartMultiplier { get => 1 + difficulty / (float)maxDifficulty; }

        // ObjekterList defineres til at kunne kaldes fra alle andre, men ikke kunne kaldes fra andre classes.
        public static List<Enemy> ObjekterList { get; private set; } = new List<Enemy>();

        // spawnTime og spawnTimer assignes og defineres 
        private const float spawnTime = 6;
        private static float spawnTimer = 4;

        // spawnPosY defineres og assignes som default start position
        private const int spawnPosY = -3;

        // funktion Update defineres
        public static void Update()
        {
            // spawnTimer får tilføjet variablen deltaTime fra GameManager af
            spawnTimer += GameManager.deltaTime;
            // hvis spawnTimer er større eller lig med spawnTime, så køres koden
            if (spawnTimer >= spawnTime)
            {
                // tilfældig defineres til at være Random
                Random tilfældig = new Random();
                // integer type assignes til at være et tal mellem 0 og 100
                int type = tilfældig.Next(0, 100);
                // der tjekkes om type er mindre end 10
                if (type < 10)
                {
                    // grundtal og eksponent defineres og assignes til at være lig med et tilfældigt tal
                    int grundtal = RandomNumberGenerator.GetInt32(0, 10);
                    int eksponent = RandomNumberGenerator.GetInt32(0, 3);
                    // Der defineres et nyt objekt
                    string spørgsmål = grundtal + "^" + eksponent;
                    // Der tilføjes et nyt objekt til listen
                    ObjekterList.Add(new Enemy(tilfældig.Next(0, Drawer.windowSize[0] - spørgsmål.Length + 2), spawnPosY, ConsoleColor.Red, spørgsmål, (int)MathF.Pow(grundtal, eksponent)));
                }
                else if (type < 20)
                {
                    // grundtal og eksponent defineres og assignes til at være lig med et tilfældigt tal
                    int første = RandomNumberGenerator.GetInt32(0, 10);
                    int anden = RandomNumberGenerator.GetInt32(0, 10);
                    string spørgsmål = første + "*" + anden;
                    // Der tilføjes et nyt objekt til listen
                    ObjekterList.Add(new Enemy(tilfældig.Next(0, Drawer.windowSize[0] - spørgsmål.Length + 2), spawnPosY, ConsoleColor.Blue, spørgsmål, første * anden));
                }
                else if (type < 70)
                {
                    // grundtal og eksponent defineres og assignes til at være lig med et tilfældigt tal
                    int første = RandomNumberGenerator.GetInt32(0, 50);
                    int anden = RandomNumberGenerator.GetInt32(0, 50);
                    string spørgsmål = første + "+" + anden;
                    // Der tilføjes et nyt objekt til listen
                    ObjekterList.Add(new Enemy(tilfældig.Next(0, Drawer.windowSize[0] - spørgsmål.Length + 2), spawnPosY, ConsoleColor.Green, spørgsmål, første + anden));
                }
                // spawnTimer sættes til at være lig med 0 for at der ikke hele tiden spawner nye enemies
                spawnTimer = 0;
                // hvis sværhedsgraden er mindre end den maksimale sværhedsgrad, så øges sværhedsgraden med en enkelt.
                if (difficulty < maxDifficulty) difficulty++;
            }

            // ObjekterList loopes igennem og "e" assignes til at Enemy
            // Herefter bruges funktionen Update fra Enemy class til at opdatere enemy's position
            foreach (Enemy e in ObjekterList) e.Update();
        }

        // funktionen CheckForHitEnemy defineres med to argumenter - spillerens skud position og selve svaret
        public static void CheckForHitEnemy(int[] shotPos, int shot)
        {
            // enemiesInLine defineres
            List<int> enemiesInLine = new List<int>();
            // counter defineres som integer og assignes til 0
            int counter = 0;
            // loop af enemies fra ObjekterList
            foreach (Enemy e in ObjekterList)
            {
                // takenSpace assignes og defineres til at være lig med objektets start position og slut position
                int[] takenSpace = e.HorizontalSpaceTaken();
                // her tjekkes om skuddet er indenfor objektets start og slut position
                // hvis skuddet er indenfor, så tilføjes objektet's index til enemiesInline. Enemy's index gemmes i tilfælde af, at det senere skal slettes fra listen.
                if ((shotPos[0] >= takenSpace[0] && shotPos[0] <= takenSpace[1]) || (shotPos[1] >= takenSpace[0] && shotPos[1] <= takenSpace[1])) enemiesInLine.Add(counter);
                counter++;
            }

            // lowestY og chosen assignes og defineres til at være -1
            int lowestY = -1;
            int chosen = -1;
            // loop igennem enemiesInLine
            foreach (int i in enemiesInLine)
            {
                // y assignes og defineres til at være objektets y position
                int y = Drawer.GetScreenPos(ObjekterList[i].pos)[1];
                // hvis lowestY er -1 eller y er mindre end lowestY, så assignes chosen til at være lig med "i" og lowestY til at være lig med "y"
                if (lowestY == -1 || y < lowestY)
                {
                    chosen = i;
                    lowestY = y;
                }
            }

            // hvis chosen ikke er -1, så forsøges objektet at blive løst med skudet
            if (chosen != -1) ObjekterList[chosen].TrySolve(shot);
        }
    }
}
