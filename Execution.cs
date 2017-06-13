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
                    // ignore spaces
                    case ' ':
                        break;
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
                    // custom 3-digit hex color code
                    //@af0f
                    // color code: a
                    // hex color: #FF00FF
                    case '@':
                        i++;
                        colorCode = code[i];
                        i++;
                        hexColor = code.Substring(i, 3);
                        hexColor = hexColor[0].ToString() + hexColor[0] + hexColor[1].ToString() + hexColor[1] + hexColor[2].ToString() + hexColor[2];
                        i += 2;
                        color = ColorTranslator.FromHtml("#" + hexColor);
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
                            {
                                i--;
                                break;
                            }
                        }

                        int end = (i + 1) - start;
                        if (i + 1 >= code.Length)
                            end = i - start;

                        string sub = code.Substring(start, end);
                        string[] split = sub.Split(',');
                        int x, y, w, h;
                        // x,y,w,h
                        if (split.Length == 4)
                        {
                            if (!int.TryParse(split[0], out x) || !int.TryParse(split[1], out y) || !int.TryParse(split[2], out w) || !int.TryParse(split[3], out h))
                            {
                                Console.WriteLine("[FATAL] Invalid rectangle function usage!");
                                return;
                            }
                        }
                        // x,y,size (square)
                        else if (split.Length == 3)
                        {
                            if (!int.TryParse(split[0], out x) || !int.TryParse(split[1], out y) || !int.TryParse(split[2], out w))
                            {
                                Console.WriteLine("[FATAL] Invalid rectangle function usage!");
                                return;
                            }
                            h = w;
                        }
                        // w,h
                        else if (split.Length == 2)
                        {
                            if (!int.TryParse(split[0], out w) || !int.TryParse(split[1], out h))
                            {
                                Console.WriteLine("[FATAL] Invalid rectangle function usage!");
                                return;
                            }
                            x = 0;
                            y = 0;
                        }
                        // size
                        else if (split.Length == 1)
                        {
                            if (!int.TryParse(split[0], out w))
                            {
                                Console.WriteLine("[FATAL] Invalid rectangle function usage!");
                                return;
                            }
                            h = w;
                            x = 0;
                            y = 0;
                        }
                        else
                        {
                            Console.WriteLine("[FATAL] Invalid rectangle function usage!");
                            return;
                        }

                        Brush brush = new SolidBrush(colorCodes[colorCode]);
                        g.FillRectangle(brush, x, y, w, h);
                        break;
                    // grid of lines
                    // G04
                    case 'G':
                        i++;
                        colorCode = code[i];

                        i++;
                        start = i;
                        // loop through characters after G<ColorChar> until reaches character other than integer
                        for (; i < code.Length; i++)
                        {
                            char c = code[i];
                            // if it is a "," or integer, continue
                            if (int.TryParse(c.ToString(), out int o) || c == ',')
                                continue;
                            else
                            {
                                i--;
                                break;
                            }
                        }

                        end = (i + 1) - start;
                        if (i + 1 >= code.Length)
                            end = i - start;

                        sub = code.Substring(start, end);
                        int spacing;
                        if (!int.TryParse(sub, out spacing))
                        {
                            Console.WriteLine("[FATAL] Invalid grid function usage!");
                            return;
                        }

                        Pen pen = new Pen(new SolidBrush(colorCodes[colorCode]));
                        for (x = 0; x < bitmap.Width; x += spacing)
                            g.DrawLine(pen, x, 0, x, bitmap.Height);

                        for (y = 0; y < bitmap.Height; y += spacing)
                            g.DrawLine(pen, 0, y, bitmap.Width, y);
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
