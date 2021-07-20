using System.Numerics;

namespace RayTracerLib.Util
{
    public class Ray
    {
        /// <summary>
        /// Starting point
        /// </summary>
        public Vector3 Origin;


        private Vector3 _d;
        /// <summary>
        /// Direction vector
        /// </summary>
        public Vector3 Direction
        {
            get => _d;
            set
            {
                if (float.IsNaN(value.X))
                {
                    int avb = 0;
                }


                _d = value;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Ray() { }

        /// <summary>
        /// Argument constructor
        /// </summary>
        /// <param name="origin">origin vector</param>
        /// <param name="dir">direction vector</param>
        public Ray(Vector3 origin, Vector3 dir) {

            Origin = origin;
            Direction = dir;        

        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="r">Copy source</param>
        public Ray(Ray r)
        {
            Origin = r.Origin;
            Direction = r.Direction;
        }

    }
}
