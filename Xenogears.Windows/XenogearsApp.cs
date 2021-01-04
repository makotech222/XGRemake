using System;
using Stride.Engine;

namespace Xenogears
{
    class XenogearsApp
    {
        static void Main(string[] args)
        {
            using (var game = new CustomGame())
            {
                game.Run();
            }
        }
    }

    internal class CustomGame : Game
    {
        public CustomGame()
        {
            IsFixedTimeStep = true;
            IsDrawDesynchronized = true;
            GraphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1 / 120.0f);
        }
    }
}
