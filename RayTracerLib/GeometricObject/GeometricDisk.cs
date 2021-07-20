using RayTracerLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RayTracerLib.GeometricObject
{
    public class GeometricDisk : AbstractGeometricObject
    {
        public Vector3 Center;
        public float Radius;
        public Normal normal;

        private static readonly double _kEpsilon = 0.001;

        public GeometricDisk(Vector3 center, float radius, Normal normal)
        {
            Center = center;
            Radius = radius;
            this.normal = normal;
        }


        public override bool Hit(Ray ray, ref double tmin, ref ShadeRec shr)
        {
            float t = Vector3.Dot((Center - ray.Origin), normal) / Vector3.Dot(ray.Direction, normal);

            if(t <= _kEpsilon)
            {
                return false;
            }

            Vector3 p = ray.Origin + t * ray.Direction;

            float rSquared = Radius * Radius;

            if(Vector3.DistanceSquared(Center, p) < rSquared)
            {
                tmin = t;
                shr.Normal = normal;
                shr.LocalHitPoint = p;
                return true;
            }
            else
            {
                return false;
            }



        }

        public override Vector3 Normal(Vector3 p)
        {
            throw new NotImplementedException();
        }

        public override Vector3 Sample()
        {
            throw new NotImplementedException();
        }

        public override bool ShadowHit(Ray ray, ref double tmin)
        {
            throw new NotImplementedException();
        }

        public override BBox GetBoundingBox()
        {
            // ToDo: Implement this
            return new BBox();
        }
    }
}
