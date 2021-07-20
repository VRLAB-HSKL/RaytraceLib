using RayTracerLib.Sampler;
using RayTracerLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RayTracerLib.GeometricObject
{
    public class GeometricRectangle : AbstractGeometricObject
    {
        public Vector3 P0;
        public Vector3 A;
        public Vector3 B;

        public double _aLenSquared;
        public double _bLenSquared;

        public Normal _normal;
        public AbstractSampler sampler;

        public float _area;
        public float _invArea;

        private BBox _bbox;
        public BBox BoundingBox
        {
            get
            {
                if (_bbox is null)
                {
                    Vector3 p1 = P0 + A + B;
                    _bbox = new BBox(P0, p1);
                }

                return _bbox;
            }

            set
            {
                _bbox = value;
            }
        }


        protected double kEpsilon = 0.001;

        public GeometricRectangle()
        {
            P0 = new Vector3(-1f, 0f, -1f);
            A = new Vector3(0f, 0f, 2f);
            B = new Vector3(2f, 0f, 0f);
            _aLenSquared = 4f;
            _bLenSquared = 4f;
            _normal = new Normal(0f, 1f, 0f);
            _area = 4f;
            _invArea = 0.25f;
        }

        public GeometricRectangle(Vector3 p0, Vector3 a, Vector3 b)
        {
            P0 = p0;
            A = a;
            B = b;
            _aLenSquared = a.LengthSquared();
            _bLenSquared = b.LengthSquared();
            _normal = new Normal(Vector3.Cross(a, b));
            _area = A.Length() * B.Length();
            _invArea = 1 / _area;
        }


        public override Vector3 Sample()
        {
            Vector2 samplePoint = sampler.SampleUnitSquare();
            return (P0 + samplePoint.X * A + samplePoint.Y * B);
        }

        public override bool Hit(Ray ray, ref double tmin, ref ShadeRec shr)
        {
            double t = Vector3.Dot((P0 - ray.Origin), _normal) / Vector3.Dot(ray.Direction, _normal);

            if (t <= kEpsilon)
                return (false);

            Vector3 p = ray.Origin + Vector3.Multiply((float)t, ray.Direction);
            Vector3 d = p - P0;

            double ddota = Vector3.Dot(d, A);

            if (ddota < 0.0 || ddota > _aLenSquared)
                return (false);

            double ddotb = Vector3.Dot(d, B);

            if (ddotb < 0.0 || ddotb > _bLenSquared)
                return (false);

            tmin = t;
            shr.Normal = _normal;
            shr.LocalHitPoint = p;

            return (true);
        }

        public override bool ShadowHit(Ray ray, ref double tmin)
        {
            double t = Vector3.Dot((P0 - ray.Origin), _normal) / Vector3.Dot(ray.Direction, _normal);

            if (t <= kEpsilon)
                return (false);

            Vector3 p = ray.Origin + Vector3.Multiply((float)t, ray.Direction);
            Vector3 d = p - P0;

            double ddota = Vector3.Dot(d, A);

            if (ddota < 0.0 || ddota > _aLenSquared)
                return (false);

            double ddotb = Vector3.Dot(d, B);

            if (ddotb < 0.0 || ddotb > _bLenSquared)
                return (false);

            tmin = t;
            //shr.Normal = _normal;
            //shr.LocalHitPoint = p;

            return (true);
        }

        public override float Pdf(ref ShadeRec hit)
        {
            return _invArea;
        }

        public override Vector3 Normal(Vector3 p)
        {
            return _normal;
        }

        public override BBox GetBoundingBox()
        {
            return BoundingBox;
        }
    }
}
