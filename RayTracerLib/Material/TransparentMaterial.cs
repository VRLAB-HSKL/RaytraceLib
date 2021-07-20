using System;
using System.Numerics;

using RayTracerLib.Util;
using RayTracerLib.BRDF;
using RayTracerLib.BTDF;


namespace RayTracerLib.Material
{
    public class TransparentMaterial : PhongMaterial
    {
        private PerfectSpecularBRDF _reflectiveBRDF;
        private PerfectTransmitterBTDF _specularBTDF;

        public TransparentMaterial(
            float ks, float exp, float ior, float kr, float kt)
            : base()
        {
            _reflectiveBRDF = new PerfectSpecularBRDF();
            _specularBTDF = new PerfectTransmitterBTDF();

            _specularBRDF.KS = ks;
            _specularBRDF.SetSpecularExponent(exp);
        
            _specularBTDF.IOR = ior;

            _reflectiveBRDF.ReflectionCoefficient = kr;
            _specularBTDF.KT = kt;

        }



        //public void SetReflectionCoefficient(float ks)
        //{
        //    _reflectiveBRDF.ReflectionCoefficient = ks;
        //}

        //public void SetIOR(float ior)
        //{
        //    _specularBTDF.IOR = ior;

        //}

        public override RGBColor Shade(ref ShadeRec hit,int depth)
        {
            RGBColor L = base.Shade(ref hit, depth);

            Vector3 wo = -hit.SpecRay.Direction; // -_rayDir;
            Vector3 wi = Vector3.Zero;
            RGBColor fr = _reflectiveBRDF.SampleF(hit, wo, out wi);
            Ray reflectedRay = new Ray(hit.HitPoint, wi);

            if(_specularBTDF.Tir(hit))
            {
                L += hit.WorldRef.Tracer.TraceRay(reflectedRay, depth + 1);
            }
            else
            {
                Vector3 wt = Vector3.Zero;
                // Compute wt
                RGBColor ft = _specularBTDF.SampleF(hit, wo, out wt);

                Ray transmittedRay = new Ray(hit.HitPoint, wt);

                L += fr * hit.WorldRef.Tracer.TraceRay(reflectedRay, depth + 1) 
                    * (float)Math.Abs(Vector3.Dot(hit.Normal, wi));

                L += ft * hit.WorldRef.Tracer.TraceRay(transmittedRay, depth + 1)
                    * (float)Math.Abs(Vector3.Dot(hit.Normal, wt));
            }

            return L;
        }

        public override RGBColor AreaLightShade(ref ShadeRec hit)
        {
            return Shade(ref hit, 0);
        }

    }



}