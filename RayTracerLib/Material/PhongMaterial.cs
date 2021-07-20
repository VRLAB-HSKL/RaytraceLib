using System;
using System.Numerics;

using RayTracerLib.BRDF;
using RayTracerLib.Util;

namespace RayTracerLib.Material
{
    public class PhongMaterial : AbstractMaterial
    {
        protected LambertianBRDF _ambientBRDF;
        protected LambertianBRDF _diffuseBRDF;
        protected GlossySpecularBRDF _specularBRDF;

        public PhongMaterial()
        {
            _ambientBRDF = new LambertianBRDF();
            _diffuseBRDF = new LambertianBRDF();
            _specularBRDF = new GlossySpecularBRDF();
        }

        public void SetKA(float ka)
        {
            _ambientBRDF.ReflectionCoefficient = ka;
        }

        public void SetKD(float kd)
        {        
            _diffuseBRDF.ReflectionCoefficient = kd;
        }

        public void SetCD(RGBColor cd)
        {
            _ambientBRDF.DiffuseColor = cd;
            _diffuseBRDF.DiffuseColor = cd;
        }

        public void SetKS(float ks)
        {
            _specularBRDF.KS = ks;
        }

        public void SetExp(int exp)
        {
            _specularBRDF.SpecularExponent = exp;
        }

        public override RGBColor Shade(ref ShadeRec hit, int depth)
        {
            Vector3 wo = -hit.SpecRay.Direction; //-_rayDir;
            RGBColor rhoColor = _ambientBRDF.Rho(hit, wo);
            RGBColor ambientColor = hit.WorldRef.AmbientLight.L(hit);
            RGBColor L = rhoColor * ambientColor; //RayTraceUtility.GlobalWorld.GlobalAmbientLight.L(hit);

            var lights = hit.WorldRef.Lights; //RayTraceUtility.GlobalWorld.GlobalLights;
            int numLights = lights.Count;

            for(int i = 0; i < numLights; ++i)
            {
                Vector3 wi = lights[i].GetDirection(hit);

                float ndotwi = Vector3.Dot(hit.Normal, wi);

                if(ndotwi > 0f)
                {
                    bool inShadow = false;

                    if(lights[i].CastShadows)
                    {
                        Ray shadowRay = new Ray(hit.HitPoint, wi);
                        inShadow = lights[i].InShadow(shadowRay, hit);
                    }


                    if (!inShadow)
                    {
                        RGBColor diff = _diffuseBRDF.F(hit, wo, wi);
                        RGBColor spec = _specularBRDF.F(hit, wo, wi);
                        RGBColor lambertianLight = (diff + spec);
                        RGBColor worldLights = lights[i].L(hit) * ndotwi;
                        L += lambertianLight * worldLights;

                        //L += (_diffuseBRDF.F(hit, wo, wi) + _specularBRDF.F(hit, wo, wi)) * lights[i].L(hit) * ndotwi;

                    }                
                }
            }

            return L;
        }

        public override RGBColor AreaLightShade(ref ShadeRec hit)
        {
            Vector3 wo = -hit.SpecRay.Direction; //-_rayDir;
            RGBColor rhoColor = _ambientBRDF.Rho(hit, wo);
            RGBColor ambientColor = hit.WorldRef.AmbientLight.L(hit);
            RGBColor L = rhoColor * ambientColor; //RayTraceUtility.GlobalWorld.GlobalAmbientLight.L(hit);

            var lights = hit.WorldRef.Lights; //RayTraceUtility.GlobalWorld.GlobalLights;
            int numLights = lights.Count;

            for (int i = 0; i < numLights; ++i)
            {
                Vector3 wi = lights[i].GetDirection(hit);

                float ndotwi = Vector3.Dot(hit.Normal, wi);

                if (ndotwi > 0f)
                {
                    bool inShadow = false;

                    if (lights[i].CastShadows && CastShadows) //lights[i].CastShadows)
                    {
                        Ray shadowRay = new Ray(hit.HitPoint, wi);
                        inShadow = lights[i].InShadow(shadowRay, hit);
                    }

                    


                    if (!inShadow)
                    {
                        RGBColor diff = _diffuseBRDF.F(hit, wo, wi);
                        RGBColor lLight = lights[i].L(hit);
                        float gLight = lights[i].G(ref hit);
                        float pdfLight = lights[i].PDF(ref hit);

                        L += diff * lLight * gLight * ndotwi / pdfLight;

                        //RGBColor spec = _specularBRDF.F(hit, wo, wi);
                        //RGBColor lambertianLight = (diff + spec);
                        //RGBColor worldLights = lights[i].L(hit) * ndotwi;
                        //L += lambertianLight * worldLights;

                        //L += (_diffuseBRDF.F(hit, wo, wi) + _specularBRDF.F(hit, wo, wi)) * lights[i].L(hit) * ndotwi;

                    }
                }
            }

            return L;
        }

        public override RGBColor PathShade(ref ShadeRec hit)
        {
            throw new System.NotImplementedException();
        }

    
    }


}