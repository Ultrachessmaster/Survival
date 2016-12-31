using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation
{
    public class Area
    {
        public int[,,] tiles;
        public List<Entity> entities = new List<Entity>();
        public Area (int[,,] t)
        {
            tiles = t;
        }
        public void Upd()
        {
            
        }
        public int NumberSurrounding(int x, int y, int z, int type)
        {
            int count = 0;
            for(int i = x - 1; i < x + 2; i++)
            {
                for (int j = y - 1; j < y + 2; j++)
                {
                    if (i == x && j == y)
                        continue;
                    if (i < 0 || j < 0)
                        continue;
                    if (i >= tiles.GetUpperBound(0) + 1 || j >= tiles.GetUpperBound(1) + 1 || z >= tiles.GetUpperBound(2) + 1)
                        continue;
                    if (tiles[i, j, z] == type)
                        count++;
                }
            }
            return count;
        }
       
    }
}
