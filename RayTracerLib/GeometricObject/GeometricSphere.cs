using System;
using System.Numerics;
using System.Windows.Media;

using RayTracerLib.Util;

namespace RayTracerLib.GeometricObject
{
    public class GeometricSphere : AbstractGeometricObject
    {
        private Vector3 _center;

        private float _radius;

        private double _kEpsilon = 0.001f;


        private BBox _bbox;
        public BBox BoundingBox
        {
            get
            {
                if (_bbox is null)
                {
                    Vector3 p0 = new Vector3(_center.X - _radius, _center.Y - _radius, _center.Z - _radius);
                    Vector3 p1 = new Vector3(_center.X + _radius, _center.Y + _radius, _center.Z + _radius);

                    _bbox = new BBox(p0, p1);
                }

                return _bbox;
            }

            set
            {
                _bbox = value;
            }
        }



        public GeometricSphere(Vector3 c, float r)
        {
            _center = c;
            _radius = r;
        }

        public GeometricSphere(Vector3 c, float r, RGBColor col)
        {
            _center = c;
            _radius = r;
            Color = col;
        }

        public override BBox GetBoundingBox()
        {
            return BoundingBox;
        }

        public override bool Hit(Ray ray, ref double tmin, ref ShadeRec shr)
        {
            if (!BoundingBox.Hit(ray))
                return false;

            double t;
            Vector3 tmp = ray.Origin - _center;
            double a = Vector3.Dot(ray.Direction, ray.Direction);
            double b = Vector3.Dot(Vector3.Multiply(2f, tmp), ray.Direction);
            double c = Vector3.Dot(tmp, tmp) - _radius * _radius;
            double disc = b * b - 4f * a * c;

            if(disc < 0f)
            {
                return false;
            }
            else
            {
                double e = Math.Sqrt(disc);
                double denom = 2f * a;

                // Smaller root
                t = (-b - e) / denom; 
                if(t > _kEpsilon)
                {
                    tmin = t;
                    shr.Normal = new Normal(tmp + Vector3.Multiply((float)t, ray.Direction) / _radius);
                    shr.LocalHitPoint = ray.Origin + Vector3.Multiply((float)t, ray.Direction);
                    return true;
                }

                // Larger root
                t = (-b + e) / denom;
                if (t > _kEpsilon)
                {
                    tmin = t;
                    shr.Normal = new Normal(tmp + Vector3.Multiply((float)t, ray.Direction) / _radius);
                    shr.LocalHitPoint = ray.Origin + Vector3.Multiply((float)t, ray.Direction);
                    return true;
                }
            }

            return false;
        }

        public override Vector3 Normal(Vector3 p)
        {
            throw new NotImplementedException();
        }

        public override float Pdf(ref ShadeRec hit)
        {
            throw new NotImplementedException();
        }

        public override Vector3 Sample()
        {
            throw new NotImplementedException();
        }

        public override bool ShadowHit(Ray ray, ref double tmin)
        {
            if (!CastShadows)
            {
                return false;
            }                

            double t;
            Vector3 tmp = ray.Origin - _center;
            double a = Vector3.Dot(ray.Direction, ray.Direction);
            double b = Vector3.Dot(Vector3.Multiply(2f, tmp), ray.Direction);
            double c = Vector3.Dot(tmp, tmp) - _radius * _radius;
            double disc = b * b - 4f * a * c;

            if (disc < 0f)
            {
                return false;
            }
            else
            {
                double e = Math.Sqrt(disc);
                double denom = 2f * a;

                // Smaller root
                t = (-b - e) / denom;
                if (t > _kEpsilon)
                {
                    tmin = t;
                    //shr.Normal = (tmp + Vector3.Multiply((float)t, ray.Direction) / _radius);
                    //shr.LocalHitPoint = ray.Origin + Vector3.Multiply((float)t, ray.Direction);
                    return true;
                }

                // Larger root
                t = (-b + e) / denom;
                if (t > _kEpsilon)
                {
                    tmin = t;
                    //shr.Normal = (tmp + Vector3.Multiply((float)t, ray.Direction) / _radius);
                    //shr.LocalHitPoint = ray.Origin + Vector3.Multiply((float)t, ray.Direction);
                    return true;
                }
            }

            return false;
        }
    }
}
