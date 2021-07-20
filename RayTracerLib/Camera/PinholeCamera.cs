using RayTracerLib.Util;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace RayTracerLib.Camera
{
    public class PinholeCamera : AbstractCamera
    {
        /// <summary>
        /// Distance between eye and view plane
        /// Increasing this value decreases the field of view angle
        /// and reduces the amount of the scene that is visible in the
        /// final image
        /// /// </summary>
        public float DistanceToViewPlane = 1f;
        

        public float ZoomFactor = 1f;


        public Vector3 RayDirection(Vector2 point)
        {
            //Vector3 x = point.X * U;
            //Vector3 y = point.Y * V;
            //Vector3 z = DistanceToViewPlane * W;

            Vector3 dir = point.X * U + point.Y * V - DistanceToViewPlane * W;
            return Vector3.Normalize(dir);
        }

        public static Vector3 RayDirection(Vector2 point, Vector3 u, Vector3 v, Vector3 w, float distVP)
        {
            Vector3 dir = point.X * u + point.Y * v - distVP * w;
            return Vector3.Normalize(dir);
        }

        public override void RenderScene(World w)
        {
            RGBColor L = new RGBColor();
            ViewPlane vp = w.ViewPlane;
            Ray ray = new Ray();
            int depth = 0; // Recursion depth
            Vector2 sp = new Vector2(); // sample point in [0, 1] x [0, 1]
            Vector2 pp = new Vector2(); // sample point on a pixel

            vp.PixelSize /= ZoomFactor;
            ray.Origin = Eye;

            
            List<Task> taskList = new List<Task>();

            for(int y = 0; y < vp.VRes; y++) // up
            {
                for(int x = 0; x < vp.HRes; x++) // across
                {
                    if(UseParallelTasks)
                    {
                        int localX = x;
                        int localY = y;

                        Task t = new Task(
                            () =>
                            RenderTask(localX, localY, U, V, W, w, ray, ExposureTime, DistanceToViewPlane)
                        );

                        taskList.Add(t);
                    }
                    else
                    {
                        L = RGBColor.Black;

                        for (int j = 0; j < vp.NumSamples; j++)
                        {
                            sp = vp.Sampler.SampleUnitSquare();
                            float ppx = vp.PixelSize * (x - 0.5f * vp.HRes + sp.X);
                            float ppy = vp.PixelSize * (y - 0.5f * vp.VRes + sp.Y);
                            pp = new Vector2(ppx, ppy);
                            ray.Direction = RayDirection(pp);
                            //L += w.Tracer.TraceRay(ray, depth);
                            L += w.Tracer.TraceRay(ray, depth);
                        }

                        L /= vp.NumSamples;
                        L *= ExposureTime;

                        ////Console.WriteLine("Pixel (x = " + x + ", y = " + y + ")");
                        w.DisplayPixel(x, y, L);
                    }




                }


                //Console.WriteLine("y = " + y);
            }

            if(UseParallelTasks)
            {
                var taskArr = taskList.ToArray();

                foreach (Task t in taskArr)
                {
                    t.Start();
                }

                Task.WaitAll(taskArr);
            }
            

        }

        private static void RenderTask(int x, int y,
            Vector3 u, Vector3 v, Vector3 w,
            World wrld, Ray ray, float exposureTime, float distVP)
        {
            ViewPlane vp = wrld.ViewPlane;

            RGBColor L = RGBColor.Black;
            Vector2 sp = new Vector2(); // sample point in [0, 1] x [0, 1]
            Vector2 pp = new Vector2(); // sample point on a pixel

            for (int j = 0; j < vp.NumSamples; j++)
            {
                sp = vp.Sampler.SampleUnitSquare();
                float ppx = vp.PixelSize * (x - 0.5f * vp.HRes + sp.X);
                float ppy = vp.PixelSize * (y - 0.5f * vp.VRes + sp.Y);
                pp = new Vector2(ppx, ppy);
                ray.Direction = RayDirection(pp, u, v, w, distVP);
                //L += w.Tracer.TraceRay(ray, depth);
                L += wrld.Tracer.TraceRay(ray, 0); //depth);
            }

            L /= vp.NumSamples;
            L *= exposureTime;

            //Console.WriteLine("Pixel (x = " + x + ", y = " + y + ")");

            wrld.DisplayPixel(x, y, L); //w.DisplayPixel(x, y, L);

            //Console.WriteLine("Task done: x = " + x + ", y = " + y);
        }


    }
}
