using System.Numerics;

using RayTracerLib.Util;

namespace RayTracerLib.Light
{
    public class DirectionalLight : AbstractLight
    {
        private float _ls;
        public float RadianceFactor { get => _ls; set { _ls = value; } }

        private RGBColor _color;
        private Vector3 _dir;		// direction the light comes from
        private Vector3 _location;

        public DirectionalLight(Vector3 direction, Vector3 location)
        {
            _ls = 1f;
            _color = RGBColor.White;
            _dir = direction;
        }

        public override Vector3 GetDirection(ShadeRec hit)
        {
            return _dir;
        }

        public override bool InShadow(Ray ray, ShadeRec hit)
        {
            //if (Physics.Raycast(ray, out ShadeRec tmpHit, 30, ~(1 << 9)))
            //{
            //    float t = tmpHit.distance;
            //    float d = Vector3.Distance(_location, ray.origin);
            //    return t < d;
            //}

            return false;
        }

        public override RGBColor L(ShadeRec hit)
        {
            return _ls * _color;
        }
    }

}