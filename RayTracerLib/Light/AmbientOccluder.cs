using System.Numerics;

using RayTracerLib.Sampler;
using RayTracerLib.Util;

namespace RayTracerLib.Light
{
    public class AmbientOccluder : AbstractLight
    {
        private Vector3 _u = Vector3.Zero;
        private Vector3 _v = Vector3.Zero;
        private Vector3 _w = Vector3.Zero;

        private float _ls;
        public float RadianceFactor { get => _ls; set { _ls = value; } }
        public RGBColor LightColor { get; set; }

        private AbstractSampler _sampler;
        private RGBColor _minAmount = RGBColor.Black;

        public AmbientOccluder(AbstractSampler sampler, RGBColor minAmount)
        {
            _sampler = sampler;
            _minAmount = minAmount;

            _sampler.MapSamplesToHemisphere(1f);

            _ls = 1f;
            LightColor = RGBColor.White;
        }

        public AmbientOccluder(float ls, RGBColor color, AbstractSampler sampler, RGBColor minAmount)
        {
            _ls = ls;
            LightColor = color;
            _sampler = sampler;
            _minAmount = minAmount;

            _sampler.MapSamplesToHemisphere(1f);
        }


        public override Vector3 GetDirection(ShadeRec hit)
        {
            Vector3 sp = _sampler.SampleHemisphere();
            Vector3 retVal = sp.X * _u + sp.Y * _v + sp.Z * _w;

            if(float.IsNaN(retVal.X))
            {
                int avb = 0;
            }

            return retVal;
        }

        public override bool InShadow(Ray ray, ShadeRec hit)
        {
            double t = 0f;
            int numObjects = hit.WorldRef.GeometricObjects.Count;

            for(int j = 0; j < numObjects; j++)
            {
                if (hit.WorldRef.GeometricObjects[j].ShadowHit(ray, ref t))
                    return true;
            }

            return false;

        }

        public override RGBColor L(ShadeRec hit)
        {
            _w = hit.Normal;

            //Vector3 wCrossUp = Vector3.Cross(_w, Vector3.UnitY);
            //_v = Vector3.Cross(_w, new Vector3(0.0072f, 1f, 0.0034f));  //wCrossUp / (wCrossUp.Length()); // Vector3.magnite(wCrossUp)
            //_v = Vector3.Normalize(_v);

            //_u = Vector3.Cross(_v, _w);

            // Jitter up vector in case normal is vertical
            //_v.X = (float)System.Math.Pow(_w.X, 0.0072f);
            //_v.Y = (float)System.Math.Pow(_w.Y, 1f);
            //_v.Z = (float)System.Math.Pow(_w.Z, 0.0034f);
            _v = Vector3.Cross(_w, new Vector3(0.0072f, 1f, 0.0034f));
            _v = Vector3.Normalize(_v);

            //_u.X = (float)System.Math.Pow(_v.X, _w.X);
            //_u.Y = (float)System.Math.Pow(_v.Y, _w.Y);
            //_u.Z = (float)System.Math.Pow(_v.Z, _w.Z);
            _u = Vector3.Cross(_v, _w);

            Ray shadowRay = new Ray(hit.HitPoint, GetDirection(hit));

            if(InShadow(shadowRay, hit))
            {
                return _minAmount * _ls * LightColor;
            }
            else
            {
                return _ls * LightColor;
            }
        }
    }

}