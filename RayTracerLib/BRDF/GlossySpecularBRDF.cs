using System.Windows.Media;
using System.Numerics;
using System;

using RayTracerLib.Sampler;
using RayTracerLib.Util;

namespace RayTracerLib.BRDF
{
    public class GlossySpecularBRDF : AbstractBRDF
    {
        private float _ks;
        public float KS { get => _ks; set { _ks = value; } }

        private RGBColor _cs;  // specular color
        private float _exp; // specular exponent

        public float SpecularExponent { get => _exp; set { _exp = value; } }

        public GlossySpecularBRDF() : base()
        {
            _ks = 0f;
            _cs = new RGBColor(1f, 1f, 1f);

            _exp = 1f;
        }

        public void SetSampler(AbstractSampler sampler, float exp)
        {
            _sampler = sampler;
            _sampler.MapSamplesToHemisphere(exp);
        }

        public void SetSpecularExponent(float exp)
        {
            _exp = exp;
        }

        public void SetSamples(int numSamples, int numSteps, float hStep, float vStep, float exp)
        {
            _sampler = new MultiJitteredSampler(numSamples, numSteps, hStep, vStep);
            _sampler.MapSamplesToHemisphere(exp);
        }

        public override RGBColor F(ShadeRec hit, Vector3 wi, Vector3 wo)
        {
            var normal = hit.Normal; //Vector3.Normalize(hit.Normal); 
            RGBColor L = new RGBColor();
            float ndotwi = Vector3.Dot(normal, wi);

            // ToDo: Refactor this !
            var val1 = 2f * normal * ndotwi;

            Vector3 r = (-wi + val1); // new Vector3();
            //r.X = -wi.X + 2f * hit.Normal.X * ndotwi;
            //r.Y = -wi.Y + 2f * hit.Normal.Y * ndotwi;
            //r.Z = -wi.Z + 2f * hit.Normal.Z * ndotwi;

            float rdotwo = Vector3.Dot(r, wo);

            if (rdotwo > 0f)
            {
                float x = (float)Math.Pow(rdotwo, _exp);
                L = _ks * _cs * x;
            }            

            return L;
        }

        public override RGBColor Rho(ShadeRec hit, Vector3 wo)
        {
            return new RGBColor(0f, 0f, 0f);
        }

        public override RGBColor SampleF(ShadeRec hit, Vector3 wo, out Vector3 wi)
        {
            throw new NotImplementedException();
        }

        public override RGBColor SampleF(ShadeRec hit, Vector3 wo, out Vector3 wi, out float pdf)
        {
            float ndotwo = Vector3.Dot(hit.Normal, wo);

            Vector3 r = -wo + 2f * hit.Normal * ndotwo;     // direction of mirror reflection
            Vector3 w = r;
            Vector3 u = new Vector3(0.00424f, 1f, 0.00764f);
            //u.X = (float)System.Math.Pow(u.X, w.X);
            //u.Y = (float)System.Math.Pow(u.Y, w.Y);
            //u.Z = (float)System.Math.Pow(u.Z, w.Z);
            u = Vector3.Cross(u, w);
            u = Vector3.Normalize(u);


            //Vector3 v = 
            //    new Vector3(
            //        (float)System.Math.Pow(u.X, w.X), 
            //        (float)Math.Pow(u.Y, w.Y), 
            //        (float)Math.Pow(u.Z, w.Z)); // u ^ w;
            Vector3 v = Vector3.Cross(u, w);

            Vector3 sp = _sampler.SampleHemisphere();
            wi = sp.X * u + sp.Y * v + sp.Z * w;            // reflected ray direction

            // ToDo: Make sure this dot product is correct
            if (Vector3.Dot(hit.Normal, wi) < 0f)                       // reflected ray is below tangent plane
                wi = -sp.X * u - sp.Y * v + sp.Z * w;

            // ToDo: Make sure this dot product is correct
            float phong_lobe = (float)Math.Pow(Vector3.Dot(r,wi), _exp);
            pdf = phong_lobe * (Vector3.Dot(hit.Normal, wi));

            
            return (_ks * _cs * phong_lobe);
        }
    }


}