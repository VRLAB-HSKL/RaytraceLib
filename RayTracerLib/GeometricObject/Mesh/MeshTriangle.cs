using RayTracerLib.GeometricObject;
using RayTracerLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RayTracerLib.Material.Mesh
{
    public abstract class MeshTriangle : AbstractGeometricObject
    {
        public Mesh mesh;
        public Normal normal;

        public int index0;
        public int index1;
        public int index2;
        public float area;

        private BBox _bbox;
        public BBox BoundingBox
        {
            get
            {
                if (_bbox is null)
                {
                    float delta = 0.0001f; // To avoid degenerate bounding boxes

                    Vector3 v1 = mesh.Vertices[index0];
                    Vector3 v2 = mesh.Vertices[index1];
                    Vector3 v3 = mesh.Vertices[index2];

                    float x0 = (float)Math.Min(Math.Min(v1.X, v2.X), v3.X) - delta;
                    float y0 = (float)Math.Min(Math.Min(v1.Y, v2.Y), v3.Y) - delta;
                    float z0 = (float)Math.Min(Math.Min(v1.Z, v2.Z), v3.Z) - delta;

                    float x1 = (float)Math.Max(Math.Max(v1.X, v2.X), v3.X) + delta;
                    float y1 = (float)Math.Max(Math.Max(v1.Y, v2.Y), v3.Y) + delta;
                    float z1 = (float)Math.Max(Math.Max(v1.Z, v2.Z), v3.Z) + delta;

                    _bbox = new BBox(x0, x1, y0, y1, z0, z1);
                }

                return _bbox;
            }

            set
            {
                _bbox = value;
            }
        }

        protected static readonly double _kEpsilon = 0.001f;

        public MeshTriangle()
        {
            index0 = 0;
            index1 = 0;
            index2 = 0;
            normal = new Normal();
        }

        public MeshTriangle(Mesh mesh, int index0, int index1, int index2)
        {
            this.mesh = mesh;
            this.index0 = index0;
            this.index1 = index1;
            this.index2 = index2;
            //ComputeNormal(false);
        }

        public override BBox GetBoundingBox()
        {
            return BoundingBox;
        }

        public void ComputeNormal(bool reverseNormal)
        {
            Vector3 vec01 = mesh.Vertices[index1] - mesh.Vertices[index0];
            Vector3 vec02 = mesh.Vertices[index2] - mesh.Vertices[index0];

            normal = new Normal(Vector3.Cross(vec01, vec02));

            if (reverseNormal)
                normal = -normal;
        }

        public override abstract bool Hit(Ray ray, ref double tmin, ref ShadeRec shr);


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
            Vector3 v0 = mesh.Vertices[index0];
            Vector3 v1 = mesh.Vertices[index1];
            Vector3 v2 = mesh.Vertices[index2];

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


        protected float InterpolateU(float beta, float gamma)
        {
            return ((1 - beta - gamma) * mesh.U[index0]
                + beta * mesh.U[index1]
                    + gamma * mesh.U[index2]);
        }

        protected float InterpolateV(float beta, float gamma)
        {
            return ((1 - beta - gamma) * mesh.V[index0]
                + beta * mesh.V[index1]
                    + gamma * mesh.V[index2]);
        }

    }
}
