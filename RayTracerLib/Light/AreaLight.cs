using RayTracerLib.GeometricObject;
using RayTracerLib.Material;
using RayTracerLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RayTracerLib.Light
{
    public class AreaLight : AbstractLight
    {
        private AbstractGeometricObject _obj;
        public AbstractGeometricObject Object
        {
            get => _obj;
            set
            {
                _obj = value;
                Material = _obj.Material;
            }
        }


        public AbstractMaterial Material;
        public Vector3 SamplePoint;

        /// <summary>
        /// Assigned in GetDirection()
        /// </summary>
        public Normal LightNormal;

        /// <summary>
        /// Unit direction from hit point being shaded to sample point on light surface
        /// </summary>
        public Vector3 Wi;


        public AreaLight()
        {
           
        }

        public AreaLight(AreaLight rhs)
        {
            if(rhs.Object != null)
            {
                _obj = rhs.Object;
            }

            if(rhs.Material != null)
            {
                Material = rhs.Material;
            }
        }

        public override Vector3 GetDirection(ShadeRec hit)
        {
            SamplePoint = Object.Sample();
            LightNormal = new Normal(Object.Normal(SamplePoint));
            Wi = Vector3.Subtract(SamplePoint, hit.HitPoint);
            Wi = Vector3.Normalize(Wi);

            return Wi;
        }

        public override bool InShadow(Ray ray, ShadeRec hit)
        {
            double t = 0;
            int numObjects = hit.WorldRef.GeometricObjects.Count;
            float ts = Vector3.Dot((SamplePoint - ray.Origin), ray.Direction);
        
            for(int i = 0; i < numObjects; i++)
            {
                if (hit.WorldRef.GeometricObjects[i].ShadowHit(ray, ref t) && t < ts)
                    return true;
            }

            return false;
        }


        public override RGBColor L(ShadeRec hit)
        {
            float ndotd = -(Vector3.Dot(LightNormal, Wi));

            if(ndotd > 0f)
            {
                return Material.Le(ref hit);
            }
            else
            {
                return RGBColor.Black;
            }

        }

        public override float G(ref ShadeRec hit)
        {
            float ndotd = -Vector3.Dot(LightNormal, Wi);
            float d2 = Vector3.DistanceSquared(SamplePoint, hit.HitPoint);

            return (ndotd / d2);
        }

        public override float PDF(ref ShadeRec hit)
        {
            return _obj.Pdf(ref hit);
        }
    }
}
