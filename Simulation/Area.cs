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
        public static Tile[,,] tiles;
        static private List<Entity> entities = new List<Entity>();

        public void SetMap(Tile[,,] t)
        {
            tiles = t;
        }
        public void Update(GameTime gameTime)
        {
            for (int i = entities.Count - 1; i >= 0; i--)
            {
                var ent = entities[i];
                if (ent.Update != null && ent.enabled.Value)
                    ent.Update.Invoke(gameTime);
                else if (!ent.enabled.Value)
                {
                    entities.RemoveAt(i);
                }
            }
        }
        public void Draw(SpriteBatch sb, int pxlratio, int tilesize, Texture2D[] atlas, Color color)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                var ent = entities[i];
                ent.Draw(sb, pxlratio, tilesize, atlas, color);
            }
        }
        public static int TilesSurrounding(int x, int y, int z, Tile type)
        {
            int count = 0;
            for (int i = x - 1; i < x + 2; i++)
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
        public static int EntitiesSurrounding(int x, int y, int z, string tag)
        {
            int count = 0;
            for(int i = 0; i < entities.Count; i++)
            {
                var e = entities[i];
                if (e.Tag != tag)
                    continue;
                bool correctx = (x <= e.pos.X + 1 && x >= e.pos.X - 1);
                bool correcty = (y <= e.pos.Y + 1 && y >= e.pos.Y - 1);
                if (correctx && correcty)
                    count++;
            }
            return count;
        }

        public static bool CanWalk(XY pos, int layer)
        {
            if (pos.X >= tiles.GetUpperBound(0) || pos.Y >= tiles.GetUpperBound(1) || pos.X < 0 || pos.Y < 0 || layer >= tiles.GetUpperBound(2) || layer < 0)
                return false;
            bool notwater = tiles[pos.X, pos.Y, layer] != Tile.Water;
            bool notwall = tiles[pos.X, pos.Y, layer] != Tile.PlatinumWall;
            bool notriver = tiles[pos.X, pos.Y, layer] != Tile.River;
            bool notstone = tiles[pos.X, pos.Y, layer] != Tile.Stone;
            bool notiron = tiles[pos.X, pos.Y, layer] != Tile.Iron;
            bool notcopper = tiles[pos.X, pos.Y, layer] != Tile.Copper;
            bool notcoal = tiles[pos.X, pos.Y, layer] != Tile.Coal;
            bool nottin = tiles[pos.X, pos.Y, layer] != Tile.Platinum;
            return notwater && notriver && notwall && notstone && notiron && notcopper && notcoal && nottin;
        }

        public static Entity GetEntity(XY pos, string tag)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                var e = entities[i];
                if (e.pos.Equals(pos) && e.Tag == tag)
                    return e;
            }
            return null;
        }

        public static Entity GetEntity(XY pos)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                var e = entities[i];
                if (e.pos.Equals(pos))
                {
                    return e;
                }
            }
            return null;
        }

        public static List<Entity> GetEntities (string tag)
        {
            var ents = new List<Entity>();
            for(int i = 0; i < entities.Count; i++)
            {
                if (entities[i].Tag == tag)
                    ents.Add(entities[i]);
            }
            return ents;
        }

        public static List<Entity> GetEntities(XY pos)
        {
            var ents = new List<Entity>();
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].pos == pos)
                    ents.Add(entities[i]);
            }
            return ents;
        }

        public static List<Entity> GetEntities<T>()
        {
            var ents = new List<Entity>();
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i] is T)
                    ents.Add(entities[i]);
            }
            return ents;
        }

        public static void AddEntity(Entity e)
        {
            entities.Add(e);
        }
        public static void RemoveEntity(Entity e)
        {
            entities.Remove(e);
        }
        public static void AddRangeE(List<Entity> e)
        {
            entities.AddRange(e);
        }
    }
}
