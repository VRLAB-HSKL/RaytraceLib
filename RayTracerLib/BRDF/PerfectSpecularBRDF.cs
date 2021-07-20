using System.Numerics;
using RayTracerLib.Util;

namespace RayTracerLib.BRDF
{
    public class PerfectSpecularBRDF : AbstractBRDF
    {
        private float _kr; // reflection coefficient
    
        public float ReflectionCoefficient { get => _kr; set { _kr = value; } }
    
        public RGBColor ReflectionColor { get; set; } // the reflection color
    
        public PerfectSpecularBRDF() : base()
        {
            _kr = 0f;
            ReflectionColor = new RGBColor(1f, 1f, 1f);
        }

        public override RGBColor F(ShadeRec hit, Vector3 wi, Vector3 wo)
        {
            return RGBColor.Black;
        }

        public override RGBColor Rho(ShadeRec hit, Vector3 wo)
        {
            return RGBColor.Black;
        }

        public override RGBColor SampleF(ShadeRec hit, Vector3 wo, out Vector3 wi)
        {
            // ToDo: Make sure this dot product is correct
            float ndotwo = Vector3.Dot(hit.Normal, wo);
            wi = -wo + 2f * hit.Normal * ndotwo;

            RGBColor retColor = ReflectionCoefficient * ReflectionColor / (Vector3.Dot(hit.Normal, wi));
            return retColor;
            
            
            //return (_kr * ReflectionColor / (float)System.Math.Abs(Vector3.Dot(hit.Normal,wi)));
            // why is this fabs? 
            // kr would be a Fresnel term in a Fresnel reflector
            // for transparency when ray hits inside surface?, if so it should go in Chapter 24
        }

        public override RGBColor SampleF(ShadeRec hit, Vector3 wo, out Vector3 wi, out float pdf)
        {
            // ToDo: Make sure this dot product is correct
            float ndotwo = Vector3.Dot(hit.Normal, wo);
            wi = -wo + 2f * hit.Normal * ndotwo;
            // ToDo: Make sure this dot product is correct
            pdf = (float)System.Math.Abs(Vector3.Dot(hit.Normal, wi));
            return (_kr * ReflectionColor);
        }
    }

}