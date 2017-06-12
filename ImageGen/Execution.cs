using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Reflection;

namespace ImageGen
{
    class Execution
    {
        private static Dictionary<char, Color> colorCodes = new Dictionary<char, Color>();

        // load ColorCodes.txt
        public static void LoadColorCodes() {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "ImageGen.ColorCodes.txt";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    // <ColorChar> <A> <R> <G> <B>
                    string[] split = line.Split(' ');
                    if (split.Length != 5)
                        continue;

                    char colorChar;
                    int a, r, g, b;
                    if (!char.TryParse(split[0], out colorChar) || !int.TryParse(split[1], out a) || !int.TryParse(split[2], out r) || !int.TryParse(split[3], out g) || !int.TryParse(split[4], out b))
                        continue;
                    colorCodes.Add(colorChar, Color.FromArgb(a, r, g, b));
                }
            }
        }

        public static void Execute(string code)
        {
            Bitmap bitmap = GenerateBitmap(code);
            bool explicitDimensions = bitmap != null;
            if (bitmap == null)
            {
                Console.WriteLine("[WARN] Did you forget to give the image dimensions (<w>x<h> <code>)? Assuming w=600, h=400");
                bitmap = new Bitmap(600, 400);
            }

            Graphics g = Graphics.FromImage(bitmap);
            // get rid of "<w>x<h>", if there is one
            int i = 0;
            if (explicitDimensions)
            {
                string[] split = code.Split(' ');
                i = split[0].Length + 1;
            }

            for (; i < code.Length; i++)
            {
                char ch = code[i];
                switch (ch)
                {
                    // custom 6-digit hex color code
                    //#acdcdcd
                    // color code: a
                    // hex color: cdcdcd
                    case '#':
                        i++;
                        char colorCode = code[i];
                        i++;
                        string hexColor = code.Substring(i, 6);
                        i += 5;
                        Color color = ColorTranslator.FromHtml("#" + hexColor);
                        colorCodes.Add(colorCode, color);
                        break;
                    case 'F':
                        // fill entire image
                        // example:
                        // FB
                        // fill image with blue color
                        i++;
                        colorCode = code[i];
                        
                        g.Clear(colorCodes[colorCode]);
                        break;
                    case 'R':
                        // filled rectangle
                        // example:
                        // RG50,50,100,100
                        // green rectangle at (50,50) with dimensions 100x100
                        i++;
                        colorCode = code[i];
                        // add one to get to coords
                        i++;
                        int start = i;
                        // loop through characters after R<ColorChar> until reaches character other than "," or integer
                        for (; i < code.Length; i++)
                        {
                            char c = code[i];
                            // if it is a "," or integer, continue
                            if (int.TryParse(c.ToString(), out int o) || c == ',')
                                continue;
                            else
                                break;
                        }

                        string sub = code.Substring(start, i - start);
                        // to get "50, 50, 100, 100"
                        string[] split = sub.Split(',');
                        if (split.Length != 4)
                        {
                            Console.WriteLine("[FATAL] Invalid rectangle function usage! Example: RG50,50,100,100");
                            return;
                        }

                        int x, y, w, h;
                        if (!int.TryParse(split[0], out x) || !int.TryParse(split[1], out y) || !int.TryParse(split[2], out w) || !int.TryParse(split[3], out h))
                        {
                            Console.WriteLine("[FATAL] Invalid rectangle function usage! Example: RG50,50,100,100");
                            return;
                        }

                        Brush brush = new SolidBrush(colorCodes[colorCode]);
                        g.FillRectangle(brush, x, y, w, h);
                        break;
                    default:
                        Console.WriteLine("[WARN] Ignoring unknown character " + ch + " at position " + i + ".");
                        break;
                }
            }
            g.Dispose();

            Console.WriteLine("Generated image saved to out.png");
            bitmap.Save("out.png", System.Drawing.Imaging.ImageFormat.Png);
            bitmap.Dispose();
        }

        private static Bitmap GenerateBitmap(string code)
        {
            // <width>x<height> <code>
            string[] split = code.Split(' ');
            if (split.Length == 0)
                return null;

            string[] dimensionsSplit = split[0].Split('x');
            if (dimensionsSplit.Length < 2)
                return null;

            int width, height;
            if (int.TryParse(dimensionsSplit[0], out width) && int.TryParse(dimensionsSplit[1], out height))
                return new Bitmap(width, height);
            return null;
        }
    }
}
