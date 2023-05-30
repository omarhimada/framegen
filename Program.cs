#pragma warning disable CA1416 // Validate platform compatibility
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

var random = new Random();

Dictionary<int, SolidBrush> map = new();
int dimensionW = 512;
int dimensionH = 960;
int maxFrames = 3354;
string date = DateTime.UtcNow.Ticks.ToString();
string outputPath = @"C:\framegen\output_" + date;
DirectoryInfo _ = Directory.CreateDirectory(outputPath);
for (int frame = 0; frame < maxFrames; frame++)
{
    for (int i = 0; i < (dimensionW * dimensionH); i++)
    {
        int r = random.Next(255);
        int g = random.Next(255);
        int b = random.Next(255);

        while (r == 80 && g == 80 && b == 80)
        {
            r = random.Next(255);
            g = random.Next(255);
            b = random.Next(255);
        }

        map[i] = new SolidBrush(Color.FromArgb(255, r, g, b));
        // Console.WriteLine("R: " + r + " G: " + g + " B: " + b);
    }
    Console.WriteLine("Generated " + map.Count + " SolidBrush colors for frame " + frame + ".");

    using (var b = new Bitmap(dimensionW, dimensionH))
    {
        // 000000000
        using (Graphics g = Graphics.FromImage(b))
        {
            int w = 1, h = 1;
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
        frame++;
    }
}
#pragma warning restore CA1416 // Validate platform compatibility