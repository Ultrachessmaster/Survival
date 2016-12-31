using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoiseTest;

namespace Simulation
{
    class Generation
    {
        public static float waterbiome = 0.3f;
        public static int[,,] Generate(Vector3 size)
        {
            int[,] map = new int[(int)size.X, (int)size.Y];
            OpenSimplexNoise o = new OpenSimplexNoise();
            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    var noisex = (x / size.X) - 0.5f;
                    var noisey = (y / size.Y) - 0.5f;
                    float height = ((float)o.Evaluate(25 * noisex, 25 * noisey) * 0.5f) + 0.5f;
                    float scaledheigt = (float)Math.Pow(height, 2);
                    //Console.WriteLine(height);
                    if(height < waterbiome)
                    {
                        map[x, y] = 4;
                    }
                }
            }

            int[,,] fullmap = new int[(int)size.X, (int)size.Y, (int)size.Z];
            for(int x = 0; x < size.X; x++)
            {
                for(int y = 0; y < size.Y; y++)
                {
                    fullmap[x, y, 0] = map[x, y];
                }
            }
            return fullmap;
        }
    }
}
