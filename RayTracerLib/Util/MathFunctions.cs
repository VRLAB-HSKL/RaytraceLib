using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracerLib.Util
{
    public static class MathFunctions
    {
        // Fisher-Yates Shuffle
        // https://stackoverflow.com/a/1262619
        public static List<int> FisherYatesShuffle(List<int> list)
        {
            Random rnd = new Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(0, n + 1);  // UnityEngine.Random.Range(0, n + 1);
                int value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }
    }
}
