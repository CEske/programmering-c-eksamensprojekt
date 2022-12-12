using System;
using System.Collections.Generic;

namespace MathInvaders
{
    /// <summary>
    /// Gør det muligt at tegne firkanter og tekstbokse til konsollen
    /// </summary>
    public static class Drawer
    {
        public static char defaultChar { get; private set; } = '#'; //det standard tegn
        private static List<IDrawable> drawables = new List<IDrawable>(); //holder alle tegnbare objekter

        public static int[] windowSize = { 200, 45 }; //konsollens dimensioner

        /// <summary>
        /// Kaldes ved opstart
        /// </summary>
        public static void Init()
        {
            Console.SetWindowSize(windowSize[0], windowSize[1]); //sætter konsollen til de givne dimensioner
        }

        /// <summary>
        /// Tegner alle tegnbare objekter
        /// </summary>
        public static void Draw()
        {
            foreach (IDrawable id in drawables)
            {
                id.Draw();
            }
        }

        /// <summary>
        /// Sletter alt i den givne firkant
        /// </summary>
        public static void Erase(int x, int y, int width, int height)
        {
            string s = "";
            for (int n = 0; n < width; n++)
            {
                s += " ";
            }
            for (int i = 0; i < height; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(s);
            }
        }

        public static void Erase(int[] pos, int[] size)
        {
            Erase(pos[0], pos[1], size[0], size[1]);
        }

        /// <summary>
        /// Mapper de givne koordinater til linje/kolonne koordinater for konsolen
        /// </summary>
        public static int[] GetScreenPos(float[] pos)
        {
            return new int[] { (int)MathF.Floor(windowSize[0] * pos[0]), (int)MathF.Floor(windowSize[1] * pos[1]) };
        }

        public static float[] GetScreenPos(int posX, int posY)
        {
            return new float[] { posX / (float)windowSize[0], posY / (float)windowSize[1] };
        }

        /// <summary>
        /// Krav til alle tegnbare objekter
        /// </summary>
        public interface IDrawable
        {
            void Draw();
        }

        /// <summary>
        /// Klasse der gør det muligt at tegne firkanter til konsolen
        /// </summary>
        public class Rect : IDrawable 
        {
            private int width;
            private int height;
            private int posX;
            private int posY;
            private bool filled; //hvorvidt firkanten er fyldt eller ej
            private char drawChar; //firkantens tegn
            public ConsoleColor drawColor; //firkantens farve

            //Positionen er givet i linjer og kolonner
            public Rect(int _width, int _height, int[] _pos, bool _filled, char _drawChar, ConsoleColor _drawColor)
            {
                width = _width;
                height = _height;

                SetPos(_pos);

                filled = _filled;
                drawChar = _drawChar;
                drawColor = _drawColor;
                drawables.Add(this); //tilføjer objektet til listen over tegnbare objekter
            }

            //Positionen er givet i viewport koordinater
            public Rect(int _width, int _height, float[] _pos, bool _filled, char _drawChar, ConsoleColor _drawColor)
            {
                width = _width;
                height = _height;

                SetPos(_pos);

                filled = _filled;
                drawChar = _drawChar;
                drawColor = _drawColor;
                drawables.Add(this);
            }

            /// <summary>
            /// Tegner den givne firkant
            /// </summary>
            public virtual void Draw()
            {
                if(posX >= 0 && posX < windowSize[0])
                {
                    for (int n = 0; n < height; n++)
                    {
                        int rowPos = posY + n;
                        if (rowPos >= 0 && rowPos < windowSize[1])
                        {
                            Console.SetCursorPosition(posX, rowPos);
                            for (int i = 0; i < width; i++)
                            {
                                if (filled || (n == 0 || n == height - 1) || (i == 0 || i == width - 1))
                                {
                                    Console.ForegroundColor = drawColor;
                                    Console.Write(drawChar);
                                }
                                else
                                {
                                    Console.ForegroundColor = Console.BackgroundColor;
                                    Console.Write(drawChar);
                                }
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Fjerner den givne firkant
            /// </summary>
            public void Remove()
            {
                drawables.Remove(this);
                Erase(posX, posY, width, height);
            }

            /// <summary>
            /// Sætter firkantens position
            /// </summary>
            /// <param name="_pos">Positionen</param>
            public void SetPos(int[] _pos)
            {
                //Koordinater måles fra øverste venstre hjørne af objektet
                posX = _pos[0];
                posY = _pos[1];
            }

            public void SetPos(float[] _pos)
            {
                SetPos(GetScreenPos(_pos));
            }

            public int[] GetPos()
            {
                return new int[] { posX, posY };
            }
        }

        /// <summary>
        /// Firkantsobjekt der kan vise tekst
        /// </summary>
        public class TextBox : Rect //arver implicit IDrawable
        {
            protected string[] text;

            public TextBox(string[] _text, int _width, int _height, float[] _pos, char _drawChar, ConsoleColor _drawColor)
                : base(_width, _height, _pos, false, _drawChar, _drawColor)
            {
                text = _text;
            }

            public TextBox(string[] _text, int _width, int _height, int[] _pos, char _drawChar, ConsoleColor _drawColor)
                : base(_width, _height, _pos, false, _drawChar, _drawColor)
            {
                text = _text;
            }

            /// <summary>
            /// Tegner tekstboksen
            /// </summary>
            public override void Draw()
            {
                base.Draw();
                int counter = 0;
                foreach (string s in text)
                {
                    int posY = GetPos()[1] + 1 + counter;
                    if(posY >= 0 && posY < windowSize[1])
                    {
                        Console.SetCursorPosition(GetPos()[0] + 1, posY);
                        Console.Write(s);

                    }
                    counter++;
                }
            }

            public void SetText(string[] _text)
            {
                text = _text;
            }
        }
    }
}

