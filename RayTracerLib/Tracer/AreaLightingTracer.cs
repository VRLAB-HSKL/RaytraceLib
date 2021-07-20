using RayTracerLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracerLib.Tracer
{
    public class AreaLightingTracer : AbstractTracer
    {
        public AreaLightingTracer(World w) : base(w) { }

        public override RGBColor TraceRay(Ray ray)
        {
            ShadeRec sr = World.HitObjects(ray);

            if (sr.HitAnObject)
            {
                sr.SpecRay = ray;
                return sr.Material.AreaLightShade(ref sr);
            }
            else
            {
                return World.BackgroundColor;
            }
        }

        public override RGBColor TraceRay(Ray ray, int depth)
        {
            if(depth > World.ViewPlane.MaxDepth)
            {
                return RGBColor.Black;
            }
            else
            {
                ShadeRec hit = World.HitObjects(ray);

                if(hit.HitAnObject)
                {
                    hit.Depth = depth;
                    hit.SpecRay = ray;

                    return hit.Material.AreaLightShade(ref hit);
                }
                else
                {
                    return World.BackgroundColor;
                }

            }
        }

        public override RGBColor TraceRay(Ray ray, float tmin, int depth)
        {
            throw new NotImplementedException();
        }
    }
}
