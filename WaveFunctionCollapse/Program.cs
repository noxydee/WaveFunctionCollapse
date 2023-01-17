// See https://aka.ms/new-console-template for more information
using System.Collections;
using System.Drawing;
using System.Net.NetworkInformation;

using WaveFunctionCollapse.WaveFunction;

Console.WriteLine("Wave function collapse!");

ICollapse collapse = new Collapse();
Bitmap bitmap = new Bitmap("InputFiles/PipeTest.bmp");

var watch = System.Diagnostics.Stopwatch.StartNew();

collapse.CollapseBitmap(bitmap, 500, 500, 10);
watch.Stop();

Console.WriteLine($"Wave function collapse took:{watch.ElapsedMilliseconds}ms to build");


var xx = 10;