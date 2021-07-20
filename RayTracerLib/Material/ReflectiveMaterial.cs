using System;
using System.Numerics;

using RayTracerLib.Util;
using RayTracerLib.BRDF;

namespace RayTracerLib.Material
{
    public class ReflectiveMaterial : PhongMaterial
    {
        PerfectSpecularBRDF _reflectiveBRDF;

        public ReflectiveMaterial()
            : base()
        {
            _reflectiveBRDF = new PerfectSpecularBRDF();
        }

        public void SetKR(float kr)
        {
            _reflectiveBRDF.ReflectionCoefficient = kr;
        }

        public void SetCR(RGBColor cr)
        {
            _reflectiveBRDF.ReflectionColor = cr;
        }

        public override RGBColor Shade(ref ShadeRec hit, int depth)
        {
            //Debug.Log("ReflectShade - Depth: " + depth);

            // Calculate direct illumination
            RGBColor L = base.Shade(ref hit, depth);

            // Calculate reflection based color parts
            Vector3 wo = -hit.SpecRay.Direction;
            Vector3 wi = new Vector3();
            RGBColor fr = _reflectiveBRDF.SampleF(hit, wo, out wi);
            Ray reflected_ray = new Ray(hit.HitPoint, wi);

            //reflected_ray.Depth = hit.Depth + 1;

            var reflecColor = hit.WorldRef.Tracer.TraceRay(reflected_ray, depth + 1);
            var dotP = Vector3.Dot(hit.Normal, wi);

            //if(depth == 0)
            //{
            //    int ejdej = 0;
            //}

            L += fr * reflecColor
                    * dotP;

            return L;
        }
    }


}