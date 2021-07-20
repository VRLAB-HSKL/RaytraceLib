using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RayTracerLib.Util
{

    public class Normal
    {
        public double X;
        public double Y;
        public double Z;

        public Normal()
        {
            X = 0f;
            Y = 0f;
            Z = 0f;
        }

        public Normal(double a)
        {
            X = a;
            Y = a;
            Z = a;
        }

        public Normal(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Normal(Normal rhs)
        {
            X = rhs.X;
            Y = rhs.Y;
            Z = rhs.Z;
        }

        public Normal(Vector3 rhs)
        {
            Vector3 normalizedVec = Vector3.Normalize(rhs); 

            X = normalizedVec.X;
            Y = normalizedVec.Y;
            Z = normalizedVec.Z;
        }

        public override string ToString()
        {
            return X + " " + Y + " " + Z;
        }

        public static Normal operator-(Normal n)
        {
            return new Normal(-n.X, -n.Y, -n.Z);
        }

        public static Normal operator+(Normal lhs, Normal rhs)
        {
            return new Normal(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
        }

        public static Vector3 operator +(Vector3 lhs, Normal rhs)
        {
            return new Vector3(lhs.X + (float)rhs.X, lhs.Y + (float)rhs.Y, lhs.Z + (float)rhs.Z);
        }

        public static Vector3 operator -(Vector3 lhs, Normal rhs)
        {
            return new Vector3(lhs.X - (float)rhs.X, lhs.Y - (float)rhs.Y, lhs.Z - (float)rhs.Z);
        }


        public static double operator*(Normal lhs, Vector3 rhs)
        {
            return (lhs.X * rhs.X + lhs.Y * rhs.Y + lhs.Z * rhs.Z);
        }

        public static Normal operator *(Normal lhs, double rhs)
        {
            return new Normal(lhs.X * rhs, lhs.Y * rhs, lhs.Z * rhs);
        }

        public static Normal operator *(double lhs, Normal rhs)
        {
            return new Normal(lhs * rhs.X, lhs * rhs.Y, lhs * rhs.Z);
        }

        public static Normal operator *(Vector3 lhs, Normal rhs)
        {
            return new Normal(lhs.X * rhs.X, lhs.Y * rhs.Y, lhs.Z * rhs.Z);
        }

        public static implicit operator Vector3(Normal n)
        {
            return new Vector3((float)n.X, (float)n.Y, (float)n.Z);
        }

    }
}
