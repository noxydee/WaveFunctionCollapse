// See https://aka.ms/new-console-template for more information
using System.Collections;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;

using WaveFunctionCollapse.GifService;
using WaveFunctionCollapse.WaveFunction;

Console.WriteLine("Wave function collapse!");

ICollapse collapse = new Collapse();
Bitmap bitmap = new Bitmap("InputFiles/PipeTest.bmp");

var watch = System.Diagnostics.Stopwatch.StartNew();

collapse.CollapseBitmap(bitmap, 500, 500, 10);
watch.Stop();
int totalFrames = 2500;

Console.WriteLine($"Wave function collapse took:{watch.ElapsedMilliseconds}ms to build");
Console.WriteLine("Started building gif file");
watch = System.Diagnostics.Stopwatch.StartNew();
    using (FileStream stream = File.Create("output.gif"))
    {
        GifWriter writer = new GifWriter(stream, 75);
        for (int i = 0; i < totalFrames; i++)
        {
            string imagePath = $"output/{i}.jpeg";
            Bitmap frame = new Bitmap(imagePath);
            writer.WriteFrame(frame);
        }
        stream.Close();
    }
watch.Stop();
Console.WriteLine($"Gif took:{watch.ElapsedMilliseconds}ms to build");