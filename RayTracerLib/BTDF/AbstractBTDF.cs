using System.Numerics;

using RayTracerLib.Sampler;
using RayTracerLib.Util;


namespace RayTracerLib.BTDF
{

    public abstract class AbstractBTDF
    {
        protected readonly static float INV_PI = 0.3183098861837906715F;

        protected static AbstractSampler _sampler = 
            new MultiJitteredSampler(83, 50, RayTraceUtility.H_STEP, RayTraceUtility.V_STEP);

        public void SetSampler(AbstractSampler sampler)
        {
            _sampler = sampler;
        }

        public abstract RGBColor F(ShadeRec hit, Vector3 wo, Vector3 wi);
        public abstract RGBColor SampleF(ShadeRec hit, Vector3 wo, out Vector3 wt);
        public abstract RGBColor SampleF(ShadeRec hit, Vector3 wo, Vector3 wi, out float pdf);
        public abstract RGBColor Rho(ShadeRec hit, Vector3 wo);

        public abstract bool Tir(ShadeRec hit);
    }

}