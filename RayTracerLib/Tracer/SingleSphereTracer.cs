using RayTracerLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RayTracerLib.Tracer
{
    public class SingleSphereTracer : AbstractTracer
    {
        public SingleSphereTracer(World w) : base(w) { }

        public override RGBColor TraceRay(Ray ray)
        {
            ShadeRec sr = new ShadeRec(World);
            double t = 0.0;

            if(World.Sphere.Hit(ray, ref t, ref sr))
            {
                return RGBColor.Red;
            }
            else
            {
                return RGBColor.Black;
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
