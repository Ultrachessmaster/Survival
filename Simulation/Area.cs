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
        private List<Entity> entities = new List<Entity>();
        private List<Entity> closeentities = new List<Entity>();

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
                for(int j = 0; j < ColonistManager.NumColonists(); j++)
                {
                    var colon = ColonistManager.colonists[i];
                    var dist = XY.Distance(colon.pos, ent.pos);
                    if (dist < Colonist.longestdist)
                        closeentities.Add(ent);
                }
            }
        }
        public void Draw(SpriteBatch sb, int pxlratio, int tilesize, Texture2D spriteatlas, Texture2D animalatlas, Color color)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                var ent = entities[i];
                ent.Draw(sb, pxlratio, tilesize, spriteatlas, animalatlas, color);
            }
        }
        public int TilesSurrounding(int x, int y, int z, Tile type)
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
        public int EntitiesSurrounding(int x, int y, int z, string tag)
        {
            int count = 0;
            foreach (Entity e in entities)
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
            for (int i = 0; i < closeentities.Count; i++)
            {
                var e = closeentities[i];
                if (e.pos.Equals(pos) && e.Tag == tag)
                    return e;
            }
            for (int i = 0; i < entities.Count; i++)
            {
                var e = entities[i];
                if (e.pos.Equals(pos) && e.Tag == tag)
                    return e;
            }
            return null;
        }

        public Entity GetEntity(XY pos)
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

        public List<Entity> GetEntities (string tag)
        {
            var ents = new List<Entity>();
            for(int i = 0; i < entities.Count; i++)
            {
                if (entities[i].Tag == tag)
                    ents.Add(entities[i]);
            }
            return ents;
        }
        public void AddEntity(Entity e)
        {
            entities.Add(e);
        }
        public void RemoveEntity(Entity e)
        {
            entities.Remove(e);
        }
        public void AddRangeE(List<Entity> e)
        {
            entities.AddRange(e);
        }
    }
}
