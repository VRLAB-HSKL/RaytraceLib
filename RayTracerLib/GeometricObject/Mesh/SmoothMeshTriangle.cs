using RayTracerLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RayTracerLib.Material.Mesh
{
    public class SmoothMeshTriangle : MeshTriangle
    {
        public SmoothMeshTriangle(Mesh m, int i0, int i1, int i2)
            : base(m, i0, i1, i2)
        {}


        public override bool Hit(Ray ray, ref double tmin, ref ShadeRec shr)
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
			shr.Normal = InterpolateNormal((float)beta, (float)gamma); // for smooth shading
			shr.LocalHitPoint = ray.Origin + (float)t * ray.Direction;

			return (true);
		}

        protected Normal InterpolateNormal(float beta, float gamma)
        {
            Normal normal = new Normal((1 - beta - gamma) * mesh.Normals[index0]
                        + beta * mesh.Normals[index1]
                                + gamma * mesh.Normals[index2]);
            
            return (normal);
        }


    }
}
