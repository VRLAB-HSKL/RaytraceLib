using System.Windows.Media;
using System.Numerics;

using RayTracerLib.Util;

namespace RayTracerLib.BRDF
{
    public class FresnelReflectorBRDF : AbstractBRDF
    {
        public float EtaIn { get; set; }
        public float EtaOut { get; set; }

        private float _kr; // reflection coefficient
        public float ReflectionCoefficient { get => _kr; set { _kr = value; } }
        public RGBColor ReflectionColor { get; set; } = RGBColor.White; // the reflection color

        public float Fresnel(ShadeRec hit)//, Vector3 direction)
        {
            Vector3 normal = hit.Normal;
            float ndotd = Vector3.Dot(-normal, hit.SpecRay.Direction); //direction);
            float eta;

            if(ndotd < 0f)
            {
                normal = -normal;
                eta = EtaOut / EtaIn;
            }
            else
            {
                eta = EtaIn / EtaOut;
            }

            float cos_theta_i = Vector3.Dot(-normal, hit.SpecRay.Direction); //direction);
            float temp = 1f - (1f - cos_theta_i * cos_theta_i) / (eta * eta);

            float cos_theta_t = (float)System.Math.Sqrt(1f - (1f - cos_theta_i * cos_theta_i) / (eta * eta));
            float r_parallel = (eta * cos_theta_i - cos_theta_t) / (eta * cos_theta_i + cos_theta_t);
            float r_perpendicular = (cos_theta_i - eta * cos_theta_t) / (cos_theta_i + eta * cos_theta_t);
            float kr = 0.5f * (r_parallel * r_parallel + r_perpendicular * r_perpendicular);

            return kr;
        }

        public override RGBColor F(ShadeRec hit, Vector3 wo, Vector3 wi)
        {
            throw new System.NotImplementedException();
        }

        public override RGBColor Rho(ShadeRec hit, Vector3 wo)
        {
            throw new System.NotImplementedException();
        }

        public override RGBColor SampleF(ShadeRec hit, Vector3 wo, out Vector3 wi)
        {
            // ToDo: Make sure this dot product is correct
            float ndotwo = Vector3.Dot(hit.Normal, wo);
            wi = -wo + 2f * hit.Normal * ndotwo;
            // ToDo: Make sure this dot product is correct

            
            //Vector3 calcVector = new Vector3(ReflectionColor);
            //calcVector.X = (float)ReflectionColor.R / 255f;
            //calcVector.Y = (float)ReflectionColor.G / 255f;
            //calcVector.Z = (float)ReflectionColor.B / 255f;


            RGBColor vd = (_kr * ReflectionColor / (float)System.Math.Abs(Vector3.Dot(hit.Normal, wi)));
            //RGBColor retCol = new RGBColor();
            //retCol.R = (vd.X * 255f);
            //retCol.G = (vd.Y * 255f);
            //retCol.B = (vd.Z * 255f);
            return vd;

            // why is this fabs? 
            // kr would be a Fresnel term in a Fresnel reflector
            // for transparency when ray hits inside surface?, if so it should go in Chapter 24
        }

        public override RGBColor SampleF(ShadeRec hit, Vector3 wo, out Vector3 wi, out float pdf)
        {
            throw new System.NotImplementedException();
        }
    }


}