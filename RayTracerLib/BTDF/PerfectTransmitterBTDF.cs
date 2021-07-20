using System.Numerics;

using RayTracerLib.Util;


namespace RayTracerLib.BTDF
{
    public class PerfectTransmitterBTDF : AbstractBTDF
    {
        private float _kt;

        public float KT { get => _kt; set { _kt = value; } }

        private float _ior;

        public float IOR { get => _ior; set { _ior = value; } }

        //private Vector3 _rayDir;

        public PerfectTransmitterBTDF()
        {
            //_rayDir = rayDir;
        }

        public override RGBColor F(ShadeRec hit, Vector3 wo, Vector3 wi)
        {
            throw new System.NotImplementedException();
        }

        public override RGBColor Rho(ShadeRec hit, Vector3 wo)
        {
            throw new System.NotImplementedException();
        }

        public override RGBColor SampleF(ShadeRec hit, Vector3 wo, out Vector3 wt)
        {
            Normal n = hit.Normal;
            float cos_theta_i = Vector3.Dot(n, wo);
            float eta = _ior;

            if(cos_theta_i < 0f)
            {
                cos_theta_i = -cos_theta_i;
                n = -n;
                eta = (float) 1f / eta;
            }

            float temp = 1f - (1f - cos_theta_i * cos_theta_i) / (eta * eta);
            float cos_theta_2 = (float)System.Math.Sqrt(temp);
            wt = -wo / eta - (cos_theta_2 - cos_theta_i / eta) * n;

            return (_kt / (eta * eta) * RGBColor.White / (float)System.Math.Abs(Vector3.Dot(hit.Normal, wt)));
        }

        public override RGBColor SampleF(ShadeRec hit, Vector3 wo, Vector3 wi, out float pdf)
        {
            throw new System.NotImplementedException();
        }

        public override bool Tir(ShadeRec hit)
        {
            Vector3 wo = -hit.SpecRay.Direction; // ToDo: Check if this is correct; //-_rayDir;
            float cos_theta_i = Vector3.Dot(hit.Normal, wo);
            float eta = _ior;

            if(cos_theta_i < 0f)
            {
                eta = (float) (1f / eta);
            }

            return (1f - (1f - cos_theta_i * cos_theta_i) / (eta * eta)) < 0f;
        }
    }

}