using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Threading;

namespace CSharpSFML
{
    public static class Scene
    {
        private static Random Rand = new Random();
        private static float Score;
        private static bool clickable;

        public static uint Count;
        public static Action<string> Output;//Output function for debugging.
        public static Action<string> Result;//Display Results
        public static Text Text = new Text();//Text to draw with
        public static string title = "Hello Poke";
        public static string info = "Green(Play), Yellow(TODO), Red(EXIT)";

        private static List<Shape> shapes;
        public static Func<Vector2f> Bot;
        public static int BotDelay = 1000;

        public static void Debug(string str)
        {
            Output?.Invoke(str);//IF not null, invoke this function
        }
        public static void Print(string str, RenderWindow Window, Color color, Vector2f position = new Vector2f(), bool center = false)
        {
            if(Text.Font != null)
            {
                Text.DisplayedString = str;
                Text.FillColor = color;
                if (center)
                {
                    Text.Position = new Vector2f(
                        position.X - Text.GetGlobalBounds().Width / 2, 
                        position.Y - Text.GetGlobalBounds().Height / 2
                    );
                }
                else
                {
                    Text.Position = position;
                }
                Window.Draw(Text);
            }
        }
        public static void Title(Game game)
        {
            //Making our rectangles a third of the screen, relative to x-axis
            float thirdOfScreen = game.Window.Size.X / 3;

            //Determine the rectangles size
            Vector2f size = new Vector2f(thirdOfScreen, game.Window.Size.Y);
            RectangleShape play = new RectangleShape(size);
            RectangleShape option = new RectangleShape(size);
            RectangleShape quit = new RectangleShape(size);

            //Set position of rectangles
            play.Position = new Vector2f(0, 0);
            option.Position = new Vector2f(thirdOfScreen, 0);
            quit.Position = new Vector2f(thirdOfScreen * 2, 0);

            //Color the rectangles
            play.FillColor = Color.Green;
            play.OutlineColor = Color.Black;
            option.FillColor = Color.Yellow;
            option.OutlineColor = Color.Black;
            quit.FillColor = Color.Red;
            quit.OutlineColor = Color.Black;

            //Setup event handlers:
            clickable = true;
            EventHandler<SFML.Window.MouseButtonEventArgs> handle = null;
            handle = (sender, e) =>
            {
                if (clickable)
                {
                    if (play.GetGlobalBounds().Contains(e.X, e.Y))
                    {
                        game.Window.MouseButtonPressed -= handle;
                        clickable = false;
                        game.process = () => { Generate(game); };//Setup stage 1 (function)
                    }
                    else if (option.GetGlobalBounds().Contains(e.X, e.Y))
                    {
                        game.Window.MouseButtonPressed -= handle;
                        clickable = false;
                        game.process = () => { Generate(game, 10, true); };//Setup stage 1 (function)
                    }
                    else if (quit.GetGlobalBounds().Contains(e.X, e.Y))
                    {
                        //QUIT GAME
                        game.Running = false;
                    }
                }
            };
            game.Window.MouseButtonPressed += handle;

            //Setup proccess:
            game.Update.Clear();
            game.Draw.Clear();
            game.Draw.Add((delta, Window) =>
            {
                Window.Clear();
                Window.Draw(play);
                Window.Draw(option);
                Window.Draw(quit);
                Print(title, Window, Color.Blue, new Vector2f(Window.Size.X / 2, Window.Size.Y / 2), true);
                Print(info, Window, Color.Blue, new Vector2f(Window.Size.X / 2, Window.Size.Y / 2 + 100), true);
                Window.Display();
            });
        }
        public static void Generate(Game game, uint n = 10, bool bot = false)
        {
            Score = 0;
            Count = n;
            float width = game.Window.Size.X / n;
            float height = game.Window.Size.Y / n;
            Color[] color = new Color[] { Color.Green, Color.Red };
            int count = 0;
            while (!Convert.ToBoolean(count))//Retry if no greens
            {
                shapes = new List<Shape>();
                for (int y = 0; y < Count; ++y)
                {
                    for (int x = 0; x < Count; ++x)
                    {
                        RectangleShape shape = new RectangleShape(
                            new Vector2f(width, height));
                        shape.Position = new Vector2f(width * x, height * y);
                        shape.FillColor = Convert.ToBoolean(Rand.Next(2)) ? Color.Red : Color.Green;
                        if (shape.FillColor == Color.Green)
                            ++count;
                        shape.OutlineColor = Color.Black;
                        shapes.Add(shape);
                    }
                }
            }
            game.Draw.Clear();
            game.Draw.Add((delta, Window) => {
                Window.Clear();
                foreach (Shape shape in shapes)
                    if(shape != null)
                        Window.Draw(shape);
                Window.Display();
            });

            Clock clock = new Clock();
            if(bot && Bot != null)
            {
                game.updatePerSecond = BotDelay;
                game.Update.Add((delta, Window) =>
                {
                    Vector2f pos = Bot.Invoke();
                    foreach (Shape shape in shapes.ToArray())
                    {
                        if(shape != null)
                        {
                            if (shape.GetGlobalBounds().Contains(pos.X, pos.Y))
                            {
                                if (shape.FillColor == Color.Green)
                                {
                                    shapes.Remove(shape);
                                    --count;
                                    Score += 1000;
                                }
                                else
                                {
                                    Score -= 250;
                                }
                            }
                        }
                    }
                    if (count < 1)
                    {
                        title = "You've won with a of: " + Score / clock.ElapsedTime.AsSeconds() + "\n";
                        game.process = () => { Title(game); };
                    }
                });
            }
            else if(!bot)//Play
            {
                EventHandler<SFML.Window.MouseButtonEventArgs> handle = null;
                handle = (sender, e) =>
                {
                    foreach (Shape shape in shapes.ToArray())
                    {
                        if (shape.GetGlobalBounds().Contains(e.X, e.Y))
                        {
                            if (shape.FillColor == Color.Green)
                            {
                                shapes.Remove(shape);
                                --count;
                                Score += 1000;
                            }
                            else
                            {
                                Score -= 250;
                            }
                        }
                    }
                    if (count < 1)
                    {
                        game.Window.MouseButtonPressed -= handle;
                        title = "You've won with a of: " + Score / clock.ElapsedTime.AsSeconds() + "\n";
                        game.process = () => { Title(game); };
                    }
                };
                game.Window.MouseButtonPressed += handle;
            }
            else
            {
                title = "No Bot was programed\n";
                game.process = () => { Title(game); };
            }
        }
    };
}