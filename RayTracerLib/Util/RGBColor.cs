using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracerLib.Util
{
	public class RGBColor
    {
		public float R;
		public float G;
		public float B;


		public static RGBColor Black = new RGBColor(0f, 0f, 0f);
		public static RGBColor White = new RGBColor(1f, 1f, 1f);
		public static RGBColor Red = new RGBColor(1f, 0f, 0f);
		public static RGBColor Green = new RGBColor(0f, 1f, 0f);
		public static RGBColor Blue = new RGBColor(0f, 0f, 1f);
		public static RGBColor Yellow = new RGBColor(1f, 1f, 0f);

		public RGBColor() 
		{
			R = 0f;
			G = 0f;
			B = 0f;
		}

		public RGBColor(float v)
        {
			R = v;
			G = v;
			B = v;
        }

		public RGBColor(float r, float g, float b)
        {
			R = r;
			G = g;
			B = b;
        }

		public RGBColor(RGBColor c)
        {
			R = c.R;
			G = c.B;
			G = c.B;
        }

		public RGBColor PowC(float p)
        {
			float r = (float)Math.Pow(R, p);
			float g = (float)Math.Pow(G, p);
			float b = (float)Math.Pow(B, p);
			return new RGBColor(r, g, b);
		}

		public float Average()
        {
			return (0.333333333333f * (R + G + B));
        }

        public override string ToString()
        {
			return (R + " " + G + " " + B);
        }

        public override bool Equals(object obj)
        {
            return obj is RGBColor color &&
                   R == color.R &&
                   G == color.G &&
                   B == color.B;
        }

        public override int GetHashCode()
        {
            int hashCode = -1520100960;
            hashCode = hashCode * -1521134295 + R.GetHashCode();
            hashCode = hashCode * -1521134295 + G.GetHashCode();
            hashCode = hashCode * -1521134295 + B.GetHashCode();
            return hashCode;
        }

        public static  RGBColor operator+(RGBColor lhs, RGBColor rhs)
        {
			return new RGBColor(lhs.R + rhs.R, lhs.G + rhs.G, lhs.B + rhs.B);
        }

		public static RGBColor operator *(RGBColor lhs, float rhs)
		{
			return new RGBColor(lhs.R * rhs, lhs.G * rhs, lhs.B * rhs);
		}

		public static RGBColor operator *(float lhs, RGBColor rhs)
		{
			float red = lhs * rhs.R;
			float green = lhs * rhs.G;
			float blue = lhs * rhs.B;
			RGBColor val =  new RGBColor(red, green, blue);
			return val;
		}

		public static RGBColor operator /(RGBColor lhs, float rhs)
		{
			return new RGBColor(lhs.R / rhs, lhs.G / rhs, lhs.B / rhs);
		}

		public static RGBColor operator *(RGBColor lhs, RGBColor rhs)
		{
			return new RGBColor(lhs.R * rhs.R, lhs.G * rhs.G, lhs.B * rhs.B);
		}


		public static bool operator ==(RGBColor lhs, RGBColor rhs)
		{
			return lhs.R == rhs.R && lhs.G == rhs.G && lhs.B == rhs.B;
		}

		public static bool operator !=(RGBColor lhs, RGBColor rhs)
		{
			return !(lhs == rhs);
		}

		


	}

}
