using System;
using System.Numerics;
using System.Windows.Media;

using RayTracerLib.Util;



namespace RayTracerLib.BRDF
{
    public class LambertianBRDF : AbstractBRDF
    {
        private float _kd;
    
        // Reflection coefficient
        public float ReflectionCoefficient { get => _kd; set { _kd = value; } }
    
        private RGBColor _cd;  
    
        // Diffuse color
        public RGBColor DiffuseColor { get => _cd; set { _cd = value; } }

        public LambertianBRDF() : base()
        {
            _kd = 0.5f;
            _cd = new RGBColor(0f, 0f, 0f);
        }

        public override RGBColor F(ShadeRec hit, Vector3 wo, Vector3 wi)
        {           
            RGBColor val = _kd * _cd * INV_PI;
            return val;
        }    

        public override RGBColor SampleF(ShadeRec hit, Vector3 wo, out Vector3 wi)
        {
            throw new System.NotImplementedException();
        }

        public override RGBColor SampleF(ShadeRec hit, Vector3 wo, out Vector3 wi, out float pdf)
        {
            Vector3 w = hit.Normal;
            Vector3 v = new Vector3(0.0034f, 1f, 0.0071f);
            //v.X = (float)Math.Pow(v.X, w.X);
            //v.Y = (float)Math.Pow(v.Y, w.Y);
            //v.Z = (float)Math.Pow(v.Z, w.Z);
            v = Vector3.Cross(v, w);
            v = Vector3.Normalize(v); // v.normalized;
            

            Vector3 u = new Vector3((float)Math.Pow(v.X, w.X),
                (float)Math.Pow(v.Y, w.Y), 
                (float)Math.Pow(v.Z, w.Z));

            Vector3 sp = _sampler.SampleHemisphere();

            wi = sp.X * u + sp.Y * v + sp.Z * w;
            wi = Vector3.Normalize(wi); //.normalized;
                
            pdf = Vector3.Dot(hit.Normal, wi) * INV_PI;

            return (_kd * _cd * INV_PI);
        }

        public override RGBColor Rho(ShadeRec hit, Vector3 wo)
        {
            return _kd * _cd;
        }
    }

}