using RayTracerLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RayTracerLib.GeometricObject
{
    public class GeometricTriangle : AbstractGeometricObject
    {
        public Vector3 v0, v1, v2;
        public Normal normal;

        private static readonly double _kEpsilon = 0.001f;

        public GeometricTriangle()
        {
            v0 = Vector3.Zero;
            v1 = Vector3.UnitZ;
            v2 = Vector3.UnitX;

            normal = new Normal(0f, 1f, 0f);
        }

        public GeometricTriangle(Vector3 a, Vector3 b, Vector3 c) : base()
        {
            v0 = a;
            v1 = b;
            v2 = c;

            normal = new Normal(Vector3.Cross((v1 - v0), (v2 - v0)));
            
        }

        public override bool Hit(Ray ray, ref double tmin, ref ShadeRec shr)
        {
            double a = v0.X - v1.X;
            double b = v0.X - v2.X;
            double c = ray.Direction.X;
            double d = v0.X - ray.Origin.X;

            double e = v0.Y - v1.Y;
            double f = v0.Y - v2.Y;
            double g = ray.Direction.Y;
            double h = v0.Y - ray.Origin.Y;

            double i = v0.Z - v1.Z;
            double j = v0.Z - v2.Z;
            double k = ray.Direction.Z;
            double l = v0.Z - ray.Origin.Z;

            double m = f * k - g * j;
            double n = h * k - g * l;
            double p = f * l - h * j;

            double q = g * i - e * k;
            double s = e * j - f * i;

            double invDemon = 1f / (a * m + b * q + c * s);

            double e1 = d * m - b * n - c * p;
            double beta = e1 * invDemon;

            if (beta < 0f)
                return false;

            double r = e * l - h * i;
            double e2 = a * n + d * q + c * r;
            double gamma = e2 * invDemon;

            if (gamma < 0f)
                return false;

            if (beta + gamma > 1f)
                return false;

            double e3 = a * p - b * r + d * s;
            double t = e3 * invDemon;

            if (t < _kEpsilon)
                return false;

            tmin = t;
            shr.Normal = normal;
            shr.LocalHitPoint = ray.Origin + (float)t * ray.Direction;

            return true;
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
            float p0x = Math.Min(Math.Min(v0.X, v1.X), v2.X);
            float p0y = Math.Min(Math.Min(v0.Y, v1.Y), v2.Y);
            float p0z = Math.Min(Math.Min(v0.Z, v1.Z), v2.Z);

            float p1x = Math.Max(Math.Max(v0.X, v1.X), v2.X);
            float p1y = Math.Max(Math.Max(v0.Y, v1.Y), v2.Y);
            float p1z = Math.Max(Math.Max(v0.Z, v1.Z), v2.Z);

            Vector3 p0 = new Vector3(p0x, p0y, p0z);
            Vector3 p1 = new Vector3(p1x, p1y, p1z);

            return new BBox(p0, p1);
        }
    }
}
