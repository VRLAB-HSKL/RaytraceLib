using RayTracerLib.Material;
using RayTracerLib.Sampler;
using RayTracerLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RayTracerLib.Light
{
    public class EnvironmentalLight : AbstractLight
    {
        AbstractSampler Sampler;
        AbstractMaterial Material;

        Vector3 U = Vector3.Zero;
        Vector3 V = Vector3.Zero;
        Vector3 W = Vector3.Zero;

        Vector3 Wi = Vector3.Zero;

        public EnvironmentalLight(AbstractSampler sampler, AbstractMaterial mat)
        {
            Sampler = sampler;
            Material = mat;

            Sampler.MapSamplesToHemisphere(1f);
        }

        public override Vector3 GetDirection(ShadeRec hit)
        {
            W = hit.Normal;
            V = Vector3.Cross(new Vector3(0.0034f, 1f, 0.0071f), W);
            V = Vector3.Normalize(V);

            U = Vector3.Cross(V, W);
            Vector3 sp = Sampler.SampleHemisphere();

            Wi = sp.X * U + sp.Y * V + sp.Z * W;

            return Wi;
        }

        public override bool InShadow(Ray ray, ShadeRec hit)
        {
            double t = 0f;
            int numObjects = hit.WorldRef.GeometricObjects.Count;

            for (int j = 0; j < numObjects; j++)
            {
                if (hit.WorldRef.GeometricObjects[j].ShadowHit(ray, ref t))
                    return true;
            }

            return false;
        }

        public override RGBColor L(ShadeRec hit)
        {
            return Material.Le(ref hit);
        }

        public override float PDF(ref ShadeRec hit)
        {
            return base.PDF(ref hit);
        }
    }
}
