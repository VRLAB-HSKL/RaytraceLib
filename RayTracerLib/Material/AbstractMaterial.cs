using RayTracerLib.Util;

namespace RayTracerLib.Material
{
    public abstract class AbstractMaterial
    {
        public bool CastShadows = true;

        public abstract RGBColor Shade(ref ShadeRec hit, int depth);

        public abstract RGBColor AreaLightShade(ref ShadeRec hit);

        public abstract RGBColor PathShade(ref ShadeRec hit);

        public virtual RGBColor Le(ref ShadeRec hit)
        {
            return RGBColor.Black;
        }
    }

}
