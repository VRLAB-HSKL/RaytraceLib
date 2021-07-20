using RayTracerLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RayTracerLib.GeometricObject.Mesh
{
    public class SmoothTriangle : AbstractGeometricObject
    {
        public Normal n0;
        public Normal n1;
        public Normal n2;

        public Vector3 v0;
        public Vector3 v1;
        public Vector3 v2;

        private BBox _bbox;
        public BBox BoundingBox
        {
            get
            {
                if (_bbox is null)
                {
                    float delta = 0.0001f; // To avoid degenerate bounding boxes

                    float bx0 = Math.Min(Math.Min(v0.X, v1.X), v2.X) - delta;
                    float by0 = Math.Min(Math.Min(v0.Y, v1.Y), v2.Y) - delta;
                    float bz0 = Math.Min(Math.Min(v0.Z, v1.Z), v2.Z) - delta;

                    float bx1 = Math.Max(Math.Max(v0.X, v1.X), v2.X) + delta;
                    float by1 = Math.Max(Math.Max(v0.Y, v1.Y), v2.Y) + delta;
                    float bz1 = Math.Max(Math.Max(v0.Z, v1.Z), v2.Z) + delta;

                    _bbox = new BBox(bx0, bx1, by0, by1, bz0, bz1);
                }

                return _bbox;
            }

            set
            {
                _bbox = value;
            }
        }

        private static readonly double _kEpsilon = 0.001;

        public SmoothTriangle() : base()
        {
            v0 = Vector3.Zero;
            v1 = Vector3.UnitZ;
            v2 = Vector3.UnitX;

            n0 = new Normal(0, 1, 0);
            n1 = new Normal(0, 1, 0);
            n2 = new Normal(0, 1, 0);
        }

        public SmoothTriangle(Vector3 a, Vector3 b, Vector3 c) : base()
        {
            v0 = a;
            v1 = b;
            v2 = c;

            n0 = new Normal(0, 1, 0);
            n1 = new Normal(0, 1, 0);
            n2 = new Normal(0, 1, 0);
        }

        public override BBox GetBoundingBox()
        {
            return BoundingBox;
        }

        public override bool Hit(Ray ray, ref double tmin, ref ShadeRec shr)
        {
            double a = v0.X - v1.X, b = v0.X - v2.X, c = ray.Direction.X, d = v0.X - ray.Origin.X;
            double e = v0.Y - v1.Y, f = v0.Y - v2.Y, g = ray.Direction.Y, h = v0.Y - ray.Origin.Y;
            double i = v0.Z - v1.Z, j = v0.Z - v2.Z, k = ray.Direction.Z, l = v0.Z - ray.Origin.Z;

            double m = f * k - g * j, n = h * k - g * l, p = f * l - h * j;
            double q = g * i - e * k, s = e * j - f * i;

            double inv_denom = 1.0 / (a * m + b * q + c * s);

            double e1 = d * m - b * n - c * p;
            double beta = e1 * inv_denom;

            if (beta < 0.0)
                return (false);

            double r = e * l - h * i;
            double e2 = a * n + d * q + c * r;
            double gamma = e2 * inv_denom;

            if (gamma < 0.0)
                return (false);

            if (beta + gamma > 1.0)
                return (false);

            double e3 = a * p - b * r + d * s;
            double t = e3 * inv_denom;

            if (t < _kEpsilon)
                return (false);

            tmin = t;
            shr.Normal = InterpolateNormal((float)beta, (float)gamma);
            shr.LocalHitPoint = ray.Origin + (float)t * ray.Direction;

            return (true);
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
            double a = v0.X - v1.X, b = v0.X - v2.X, c = ray.Direction.X, d = v0.X - ray.Origin.X;
            double e = v0.Y - v1.Y, f = v0.Y - v2.Y, g = ray.Direction.Y, h = v0.Y - ray.Origin.Y;
            double i = v0.Z - v1.Z, j = v0.Z - v2.Z, k = ray.Direction.Z, l = v0.Z - ray.Origin.Z;

            double m = f * k - g * j, n = h * k - g * l, p = f * l - h * j;
            double q = g * i - e * k, s = e * j - f * i;

            double inv_denom = 1.0 / (a * m + b * q + c * s);

            double e1 = d * m - b * n - c * p;
            double beta = e1 * inv_denom;

            if (beta < 0.0)
                return (false);

            double r = e * l - h * i;
            double e2 = a * n + d * q + c * r;
            double gamma = e2 * inv_denom;

            if (gamma < 0.0)
                return (false);

            if (beta + gamma > 1.0)
                return (false);

            double e3 = a * p - b * r + d * s;
            double t = e3 * inv_denom;

            if (t < _kEpsilon)
                return (false);

            tmin = t;

            return (true);
        }

        private Normal InterpolateNormal(float beta, float gamma)
        {
            Normal normal = new Normal((1 - beta - gamma) * n0 + beta * n1 + gamma * n2);
            return normal;
        }

    }
}
