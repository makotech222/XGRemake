using System;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

namespace Xenogears.ResourceRipper
{
    public static class Tools
    {
        public static byte ord(char c)
        {
            return (byte)c;
        }

        public static List<byte> chrint(int val, int b = 0)
        {
            var bytes = new List<byte>();
            for (int i = 0; i < b; i++)
            {
                byte a = (byte)(val & 0xff);
                bytes.Add(a);
                val = val >> 8;
            }
            while (bytes.Count < b)
            {
                bytes.Add(0);
            }
            return bytes;
        }

        public static int ordint(string str)
        {
            if (str.ToInt32() != null)
                return str.ToInt32().Value;
            int result = 0;
            for (int i = 0; i < str.Length; i++)
            {
                int b = ord(str[i]);
                result += (b << (i * 8));
            }
            return result;
        }

        public static int str2int(string str, int pos, int length)
        {
            string sub = str.Substring(pos, length);
            return ordint(sub);
        }

        public static class clut2bmp
        {
            public static void Execute(string inputFile)
            {
                FileInfo f = new FileInfo(inputFile);
                if (f.Extension.ToUpper() == ".CLUT")
                    ClutToPng(f);
                else if (f.Extension.ToUpper() == ".RGBA")
                    RgbaToPng(f);
            }

            public static void RgbaToPng(FileInfo file)
            {
                var rgbaBytes = File.ReadAllBytes(file.FullName).ToList();
                //var rgba = File.ReadAllText(file.FullName);
                var width = BitConverter.ToInt32(rgbaBytes.Skip(4).Take(4).ToArray());
                var height = BitConverter.ToInt32(rgbaBytes.Skip(8).Take(4).ToArray());
                var head = BmpHeader(width, height);
                var data = new List<byte>();
                while (height > 0)
                {
                    height--;
                    var pos = 12 + (height * width * 4);
                    data.AddRange(rgbaBytes.Skip(pos).Take(width * 4));
                }
                head.AddRange(data);
                ConvertBmpToPng(head.ToArray(), Path.Combine(file.Directory.FullName, Path.GetFileNameWithoutExtension(file.FullName) + ".png"));
            }

            public static void ClutToPng(FileInfo file)
            {
                var rgbaBytes = File.ReadAllBytes(file.FullName).ToList();
                var cc = BitConverter.ToInt32(rgbaBytes.Skip(4).Take(4).ToArray());
                var width = BitConverter.ToInt32(rgbaBytes.Skip(8).Take(4).ToArray());
                var height = BitConverter.ToInt32(rgbaBytes.Skip(12).Take(4).ToArray());

                var head = BmpHeader(width, height);
                var pal = new List<List<byte>>();
                var pos = 16;
                for (int i = 0; i < cc; i++)
                {
                    pal.Add(new List<byte>(rgbaBytes.Skip(pos).Take(4)));
                    pos += 4;
                }
                var data = new List<byte>();
                while (height > 0)
                {
                    height--;
                    pos = 16 + (cc * 4) + (height * width);
                    var pix = rgbaBytes.Skip(pos).Take(width).ToList();
                    while(pix.Count != width)
                    {
                        pix.Add(0);
                    }
                    for (int x = 0; x < width; x++)
                    {
                        if (pix.Count >= width)
                        {
                            var px = pix[x];
                            var bytes = pal[px];
                            data.AddRange(bytes);
                        }
                    }
                }
                head.AddRange(data);
                File.WriteAllBytes("test.bmp", head.ToArray());
                ConvertBmpToPng(head.ToArray(), Path.Combine(file.Directory.FullName, Path.GetFileNameWithoutExtension(file.FullName) + ".png"));
            }

            public static List<byte> BmpHeader(int width, int height)
            {
                var of = 0x7a;
                var sz = width * height * 4;
                List<byte> head = new List<byte>();
                head.AddRange(Encoding.Default.GetBytes(new char[] { 'B', 'M' }));
                head.AddRange(chrint(of + sz, 4));

                head.AddRange(chrint(0, 2)); // unused
                head.AddRange(chrint(0, 2)); // unused
                head.AddRange(chrint(of, 4)); // data offset

                head.AddRange(chrint(0x6c, 4)); // dib head size
                head.AddRange(chrint(width, 4)); // width
                head.AddRange(chrint(height, 4)); // height
                head.AddRange(chrint(1, 2)); // plane
                head.AddRange(chrint(32, 2)); // bit-per-pixel
                head.AddRange(chrint(3, 4)); // compression
                head.AddRange(chrint(sz, 4)); // data size
                head.AddRange(chrint(72, 4)); // density x
                head.AddRange(chrint(72, 4)); // density y
                head.AddRange(chrint(0, 4)); // palette num
                head.AddRange(chrint(0, 4)); // palette num - important

                // RGBA order
                byte BYTE = 255;
                byte ZERO = 0;
                head.AddRange(new byte[] { BYTE, ZERO, ZERO, ZERO });
                head.AddRange(new byte[] { ZERO, BYTE, ZERO, ZERO });
                head.AddRange(new byte[] { ZERO, ZERO, BYTE, ZERO });
                head.AddRange(new byte[] { ZERO, ZERO, ZERO, BYTE });

                head.AddRange(Encoding.Default.GetBytes(new char[] { 'R', 'G', 'B', 's' }));
                for (int i = 0; i < 0x24; i++)
                {
                    head.Add(ZERO);
                }

                head.AddRange(chrint(0, 4)); // gamma red
                head.AddRange(chrint(0, 4)); // gamma green
                head.AddRange(chrint(0, 4)); // gamma blue

                return head;
            }

            public static void ConvertBmpToPng(byte[] file, string fileName)
            {
                using (var ms = new MemoryStream(file))
                {
                    Bitmap img = new Bitmap(ms);
                    img.MakeTransparent();
                    img.Save(fileName, ImageFormat.Png);
                }
            }
        }
    }
}