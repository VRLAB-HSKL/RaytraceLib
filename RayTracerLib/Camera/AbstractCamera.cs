using RayTracerLib.Util;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace RayTracerLib.Camera
{


    public abstract class AbstractCamera
    {
        public bool UseParallelTasks = false;


        public Vector3 Eye;
        public Vector3 LookAt;
        public Vector3 Up = Vector3.UnitY;

        public Vector3 U;
        public Vector3 V;
        public Vector3 W;

        public float ExposureTime = 1f;
        
        public void ComputeUVW()
        {
            // Check if camera is looking exactly down
            if(Eye.X == LookAt.X && LookAt.Y < Eye.Y && Eye.Z == LookAt.Z)
            {
                U = new Vector3(0f, 0f, 1f);
                V = new Vector3(1f, 0f, 0f);
                W = new Vector3(0f, 1f, 0f);
                return;
            }

            W = Eye - LookAt;
            W = Vector3.Normalize(W);

            //float ux = (float)Math.Pow(Up.X, W.X);
            //double tmp = Math.Pow(Up.Y, W.Y);
            //float uy = (float)tmp;
            //float uz = (float)Math.Pow(Up.Z, W.Z);

            U = Vector3.Cross(Up, W); //new Vector3(ux, uy, uz);
            U = Vector3.Normalize(U);

            //float vx = (float)Math.Pow(W.X, U.X);
            //float vy = (float)Math.Pow(W.Y, U.Y);
            //float vz = (float)Math.Pow(W.Z, U.Z);

            V = Vector3.Cross(W, U); //new Vector3(vx, vy, vz);
            
        }

        public abstract void RenderScene(World w);
    }
}
