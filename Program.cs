﻿#pragma warning disable CA1416 // Validate platform compatibility
using framegen;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;

var random = new Random();

Dictionary<int, SolidBrush> map = new();

int dimensionW = 512;
int dimensionH = 960;
int maxFrames = 6180;

string date = DateTime.UtcNow.Ticks.ToString();
string outputPath = @"C:\framegen\output_" + date;
DirectoryInfo _ = Directory.CreateDirectory(outputPath);

SolidBrush black = new(Color.FromArgb(255, 0, 0, 0));

Console.WriteLine("Use colors.json? (y/n)");
bool useJson = false;
try
{
    useJson = Console.ReadLine().Trim().ToLowerInvariant()[0] == 'y';
}
catch (Exception)
{
    Console.WriteLine("Invalid input. Defaulting to false.");
}

List<string> colors = useJson ? Load().Colors : new List<string>();

for (int frame = 0; frame < maxFrames; frame++)
{
    for (int i = 0; i < (dimensionW * dimensionH); i++)
    {
        if (useJson)
        {
            // e.g.: "Black|#000000"
            string c = colors[random.Next(colors.Count)].Split("|")[1];
            map[i] = HexToBrush(c);
        }
        else
        {
            int r = random.Next(255);
            int g = random.Next(255);
            int b = random.Next(255);

            // Reduce extremes
            while (r > 199) r = random.Next(255);
            while (g > 199) g = random.Next(255);
            while (b > 222) b = random.Next(255);

            // Reduce magenta
            while (r > 166 && b > 166 && g < 133)
            {
                r = random.Next(255);
                g = random.Next(255);
                b = random.Next(255);
            }

            // Lighten
            while (r < 6 && g < 6 && b < 6)
            {
                r = random.Next(255);
                g = random.Next(255);
                b = random.Next(255);
            }

            map[i] = new SolidBrush(Color.FromArgb(255, r, g, b));
        }
    }
    Console.WriteLine("Generated " + map.Count + " SolidBrush colors for frame " + frame + ".");

    using (var b = new Bitmap(dimensionW, dimensionH))
    {
        using (Graphics g = Graphics.FromImage(b))
        {
            int w = 4, h = 4;
            int pixel = 0;

            for (int row = 0; row < dimensionH; row += h)
            {
                for (int col = 0; col < dimensionW; col += w)
                {
                    g.FillRectangle(map[pixel], col, row, w, h);
                    pixel++;
                }
            }
        }

        b.Save(outputPath + @"\framegen" + frame.ToString().PadLeft(9, '0') + @".png", ImageFormat.Png);
    }
}

static ColorDictionary Load()
{
    using (StreamReader r = new StreamReader("colors.json"))
    {
        string json = r.ReadToEnd();
        ColorDictionary? dictionary = JsonConvert.DeserializeObject<ColorDictionary>(json);
        return dictionary == null ? throw new ArgumentNullException(nameof(dictionary)) : dictionary;
    }
}

static SolidBrush HexToBrush(string? backgroundColor)
{
    if (backgroundColor == null) throw new ArgumentNullException(nameof(backgroundColor));

    Color color = ColorTranslator.FromHtml(backgroundColor);
    int r = Convert.ToInt16(color.R);
    int g = Convert.ToInt16(color.G);
    int b = Convert.ToInt16(color.B);

    return new SolidBrush(Color.FromArgb(255, r, g, b));
}
#pragma warning restore CA1416 // Validate platform compatibility