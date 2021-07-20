using RayTracerLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RayTracerLib.GeometricObject
{
    public class GeometricBox : AbstractGeometricObject
    {
        private float X0;
        private float Y0;
        private float Z0;

        private float X1;
        private float Y1;
        private float Z1;

		private static readonly double _kEpsilon = 0.001f;

		private BBox _bbox;
		public BBox BoundingBox
        {
			get
            {
				if(_bbox is null)
                {
					Vector3 p0 = new Vector3(X0, Y0, Z0);
					Vector3 p1 = new Vector3(X1, Y1, Z1);

					_bbox = new BBox(p0, p1);
				}

				return _bbox;
			}

			set
            {
				_bbox = value;
			}
        }

		public GeometricBox(Vector3 p0, Vector3 p1)
        {
			X0 = p0.X;
			Y0 = p0.Y;
			Z0 = p0.Z;

			X1 = p1.X;
			Y1 = p1.Y;
			Z1 = p1.Z;
		}

		public override bool Hit(Ray ray, ref double tmin, ref ShadeRec shr)
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

			int face_in, face_out;

			// find largest entering t value

			if (tx_min > ty_min)
            {
				t0 = tx_min;
				face_in = (a >= 0.0) ? 0 : 3;
			}				
			else
            {
				t0 = ty_min;
				face_in = (b >= 0.0) ? 1 : 4;
			}
				

			if (tz_min > t0)
            {
				t0 = tz_min;
				face_in = (c >= 0.0) ? 2 : 5;
			}
				

			// find smallest exiting t value

			if (tx_max < ty_max)
            {
				t1 = tx_max;
				face_out = (a >= 0.0) ? 3 : 0;
			}				
			else
            {
				t1 = ty_max;
				face_out = (b >= 0.0) ? 4 : 1;
			}
				

			if (tz_max < t1)
            {
				t1 = tz_max;
				face_out = (c >= 0.0) ? 5 : 2;
			}
			
			if(t0 < t1 && t1 > _kEpsilon) // condition for a hit
            {
				if(t0 > _kEpsilon)
                {
					tmin = t0; // ray hits outside the surface
					shr.Normal = GetNormal(face_in);
                }
				else
                {
					tmin = t1; // ray hits inside the surface
					shr.Normal = GetNormal(face_out);
                }

				shr.LocalHitPoint = ray.Origin + (float)tmin * ray.Direction;
				return true;
			
			}
			else
            {
				return false;
            }


			//return (t0 < t1 && t1 > _kEpsilon);
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

			int face_in, face_out;

			// find largest entering t value

			if (tx_min > ty_min)
			{
				t0 = tx_min;
				face_in = (a >= 0.0) ? 0 : 3;
			}
			else
			{
				t0 = ty_min;
				face_in = (b >= 0.0) ? 1 : 4;
			}


			if (tz_min > t0)
			{
				t0 = tz_min;
				face_in = (c >= 0.0) ? 2 : 5;
			}


			// find smallest exiting t value

			if (tx_max < ty_max)
			{
				t1 = tx_max;
				face_out = (a >= 0.0) ? 3 : 0;
			}
			else
			{
				t1 = ty_max;
				face_out = (b >= 0.0) ? 4 : 1;
			}


			if (tz_max < t1)
			{
				t1 = tz_max;
				face_out = (c >= 0.0) ? 5 : 2;
			}

			if (t0 < t1 && t1 > _kEpsilon) // condition for a hit
			{
				if (t0 > _kEpsilon)
				{
					tmin = t0; // ray hits outside the surface
					//shr.Normal = GetNormal(face_in);
				}
				else
				{
					tmin = t1; // ray hits inside the surface
					//shr.Normal = GetNormal(face_out);
				}

				//shr.LocalHitPoint = ray.Origin + (float)tmin * ray.Direction;
				return true;

			}
			else
			{
				return false;
			}
		}

		private Normal GetNormal(int faceHit)
        {
			switch(faceHit)
            {
				case 0: return new Normal(-1, 0, 0); // -x face
				case 1: return new Normal(0, -1, 0); // -y face
				case 2: return new Normal(0, 0, -1); // -z face
				case 3: return new Normal(1, 0, 0); // +x face
				case 4: return new Normal(0, 1, 0); // +y face
				case 5: return new Normal(0, 0, 1); // +z face
			}

			return new Normal(0, 0, 0);
        }

        public override BBox GetBoundingBox()
        {
			return BoundingBox;
        }
    }
}
