#pragma warning disable CA1416 // Validate platform compatibility
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

List<string> colors = Load().Colors;

for (int frame = 0; frame < maxFrames; frame++)
{
    for (int i = 0; i < (dimensionW * dimensionH); i++)
    {
        // e.g.: "Black|#000000"
        string c = colors[random.Next(colors.Count)].Split("|")[1];
        map[i] = HexToBrush(c);
    }
    Console.WriteLine("Generated " + map.Count + " SolidBrush colors for frame " + frame + ".");

    using (var b = new Bitmap(dimensionW, dimensionH))
    {
        using (Graphics g = Graphics.FromImage(b))
        {
            int w = 8, h = 8;
            int pixel = 0;

            for (int row = 0; row < dimensionH; row += h)
            {
                for (int col = 0; col < dimensionW; col += w)
                {
                    g.FillRectangle(map[pixel], col, row, w, h);
                    pixel++;
                }
            }

            // Black left side
            for (int row = 0; row < dimensionH; row += h / 2)
            {
                g.FillRectangle(black, dimensionW - 1, row, w / 2, h / 2);
            }

            // Black right side
            for (int row = dimensionH - 1; row >= 0; row -= h / 2)
            {
                g.FillRectangle(black, dimensionW - 1, row, w / 2, h / 2);
            }

            // Black bottom
            for (int col = 0; col < dimensionW; col += w / 2)
            {
                g.FillRectangle(black, col, dimensionH - 1, w / 2, h / 2);
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