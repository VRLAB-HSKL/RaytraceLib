using RayTracerLib.Material;
using RayTracerLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RayTracerLib.GeometricObject
{
    public class GeometricCompound : AbstractGeometricObject
    {
        public new List<AbstractGeometricObject> Objects = new List<AbstractGeometricObject>();

        private BBox _bbox;
        public BBox BoundingBox
        {
            get
            {
                if (_bbox is null)
                {
                    float p0x = Objects.Min(geo => (float)geo.GetBoundingBox().X0);
                    float p0y = Objects.Min(geo => (float)geo.GetBoundingBox().Y0);
                    float p0z = Objects.Min(geo => (float)geo.GetBoundingBox().Z0);

                    float p1x = Objects.Max(geo => (float)geo.GetBoundingBox().X1);
                    float p1y = Objects.Max(geo => (float)geo.GetBoundingBox().Y1);
                    float p1z = Objects.Max(geo => (float)geo.GetBoundingBox().Z1);

                    Vector3 p0 = new Vector3(p0x, p0y, p0z);
                    Vector3 p1 = new Vector3(p1x, p1y, p1z);

                    _bbox = new BBox(p0, p1);
                }

                return _bbox;
            }

            set
            {
                _bbox = value;
            }
        }



        public void SetMaterial(AbstractMaterial mat)
        {
            int numObjects = Objects.Count;
            for (int i = 0; i < numObjects; i++)
            {
                Objects[i].Material = mat;
            }
        }

        

        public GeometricCompound()
        {
            //float minX = Objects.Min(x => (float)x.GetBoundingBox().X0);
        }

        public override bool Hit(Ray ray, ref double tmin, ref ShadeRec shr)
        {
            double t = 0;
            Normal normal = new Normal();
            Vector3 localHitPoint = Vector3.Zero;
            bool hit = false;
            tmin = double.MaxValue;
            int numObjects = Objects.Count;

            for(int j = 0; j < numObjects; j++)
            {
                if(Objects[j].Hit(ray, ref t, ref shr) && (t < tmin))
                {
                    hit = true;
                    tmin = t;
                    Material = Objects[j].Material;
                    normal = shr.Normal;
                    localHitPoint = shr.LocalHitPoint;
                }
            }

            if(hit)
            {
                shr.T = (float)tmin;
                shr.Normal = normal;
                shr.LocalHitPoint = localHitPoint;
            }

            return hit;

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
            if (!CastShadows)
                return false;

            double t = 0;
            //Normal normal = new Normal();
            //Vector3 localHitPoint = Vector3.Zero;
            ShadeRec tmpRec = new ShadeRec(new World());
            bool hit = false;
            tmin = double.MaxValue;
            int numObjects = Objects.Count;

            for (int j = 0; j < numObjects; j++)
            {
                if (Objects[j].Hit(ray, ref t, ref tmpRec) && (t < tmin))
                {
                    hit = true;
                    tmin = t;
                    Material = Objects[j].Material;
                    //normal = shr.Normal;
                    //localHitPoint = shr.LocalHitPoint;
                }
            }

            //if (hit)
            //{
            //    //shr.T = (float)tmin;
            //    //shr.Normal = normal;
            //    //shr.LocalHitPoint = localHitPoint;
            //}

            return hit;
        }

        public override BBox GetBoundingBox()
        {
            return BoundingBox;
        }
    }
}
