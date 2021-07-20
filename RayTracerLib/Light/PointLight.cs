using System.Numerics;

using RayTracerLib.Util;

namespace RayTracerLib.Light
{
    public class PointLight : AbstractLight
    {
        private float _ls;
        private RGBColor _color;
        private Vector3 _location;

        public PointLight() : base()
        {
            _color = RGBColor.White;
        }

        public PointLight(float ls, RGBColor color, Vector3 location) : base()
        {
            _ls = ls;
            _color = color;
            _location = location;
        }

        public override Vector3 GetDirection(ShadeRec hit)
        {
            Vector3 tmp = (_location - hit.HitPoint);

            // ToDo: Decouple "hat" function // hat = make unit vector
            float length = (float)System.Math.Sqrt(tmp.X * tmp.X + tmp.Y * tmp.Y + tmp.Z * tmp.Z);
            tmp.X /= length;
            tmp.Y /= length;
            tmp.Z /= length;

            //tmp = Vector3.Normalize(tmp);


            return tmp; 
        }

        public override bool InShadow(Ray ray, ShadeRec hit)
        {
            //if (Physics.Raycast(ray, out ShadeRec tmpHit, 30, ~(1 << 9)))
            //{
            //    float t = tmpHit.distance;
            //    float d = Vector3.Distance(_location, ray.origin);
            //    return t < d;
            //}

            double t = 0;
            int numObjects = hit.WorldRef.GeometricObjects.Count;
            float d = Vector3.Distance(_location, ray.Origin);

            for(int j = 0; j < numObjects; j++)
            {
                if (hit.WorldRef.GeometricObjects[j].Hit(ray, ref t, ref hit) && t < d) //ShadowHit(ray, ref t) && t < d)
                    return true;
            }

            return false;
        }

        public override RGBColor L(ShadeRec hit)
        {
            return _ls * _color;
        }
    }


}