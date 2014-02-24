using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace FiniteStateMachine
{
    public static class Printer
    {
        public static int max = 21;
        public static String[] display = new String[max];
        public static int[] displaytype = new int[max];
        public static int top = 0;
        public static int amount = 0;

        public static void Print(int id, string message)
        {
            System.Console.WriteLine(id + " " + message + "\n");
            top++;
            if (top >= max) { top = 0; }
            if (amount < 21) { amount++; }
            display[top] = id + " " + message;
            displaytype[top] = id;
        }

        public static void PrintMessageData(string message)
        {
            System.Console.WriteLine("M " + message + "\n");
            top++;
            if (top >= max) { top = 0; }
            if (amount < 21) { amount++; }
            display[top] =  message;
            displaytype[top] = 5;
        }

        public static void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            spriteBatch.Begin();
            int index = top;
            for (int i = 0; i < amount; i++)
            {
                Color textColor;
                switch((int)(displaytype[index]))
                {
                    case 0:
                        textColor = Color.Blue;
                        break;
                    case 1:
                        textColor = Color.Purple;
                        break;
                    case 2:
                        textColor = Color.Red;
                        break;
                    case 3:
                        textColor = Color.Yellow;
                        break;
                    case 4:
                        textColor = Color.Black;
                        break;
                    default:
                        textColor = Color.White;
                        break;
                }
                spriteBatch.DrawString(spriteFont, display[index], new Vector2(10.0f, 450 - (i * 22)), textColor);
                index--;
                if (index < 0)
                {
                    index = max - 1;
                }
            }
            spriteBatch.End();
        }
    }
}
