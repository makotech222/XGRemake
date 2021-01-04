using System;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Xenogears.ResourceRipper
{
    class Program
    {
        static void Main(string[] args)
        {
            new ISOExtractor().Start();
            new SpriteRipper().Start();
            new PostProcessor().Start();
        }
    }
}
