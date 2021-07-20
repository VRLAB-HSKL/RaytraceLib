using RayTracerLib.BRDF;
using RayTracerLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracerLib.Material
{
    public class PlasticMaterial : AbstractMaterial
    {
        private AbstractBRDF _ambientBRDF;
        private AbstractBRDF _diffuseBRDF;
        private AbstractBRDF _specularBRDF;

        public override RGBColor AreaLightShade(ref ShadeRec hit)
        {
            throw new NotImplementedException();
        }

        public override RGBColor PathShade(ref ShadeRec hit)
        {
            throw new NotImplementedException();
        }

        public override RGBColor Shade(ref ShadeRec hit, int depth)
        {
            throw new NotImplementedException();
        }
    }
}
