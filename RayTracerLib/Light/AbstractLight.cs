using System.Numerics;

using RayTracerLib.Util;

namespace RayTracerLib.Light
{ 
    public abstract class AbstractLight 
    {
        public bool CastShadows { get; set; } = true;

        public abstract Vector3 GetDirection(ShadeRec hit);
        public abstract RGBColor L(ShadeRec hit);
        public abstract bool InShadow(Ray ray, ShadeRec hit);

        public virtual float G(ref ShadeRec hit)
        {
            return 1f;
        }

        public virtual float PDF(ref ShadeRec hit)
        {
            return 1f;
        }
    }

}