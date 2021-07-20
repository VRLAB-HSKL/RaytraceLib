
using System;
using System.Numerics;

using RayTracerLib.Util;
using RayTracerLib.BRDF;

namespace RayTracerLib.Material
{
    public class MatteMaterial : AbstractMaterial
    {
        private LambertianBRDF _ambientBRDF;
        private LambertianBRDF _diffuseBRDF;

        //private Vector3 _rayDir;  

        public MatteMaterial(Vector3 rayDir)
        {
            _ambientBRDF = new LambertianBRDF();
            _diffuseBRDF = new LambertianBRDF();

            //_rayDir = rayDir;
        }

        /// <summary>
        /// Set ambient reflection coefficient
        /// </summary>
        /// <param name="ka">Ambient reflection coefficient</param>
        public void SetKA(float ka)
        {
            _ambientBRDF.ReflectionCoefficient = ka;
        }

        /// <summary>
        /// Set diffuse reflection coefficient
        /// </summary>
        /// <param name="kd"></param>
        public void SetKD(float kd)
        {
            _diffuseBRDF.ReflectionCoefficient = kd;
        }

        public void SetCD(RGBColor cd)
        {
            _ambientBRDF.DiffuseColor = cd;
            _diffuseBRDF.DiffuseColor = cd;
        }

        public override RGBColor Shade(ref ShadeRec hit, int depth)
        {
            Vector3 wout = -hit.SpecRay.Direction; // -_rayDir;

            // Initialise color with ambient light 
            RGBColor ambientRho = _ambientBRDF.Rho(hit, wout);
            //Debug.Log("MatteMaterial - AmbientRho: " + ambientRho);
            RGBColor worldAmbientL = hit.WorldRef.AmbientLight.L(hit); //RayTraceUtility.GlobalWorld.GlobalAmbientLight.L(hit);
            //Debug.Log("MatteMaterial - WorldAmbientL: " + worldAmbientL);
            RGBColor L = ambientRho * worldAmbientL;
            //Debug.Log("MatteMaterial - AfterAmbientBRDF - RGBColor: " + L);

            var lights = hit.WorldRef.Lights; //RayTraceUtility.GlobalWorld.GlobalLights;

            // Iterate over global light sources and add diffuse radiance to color
            for(int i = 0; i < lights.Count; ++i)
            {
                // Get input ray direction
                Vector3 wi = lights[i].GetDirection(hit);
            
                // ToDo: Make sure this dot product is correct
                float ndotwi = Vector3.Dot(hit.Normal, wi);

                if(ndotwi > 0f)
                {
                    L += _diffuseBRDF.F(hit, wout, wi) * lights[i].L(hit) * ndotwi;
                }
            }
            //Debug.Log("MatteMaterial - AfterDiffuseBRDF - RGBColor: " + L);

            return L;
        }

        public override RGBColor AreaLightShade(ref ShadeRec hit)
        {
            Vector3 wo = -hit.SpecRay.Direction;
            RGBColor L = _ambientBRDF.Rho(hit, wo) * hit.WorldRef.AmbientLight.L(hit);
            int numLights = hit.WorldRef.Lights.Count;

            for(int j = 0; j < numLights; j++)
            {
                Vector3 wi = hit.WorldRef.Lights[j].GetDirection(hit);
                float ndotwi = Vector3.Dot(hit.Normal, wi);

                if(ndotwi > 0f)
                {
                    bool inShadow = false;

                    if(hit.WorldRef.Lights[j].CastShadows)
                    {
                        Ray shadowRay = new Ray(hit.HitPoint, wi);
                        inShadow = hit.WorldRef.Lights[j].InShadow(shadowRay, hit);
                    }

                    if(!inShadow)
                    {
                        //L += _diffuseBRDF.F(hit, wo, wi) *
                        //    hit.WorldRef.Lights[j].L(hit) *
                        //    hit.WorldRef.Lights[j].G *
                        //    ndotwi / hit.WorldRef.Lights[j].Pdf(hit);
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