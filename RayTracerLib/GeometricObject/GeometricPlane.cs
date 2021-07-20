using System;
using System.Numerics;
using System.Windows.Media;
using RayTracerLib.Material;
using RayTracerLib.Util;

namespace RayTracerLib.GeometricObject
{
    public class GeometricPlane : AbstractGeometricObject
    {
        /// <summary>
        /// Point through which plane passes
        /// </summary>
        private Vector3 point;

        /// <summary>
        /// Normal to the plane
        /// </summary>
        private Normal normal;

        private BBox _bbox;
        public BBox BoundingBox
        {
            get
            {
                if (_bbox is null)
                {
                    Vector3 p0 = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
                    Vector3 p1 = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

                    // ToDo: Add case for when normal is perpendicular to an axis !

                    _bbox = new BBox(p0, p1);
                }

                return _bbox;
            }

            set
            {
                _bbox = value;
            }
        }




        private static readonly double kEpsilon = 0.001f;
        

        public GeometricPlane() { }

        public GeometricPlane(Vector3 p, Normal n)
        {
            point = p;
            normal = n;
        }

        public GeometricPlane(Vector3 position, Normal normal, RGBColor c)
        {
            point = position;
            this.normal = normal;
            Color = c;            
        }

        public override bool Hit(Ray ray, ref double tmin, ref ShadeRec shr)
        {
            if (!BoundingBox.Hit(ray))
                return false;

            double t = Vector3.Dot((point - ray.Origin), normal) / Vector3.Dot(ray.Direction, normal);

            if(t > kEpsilon)
            {
                tmin = t;
                shr.Normal = normal;
                shr.LocalHitPoint = ray.Origin + Vector3.Multiply((float)t, ray.Direction);

                return true;
            }
            else
            {
                return false;
            }
            
        }

        public override bool ShadowHit(Ray ray, ref double tmin)
        {
            float v1 = Vector3.Dot((point - ray.Origin), normal);
            float v2 = Vector3.Dot(ray.Direction, normal);
            float t = v1 / v2;

            if(t > kEpsilon)
            {
                tmin = t;
                return true;
            }
            else
            {
                return false;
            }            

        }

        public override Vector3 Sample()
        {
            throw new NotImplementedException();
        }

        public override float Pdf(ref ShadeRec hit)
        {
            throw new NotImplementedException();
        }

        public override Vector3 Normal(Vector3 p)
        {
            throw new NotImplementedException();
        }

        public override BBox GetBoundingBox()
        {
            return BoundingBox;
        }
    }
}
