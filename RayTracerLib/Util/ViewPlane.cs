
using RayTracerLib.Sampler;

namespace RayTracerLib.Util
{
    public class ViewPlane
    {
        /// <summary>
        /// Horizontal image resolution
        /// </summary>
        public int HRes;

        /// <summary>
        /// Vertical image resolution
        /// </summary>
        public int VRes;

        /// <summary>
        /// Pixel size
        /// </summary>
        public float PixelSize;

        /// <summary>
        /// Monitor gamme factor
        /// </summary>
        public float Gamma;

        /// <summary>
        /// One over gamma
        /// </summary>
        public float INV_Gamma;

        public bool ShowOutOfGamut = false;

        private AbstractSampler _sampler;
        public AbstractSampler Sampler
        {
            get => _sampler;

            set
            {
                _sampler = value;
                NumSamples = _sampler.GetNumSamples();
            }

        }

        public int NumSamples { get; set; }

        public int MaxDepth;
    }
}
