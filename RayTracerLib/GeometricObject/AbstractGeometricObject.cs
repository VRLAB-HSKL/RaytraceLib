using RayTracerLib.Material;
using RayTracerLib.Util;
using System.Collections.Generic;
using System.Numerics;

namespace RayTracerLib.GeometricObject
{       
    public abstract class AbstractGeometricObject
    {
        public RGBColor Color;

        public AbstractMaterial Material;

        public bool CastShadows = true;

        // Needed for compound objects
        public List<AbstractGeometricObject> Objects = new List<AbstractGeometricObject>();



        public abstract bool Hit(Ray ray, ref double tmin, ref ShadeRec shr);

        public abstract bool ShadowHit(Ray ray, ref double tmin);

        public abstract Vector3 Sample();

        public virtual float Pdf(ref ShadeRec hit)
        {
            return 0f;
        }

        public abstract Vector3 Normal(Vector3 p);

        public abstract BBox GetBoundingBox();
        

    }
}
