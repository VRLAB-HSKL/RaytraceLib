using RayTracerLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RayTracerLib.Tracer
{
    public class MultipleObjectsTracer : AbstractTracer
    {
        public MultipleObjectsTracer(World w) : base(w) { }

        public override RGBColor TraceRay(Ray ray)
        {
            ShadeRec shr = new ShadeRec(World.HitBareBonesObjects(ray));

            if(shr.HitAnObject)
            {
                return shr.Color;
            }
            else
            {
                return World.BackgroundColor;
            }
        }

        public override RGBColor TraceRay(Ray ray, int depth)
        {
            throw new NotImplementedException();
        }

        public override RGBColor TraceRay(Ray ray, float tmin, int depth)
        {
            throw new NotImplementedException();
        }
    }
}
