using System.Numerics;

using RayTracerLib.Util;


namespace RayTracerLib.Light
{
    public class AmbientLight : AbstractLight
    {
        /// <summary>
        /// Radiance scaling factor (brightness)
        /// </summary>
        public float _ls;

        /// <summary>
        /// Light color
        /// </summary>
        public RGBColor LightColor { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public AmbientLight() : base()
        {
            _ls = 1f;
            LightColor = RGBColor.White;
        }

        /// <summary>
        /// Argument construcotr
        /// </summary>
        /// <param name="ls">Radiance scaling factor</param>
        /// <param name="color">Light color</param>
        public AmbientLight(float ls, RGBColor color) : base()
        {
            _ls = ls;
            LightColor = color;
        }

        public override Vector3 GetDirection(ShadeRec hit)
        {
            return Vector3.Zero;
        }

        public override bool InShadow(Ray ray, ShadeRec hit)
        {
            return false;
        }

        public override RGBColor L(ShadeRec hit)
        {
            return _ls * LightColor;
        }
    }

}