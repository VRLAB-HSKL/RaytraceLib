﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RayTracerLib.Util
{
    public class BBox
    {
        public double X0;
        public double X1;

        public double Y0;
        public double Y1;

        public double Z0;
        public double Z1;

        private static readonly double _kEpsilon = 0.001f;

        public BBox()
        {
            X0 = -1f;
            X1 = -1f;
            Y0 = -1f;
            Y1 = -1f;
            Z0 = -1f;
            Z1 = -1f;
        }

        public BBox(
            double x0, double x1,
            double y0, double y1,
            double z0, double z1)
        {
            X0 = x0;
            X1 = x1;
            Y0 = y0;
            Y1 = y1;
            Z0 = z0;
            Z1 = z1;
        }


        public BBox(Vector3 p0, Vector3 p1)
        {
            X0 = p0.X;
            X1 = p1.X;
            
            Y0 = p0.Y;
            Y1 = p1.Y;

            Z0 = p0.Z;
            Z1 = p1.Z;
        }

        public BBox(BBox rhs)
        {
            X0 = rhs.X0;
            X1 = rhs.X1;
            Y0 = rhs.Y0;
            Y1 = rhs.Y1;
            Z0 = rhs.Z0;
            Z1 = rhs.Z1;
        }

        public bool Hit(Ray ray)
        {
			double ox = ray.Origin.X; 
            double oy = ray.Origin.Y; 
            double oz = ray.Origin.Z;
			
            double dx = ray.Direction.X; 
            double dy = ray.Direction.Y; 
            double dz = ray.Direction.Z;

			double tx_min, ty_min, tz_min;
			double tx_max, ty_max, tz_max;

			double a = 1.0 / dx;
			if (a >= 0)
			{
				tx_min = (X0 - ox) * a;
				tx_max = (X1 - ox) * a;
			}
			else
			{
				tx_min = (X1 - ox) * a;
				tx_max = (X0 - ox) * a;
			}

			double b = 1.0 / dy;
			if (b >= 0)
			{
				ty_min = (Y0 - oy) * b;
				ty_max = (Y1 - oy) * b;
			}
			else
			{
				ty_min = (Y1 - oy) * b;
				ty_max = (Y0 - oy) * b;
			}

			double c = 1.0 / dz;
			if (c >= 0)
			{
				tz_min = (Z0 - oz) * c;
				tz_max = (Z1 - oz) * c;
			}
			else
			{
				tz_min = (Z1 - oz) * c;
				tz_max = (Z0 - oz) * c;
			}

			double t0, t1;

			// find largest entering t value

			if (tx_min > ty_min)
				t0 = tx_min;
			else
				t0 = ty_min;

			if (tz_min > t0)
				t0 = tz_min;

			// find smallest exiting t value

			if (tx_max < ty_max)
				t1 = tx_max;
			else
				t1 = ty_max;

			if (tz_max < t1)
				t1 = tz_max;

			return (t0 < t1 && t1 > _kEpsilon);
		}

        public bool Inside(Vector3 p)
        {
            return ((p.X > X0 && p.X < X1) && (p.Y > Y0 && p.Y < Y1) && (p.Z > Z0 && p.Z < Z1));
        }

    }
}
