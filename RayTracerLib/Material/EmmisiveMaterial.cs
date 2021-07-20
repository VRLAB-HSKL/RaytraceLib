using RayTracerLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RayTracerLib.Material
{
    public class EmmisiveMaterial : AbstractMaterial
    {
        public float RadianceScalingFactor;

        public RGBColor Color;

        public EmmisiveMaterial()
        {
            RadianceScalingFactor = 1f;
            Color = RGBColor.White;
        }

        public override RGBColor Le(ref ShadeRec sr)
        {
            return RadianceScalingFactor * Color;
        }

        public override RGBColor AreaLightShade(ref ShadeRec hit)
        {
            if(Vector3.Dot(hit.Normal, hit.SpecRay.Direction) > 0f)
            {
                return RadianceScalingFactor * Color;
            }
            else
            {
                return RGBColor.Black;
            }
        }

        public override RGBColor PathShade(ref ShadeRec hit)
        {
            throw new NotImplementedException();
        }

        public override RGBColor Shade(ref ShadeRec hit, int depth)
        {
            //ToDo: Is this correct?
            return AreaLightShade(ref hit);

            //throw new NotImplementedException();
        }
    }
}
