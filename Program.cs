using System;

using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.Collections.Generic;

namespace CSharpSFML
{
    class Program
    {
        static void Main(string[] args)//Test example for this game.
        {
            Scene.Output = (str) => {
                Console.WriteLine(str);
            };//Setup Debugger
            Scene.Text.Font = new Font("DroidSans.ttf");
            Game game = new Game("Example for Poke", 800, 600);
            Scene.Title(game);//Set Scene to title.
            Test(game);
            game.Run();//Run the game.
        }

        /*
         * Modify the Test Code
         * 
         * You currently know that the screen consist of of 10x10 rectangles.
         * Build an AI (machine) to acquire better result.
         * 
         * The return value is the next position to select (It pretends to be a mouse click)
         */
        static void Test(Game game)
        {
            Scene.BotDelay = 1000 / 10;//Half second delay on bot for testing.
            float x = 0, y = 0;
            Scene.Bot = () =>
            {
                uint size = Scene.Count; //Count
                uint sizeX = game.Window.Size.X / 10;//Size of boxs width
                uint sizeY = game.Window.Size.Y / 10;//Size of boxs height
                Vector2f pos = new Vector2f(x*sizeX,y*sizeY);
                if(++x >= size)
                {
                    x = 0;
                    ++y;
                }
                return pos;
            };
        }
    }
}