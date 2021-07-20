using RayTracerLib.Sampler;
using RayTracerLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RayTracerLib.Camera
{
    public class ThinLensCamera : AbstractCamera
    {
        /// <summary>
        /// Lens radius
        /// </summary>
        public float LensRadius;

        /// <summary>
        /// View plane distance
        /// </summary>
        public float ViewPlaneDistance;

        /// <summary>
        /// Focal plane distance
        /// </summary>
        public float FocalPlaneDistance;

        public float ZoomFactor = 1f;

        private AbstractSampler _sampler;
        public AbstractSampler Sampler
        {
            get => _sampler;
            set
            {
                _sampler = value;
                _sampler.MapSamplesToUnitDisk();
            }
        }

        public Vector3 RayDirection(Vector2 pixelPoint, Vector2 lensPoint)
        {
            float px = pixelPoint.X * FocalPlaneDistance / ViewPlaneDistance;
            float py = pixelPoint.Y * FocalPlaneDistance / ViewPlaneDistance;
            Vector2 p = new Vector2(px, py);

            Vector3 direction =
                (p.X - lensPoint.X) * U +
                (p.Y - lensPoint.Y) * V -
                FocalPlaneDistance * W;
            
            return Vector3.Normalize(direction);
        }
        
        public override void RenderScene(World w)
        {
            RGBColor L = new RGBColor();
            Ray ray = new Ray();
            ViewPlane vp = w.ViewPlane;
            int depth = 0;

            Vector2 sp = new Vector2(); // sample point in [0, 1] x [0, 1]
            Vector2 pp = new Vector2(); // sample point on a pixel
            Vector2 dp = new Vector2(); // sample point on unit disk
            Vector2 lp = new Vector2(); // sample point on lens

            vp.PixelSize /= ZoomFactor;

            for(int r = 0; r < vp.VRes - 1; r++)
            {
                for(int c = 0; c < vp.HRes - 1; c++)
                {
                    L = RGBColor.Black;

                    for(int n = 0; n < vp.NumSamples; n++)
                    {
                        sp = vp.Sampler.SampleUnitSquare();
                        float ppx = vp.PixelSize * (c - vp.HRes / 2f + sp.X);
                        float ppy = vp.PixelSize * (r - vp.VRes / 2f + sp.Y);

                        dp = Sampler.SampleUnitDisk();
                        lp = dp * LensRadius;

                        ray.Origin = Eye + lp.X * U + lp.Y * V;
                        ray.Direction = RayDirection(pp, lp);
                        L += w.Tracer.TraceRay(ray);

                    }

                    L /= vp.NumSamples;
                    L *= ExposureTime;
                    w.DisplayPixel(r, c, L);
                }
            }


        }

    }
}
