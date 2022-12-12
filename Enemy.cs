using System;

namespace MathInvaders
{
    public class Enemy
    {
        //Laver forskellige varialber der skal bruges
        private Drawer.TextBox text;
        public float[] pos { get; private set; }
        private int width;
        private const int height = 3;
        private string question;
        private int answer;
        private float speed { get => 0.04f * Spawner.fartMultiplier; }
        //Indhenter sætter vilkårnen for fjenden
        public Enemy(int x, int y, ConsoleColor col, string _question, int _answer)
        {
            question = _question;
            answer = _answer;
            //Bestemmer lægnden
            width = question.Length + 2;
            pos = Drawer.GetScreenPos(x, y);
            text = new Drawer.TextBox(new string[] { question }, width, height, pos, Drawer.defaultChar, col);
        }

        public void Update()
        {
            //Udregner Y position
            int posY = Drawer.GetScreenPos(pos)[1];
            //Sørger for at farteren stiger
            pos[1] += speed * GameManager.deltaTime;
            //Sætter en ny Y pos
            int newPosY = Drawer.GetScreenPos(pos)[1];
            //Hvis at fjenden rammer bunden slutter spillet
            if (newPosY == Drawer.windowSize[1]) GameManager.gameOver = true;
            //Gør så du ikke tegner noget uden for konsolen
            if (posY != newPosY && posY >= 0) Drawer.Erase(Drawer.GetScreenPos(pos)[0], posY, width, 1);
            //Textboxens posistion på skærmen ændres hver update 
            text.SetPos(pos);
        }
        //Denne metode vil kigge på om svaret er rigtigt hvis den er fjernes fjenden
        public void TrySolve(int guess)
        {
            if (guess == answer)
            {
                text.Remove();
                Spawner.ObjekterList.Remove(this);
            }
        }
        //Denne metode returner fjendes position til spawner
        public int[] HorizontalSpaceTaken()
        {
            int posX = Drawer.GetScreenPos(pos)[0];
            return new int[] { posX, posX + width };
        }
    }
}
