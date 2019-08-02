using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace CSharpSFML
{
    public class Game//Simple Game Class to demonstrate the game.
    {
        public readonly RenderWindow Window;//The Window.

        private readonly Clock Clock;//The clock to acquire delta time.

        public readonly List<Action<float, RenderWindow>> Update;
        public readonly List<Action<float, RenderWindow>> Draw;
        public readonly List<Action<float, RenderWindow>> Print;
        public Action process;

        public bool Running;//Running bool variable.

        //Some simple variables for managing how long to sleep, how often to update, how often to draw, and how often to print.
        public int sleepPerSecond;//This is how many milliseconds the game loop sleeps each cycle.

        public int updatePerSecond;//This is how many milliseconds before we do the update procedure.
        public int drawPerSecond;//This is how many milliseconds before we do the draw procedure.
        public int oncePerSecond;//This is how many milliseconds before we do the print procedure.

        public int updateAccumulator;//Accumulator for update. Units are in milliseconds.
        public int drawAccumulator;//Accumulator for draw. Units are in milliseconds.
        public int printAccumulator;//Accumulator for print. Units are in milliseconds.

        public int fpsAccumulator = 0;//Accumulator for Frames per Second.

        public Game(string title, uint width, uint height)
        {
            Window = new RenderWindow(new VideoMode(width, height), title);
            Update = new List<Action<float, RenderWindow>>();
            Draw = new List<Action<float, RenderWindow>>();
            Print = new List<Action<float, RenderWindow>>();
            Clock = new Clock();

            //Handles closed button pressed.
            Window.Closed += new EventHandler((sender, e) =>
            {
                Running = false;
                ((Window)sender).Close();
            });

            sleepPerSecond = 1000 / 240 + 1;//Occurs about 240 times per second. This is how many milliseconds the game loop sleeps each cycle.

            updatePerSecond = 1000 / 120 + 1;//Occurs about 120 times per second. This is how many milliseconds before we do the update procedure.
            drawPerSecond = 1000 / 60 + 1;//Occurs about 60 times per second. This is how many milliseconds before we do the draw procedure.
            oncePerSecond = 1000;//Occurs about 1 times per second. This is how many milliseconds before we do the print procedure.

            updateAccumulator = 0;//Accumulator for update. Units are in milliseconds.
            drawAccumulator = 0;//Accumulator for draw. Units are in milliseconds.
            printAccumulator = 0;//Accumulator for print. Units are in milliseconds.

            fpsAccumulator = 0;//Accumulator for Frames per Second.

            Scene.Title(this);
        }
        public void Run()
        {
            Running = true;//Assign our running condition to true.
            Clock.Restart();//Restart the clock to give us a fresh clock timer before the game loop.
            while (Running)
            {
                int tick = Clock.ElapsedTime.AsMilliseconds();//Acquire clock timer since last Restart in milliseconds.
                Clock.Restart();//Restart the clock timer; set ElapsedTime to zero.

                //Handles our handlers: Rate == how fast this game loop cycles == relative to how long 
                Window.DispatchEvents();

                updateAccumulator += tick;
                if (updateAccumulator > updatePerSecond)
                {
                    if (Update.Count > 0 && Update != null)
                    {
                        foreach (Action<float, RenderWindow> update in Update)
                            update?.Invoke(updateAccumulator, Window);
                    }
                    updateAccumulator = 0;
                }
                drawAccumulator += tick;
                if (drawAccumulator > drawPerSecond)
                {
                    foreach (Action<float, RenderWindow> draw in Draw)
                        draw?.Invoke(drawAccumulator, Window);
                    drawAccumulator -= drawPerSecond;
                    ++fpsAccumulator;
                }

                printAccumulator += tick;
                if (printAccumulator > oncePerSecond)
                {
                    foreach (Action<float, RenderWindow> print in Print)
                        print?.Invoke(drawAccumulator, Window);
                    printAccumulator -= oncePerSecond;
                    fpsAccumulator = 0;
                }

                if(process != null)
                {
                    process.Invoke();
                    process = null;
                }
                //Sleep to conserve processing prower and limit game loop ticks.
                System.Threading.Thread.Sleep(sleepPerSecond);//Force game loop to sleep X milliseconds.
            }
        }
    };
}