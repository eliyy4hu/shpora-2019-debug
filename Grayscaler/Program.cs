using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using libbmp;

namespace Grayscaler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                args = new[]
                    {Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().FullName), "sample.bmp")};
            var sw = Stopwatch.StartNew();
            var picture = imageprocessor.load(args[0]);
            var greyscalePicture = imageprocessor.grayscale(picture);
            var outputFile = Path.GetFileNameWithoutExtension(args[0]) + "_gray.bmp";
            imageprocessor.save<>(greyscalePicture, outputFile);
            Console.WriteLine(sw.Elapsed);
        }
    }
}