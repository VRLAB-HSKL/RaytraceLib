using RayTracerLib.BRDF;
using RayTracerLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RayTracerLib.Material
{
    public class GlossyReflectorMaterial : PhongMaterial
    {
        public GlossySpecularBRDF _glossySpecularBRDF;

        public GlossyReflectorMaterial() : base()
        {
            _glossySpecularBRDF = new GlossySpecularBRDF();
        }

        public void SetSamples(int numSamples, float exp)
        {
            _glossySpecularBRDF.SetSamples(numSamples, 50, 1f, 1f, exp);
        }

        public void SetKR(float k)
        {
            _glossySpecularBRDF.KS = k;
        }

        public void SetExponent(float exp)
        {
            _glossySpecularBRDF.SetSpecularExponent(exp);
        }

        public override RGBColor AreaLightShade(ref ShadeRec hit)
        {
            RGBColor L = base.AreaLightShade(ref hit); // direct illumination
            Vector3 wo = -hit.SpecRay.Direction;
            Vector3 wi = Vector3.Zero;
            float pdf = 0f;

            RGBColor fr = _glossySpecularBRDF.SampleF(hit, wo, out wi, out pdf);
            Ray reflectedRay = new Ray(hit.HitPoint, wi);

            L += fr * hit.WorldRef.Tracer.TraceRay(reflectedRay, hit.Depth + 1) *
                (Vector3.Dot(hit.Normal, wi)) / pdf;

            return L;
        }

    }
}
