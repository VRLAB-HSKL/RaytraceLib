
using RayTracerLib.Util;

namespace RayTracerLib.Tracer
{
    public abstract class AbstractTracer
    {
        protected World World;

        protected AbstractTracer(World w) { World = w; }

        public abstract RGBColor TraceRay(Ray ray);

        public abstract RGBColor TraceRay(Ray ray, int depth);

        public abstract RGBColor TraceRay(Ray ray, float tmin, int depth);
    }

}