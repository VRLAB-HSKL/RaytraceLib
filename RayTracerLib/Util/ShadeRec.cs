using System.Numerics;

using RayTracerLib.Material;

namespace RayTracerLib.Util
{
    public class ShadeRec
    {
        /// <summary>
        /// Did the ray hit an object ?
        /// </summary>
        public bool HitAnObject;

        /// <summary>
        /// Nearest object's material
        /// </summary>
        public AbstractMaterial Material;

        /// <summary>
        /// World coordinates of hit point
        /// </summary>
        public Vector3 HitPoint;

        /// <summary>
        /// For attaching textures to objects
        /// </summary>
        public Vector3 LocalHitPoint;

        private Normal _norm;
        /// <summary>
        /// Normal at hit point
        /// </summary>
        public Normal Normal
        {
            get => _norm;
            set
            {
                //Vector3 x = value;
                //x = Vector3.Normalize(x);
                //_norm = new Normal(x);
                _norm = value;
            }
        }

        public RGBColor Color;

        /// <summary>
        /// Ray for specular highlights
        /// </summary>
        public Ray SpecRay { get; set; }

        /// <summary>
        /// Recursion depth
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Direction for area lights
        /// </summary>
        public Vector3 Direction;

        /// <summary>
        /// World reference
        /// </summary>
        public World WorldRef;

        public float T { get; set; }


        /// <summary>
        /// Argument constructor
        /// </summary>
        /// <param name="w">World reference</param>
        public ShadeRec(World w)
        {
            HitAnObject = false;
            Material = null;
            HitPoint = Vector3.Zero;
            LocalHitPoint = Vector3.Zero;
            Normal = new Normal(0f);
            SpecRay = new Ray();
            Depth = 0;
            Direction = Vector3.Zero;
            WorldRef = w;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="rhs">Copy source</param>
        public ShadeRec(ShadeRec rhs)
        {
            HitAnObject = rhs.HitAnObject;
            Material = rhs.Material;
            HitPoint = rhs.HitPoint;
            LocalHitPoint = rhs.LocalHitPoint;
            Normal = rhs.Normal;
            Color = rhs.Color;
            SpecRay = rhs.SpecRay;
            Depth = rhs.Depth;
            Direction = rhs.Direction;
            WorldRef = rhs.WorldRef;
            T = rhs.T;
        }
    }
}
