using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracerLib.Util
{
    public static class RandomNumberGenerator
    {
        public static Random _rnd = new Random();        

        public static int RandInt()
        {
            return _rnd.Next();
        }

        public static int RandIntInRange(int minValue, int maxValue)
        {
            float value = RandFloat();
            return minValue + (int)(value * (maxValue - minValue));
        }

        public static float RandFloat()
        {
            return (float) _rnd.NextDouble(); //((float)_rnd.Next() / float.MaxValue);
        }

        public static float RandFloatInRange(float minValue, float maxValue)
        {
            float value = RandFloat();
            return minValue + (value * (maxValue - minValue));
        }
        

        public static void SetRandSeed(int seed)
        {
            _rnd = new Random(seed);
        }

    }
}
