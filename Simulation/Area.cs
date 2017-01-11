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

        public void SetMap(int[,,] t)
        {
            tiles = t;
        }

        public void Upd()
        {
            
        }
        public int TilesSurrounding(int x, int y, int z, int type)
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
        public int EntitiesSurrounding(int x, int y, int z, string tag)
        {
            int count = 0;
            foreach(Entity e in entities)
            {
                bool correcttype = (e.Tag == tag);
                bool correctx = (x <= e.pos.X + 1 && x >= e.pos.X - 1);
                bool correcty = (y <= e.pos.Y + 1 && y >= e.pos.Y - 1);
                if (correcttype && correctx && correcty)
                    count++;
            }
            return count;
        }

        public Entity GetEntity(XY pos, string tag)
        {
            foreach (Entity e in entities)
            {
                if (e.Tag == tag && e.pos.Equals(pos))
                {
                    return e;
                }
            }
            return null;
        }
    }
}
