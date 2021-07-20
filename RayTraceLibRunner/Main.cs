using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;

using RayTracerLib.Util;
using System.Diagnostics;

namespace RayTracerLib
{
    public class RTRunner
    {
        public static void Main(string[] args)
        {
            //Console.WriteLine("test123");
            //var x = Console.ReadLine();
            TestFoo();
        }
    
        public static void TestFoo()
        {
            Stopwatch sw = new Stopwatch();

            World w = new World();
            //w.Build();

            //sw.Start();
            //w.RenderScene();
            //sw.Stop();

            //long singleThreadTime = sw.ElapsedMilliseconds;

            //SaveFile(
            //    @"C:\Users\user\Desktop\rtLibOutput\singleThread_" + 
            //    singleThreadTime.ToString("G") + "ms.png",                 
            //    System.Drawing.Imaging.ImageFormat.Png 
            //    , World.PixelPuffer);

            // Console.WriteLine("Single thread time: " + singleThreadTime + " ms");

            w = new World();
            w.Build();
            w.Camera.UseParallelTasks = false;

            sw.Restart();
            w.RenderScene();
            sw.Stop();

            long taskTime = sw.ElapsedMilliseconds;

            SaveFile(
                @"C:\Users\user\Desktop\rtLibOutput\taskBased_" + 
                taskTime.ToString("G") + "ms.png",                
                System.Drawing.Imaging.ImageFormat.Png
                , World.PixelPuffer);

            
            //Console.WriteLine("Task time: " + taskTime + " ms");
            //Console.ReadLine();
        }


        public static void SavePPM(string filePath, RGBColor[][] colorArray)
        {
            bool log = false;

            if (colorArray[0] is null) return;

            if(log) Console.WriteLine("Saving PPM...");

            using (TextWriter txtW = new StreamWriter(filePath, false))
            {
                txtW.WriteLine("P3"); // Portable Pixmap ASCII Identifier

                if(log) Console.WriteLine("Dimensions: " + colorArray.Length + " x " + colorArray[0].Length);
                txtW.WriteLine(colorArray.Length + " " + colorArray[0].Length);
                txtW.WriteLine(65535);
                if(log) Console.ReadLine();

                for(int i = 0; i < colorArray.Length; i++)
                {
                    for(int j = 0; j < colorArray[0].Length; j++)
                    {
                        RGBColor finalColor = colorArray[i][j];
                        finalColor.R = (float)Math.Round(colorArray[i][j].R * 65535);
                        finalColor.G = (float)Math.Round(colorArray[i][j].G * 65535);
                        finalColor.B = (float)Math.Round(colorArray[i][j].B * 65535);
                    }
                }


                for (int i = colorArray.Length - 1; i >= 0 ; i--)
                {                    
                    List<RGBColor> valArray = new List<RGBColor>(colorArray[i]);
                    string line = string.Join(" ", valArray);                    

                    if(log)
                    {
                        for (int j = 0; j < valArray.Count; j++)
                        {
                            Console.WriteLine("[" + i + ", " + j + "]: " + valArray[j]);

                        }
                    }                    

                    txtW.WriteLine(line);
                }

                if(log)
                {
                    Console.WriteLine("Saved .ppm file in " + filePath);
                    Console.WriteLine("Press enter to exit program");
                    Console.ReadLine();
                }
            }
        }

        public static void SaveFile(
            string filePath, System.Drawing.Imaging.ImageFormat format, 
            RGBColor[][] colorArray) //System.Drawing.Bitmap bmap)
        {
            Bitmap bm = new Bitmap(colorArray[0].Length, colorArray.Length);
            int bmXCoord = 0, bmYCoord = 0;

            for(int y = colorArray.Length - 1; y >= 0; y--)
            {
                for(int x = 0; x < colorArray[0].Length; x++)
                {
                    RGBColor col = colorArray[y][x];
                    Color sdCol = Color.FromArgb(
                        (byte)(col.R * 255f),
                        (byte)(col.G * 255f),
                        (byte)(col.B * 255f)
                        );
                    bm.SetPixel(bmXCoord++, bmYCoord, sdCol);
                }

                ++bmYCoord;
                bmXCoord = 0;
            }

            bm.Save(filePath, format);
        }

    }    
}
