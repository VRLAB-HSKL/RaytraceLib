using System;
using System.Numerics;

using RayTracerLib.BRDF;
using RayTracerLib.BTDF;
using RayTracerLib.Util;

namespace RayTracerLib.Material
{
    public class DielectricMaterial : PhongMaterial
    {
        // Interior filter color
        public RGBColor CfIn { get; set; }

        // Exterior filter color
        public RGBColor CfOut { get; set; }

        FresnelReflectorBRDF _fresnelBRDF;
        FresnelTransmitterBTDF _fresnelBTDF;

        public DielectricMaterial(RGBColor cfIn, RGBColor cfOut)
            : base()
        {
            CfIn = cfIn;
            CfOut = cfOut;

            _fresnelBRDF = new FresnelReflectorBRDF();
            _fresnelBTDF = new FresnelTransmitterBTDF();//raydir);
            
        }


        //public void SetExp(float exp)
        //{
        //    base.SetExp(exp);

            

        //    return;
        //}

        public void SetEtaIn(float etaIn)
        {
            _fresnelBRDF.EtaIn = etaIn;

            // ToDo: Make sure this is correct
            //_fresnelBTDF.IOR = etaIn;
        }

        public void SetEtaOut(float etaOut)
        {
            _fresnelBRDF.EtaOut = etaOut;
        }

        public void SetIOR(float ior)
        {
            _fresnelBTDF.IOR = ior;
            
        }

        public void SetKT(float kt)
        {
            _fresnelBTDF.KT = kt;
        }




        public override RGBColor AreaLightShade(ref ShadeRec hit)
        {
            return Shade(ref hit, 0);
        }

        public override RGBColor PathShade(ref ShadeRec hit)
        {
            throw new System.NotImplementedException();
        }

        public override RGBColor Shade(ref ShadeRec hit, int depth)
        {
            RGBColor L = base.Shade(ref hit, depth);

            Vector3 wi; // = Vector3.zero;
            Vector3 wo = -hit.SpecRay.Direction; // _rayDir;

            // Compute wi
            RGBColor fr = _fresnelBRDF.SampleF(hit, wo, out wi);

            Ray reflectedRay = new Ray(hit.HitPoint, wi);
            float t = 0f;
            RGBColor Lr = new RGBColor();
            RGBColor Lt = new RGBColor();
            float ndotwi = Vector3.Dot(hit.Normal, wi);

            // Total inner reflection
            if(_fresnelBTDF.Tir(hit))
            {
                if(ndotwi < 0f)
                {
                    // Reflected ray is inside
                    Lr = hit.WorldRef.Tracer.TraceRay(reflectedRay, t, depth + 1);

                    // Inside filter color
                    //L.R += (float)Math.Pow(CfIn.R, t) * Lr.R;
                    //L.G += (float)Math.Pow(CfIn.G, t) * Lr.G;
                    //L.B += (float)Math.Pow(CfIn.B, t) * Lr.B;
                    L += CfIn.PowC(t) * Lr;

                }
                else
                {
                    // Reflected ray is outside
                    Lr = hit.WorldRef.Tracer.TraceRay(reflectedRay, t, depth + 1);

                    // kr = 1
                    // Outside filter color

                    //L.R += (float)Math.Pow(CfOut.R, t) * Lr.R;
                    //L.G += (float)Math.Pow(CfOut.G, t) * Lr.G;
                    //L.B += (float)Math.Pow(CfOut.B, t) * Lr.B;
                    L += CfOut.PowC(t) * Lr;

                }
            }
            else // No total internal reflection
            {
                Vector3 wt = Vector3.Zero;
            
                // Compute wt
                RGBColor ft = _fresnelBTDF.SampleF(hit, wo, out wt);
                Ray transmittedRay = new Ray(hit.HitPoint, wt);
                float ndotwt = Vector3.Dot(hit.Normal, wt);

                if(ndotwi < 0f)
                {
                    // Reflected ray is inside
                    Lr = fr * hit.WorldRef.Tracer.TraceRay(reflectedRay, t, depth + 1) * Math.Abs(ndotwi);

                    // Inside filter color
                    L += CfIn.PowC(t) * Lr; 

                    
                    //L.R += (float)Math.Pow(CfIn.R, t) * Lr.R;
                    //L.G += (float)Math.Pow(CfIn.G, t) * Lr.G;
                    //L.B += (float)Math.Pow(CfIn.B, t) * Lr.B;

                    // Transmitted ray is outside
                    Lt = ft * hit.WorldRef.Tracer.TraceRay(transmittedRay, t, depth + 1) * Math.Abs(ndotwt);

                    // Outside filter color
                    L += CfOut.PowC(t) * Lt;

                    //L.R += (float)Math.Pow(CfOut.R, t) * Lt.R;
                    //L.G += (float)Math.Pow(CfOut.G, t) * Lt.G;
                    //L.B += (float)Math.Pow(CfOut.B, t) * Lt.B;
                }
                else 
                {
                    // Reflected ray is outside

                    Lr = fr * hit.WorldRef.Tracer.TraceRay(reflectedRay, t, depth + 1) * Math.Abs(ndotwi);

                    // Outside filter color
                    L += CfOut.PowC(t) * Lr;

                    //L.R += (float)Math.Pow(CfOut.R, t) * Lr.R;
                    //L.G += (float)Math.Pow(CfOut.G, t) * Lr.G;
                    //L.B += (float)Math.Pow(CfOut.B, t) * Lr.B;

                    // Transmitted ray is inside
                    Lt = ft * hit.WorldRef.Tracer.TraceRay(transmittedRay, t, depth + 1) * Math.Abs(ndotwt);

                    // Inside filter color
                    L += CfIn.PowC(t) * Lt;

                    //L.R += (float)Math.Pow(CfIn.R, t) * Lt.R;
                    //L.G += (float)Math.Pow(CfIn.G, t) * Lt.G;
                    //L.B += (float)Math.Pow(CfIn.B, t) * Lt.B;
                }
            }

            return L;
        }
    }

}