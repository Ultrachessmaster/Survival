﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class Menu
    {
        Texture2D tex;
        List<int> tiles;
        int screenheight;
        public int NumTiles { get { return tiles.Count; } }

        public Menu (List<int> tiles, int screenheight)
        {
            this.tiles = tiles;
            this.screenheight = screenheight;
            tex = Simulation.CM.Load<Texture2D>("texturemap");
        }

        public void Draw(SpriteBatch sb)
        {
            for(int i = 0; i < tiles.Count; i++)
            {
                int idx = tiles[i];
                var yindex = screenheight - (Simulation.tilesize * 2);

                Rectangle destrect = new Rectangle(i * Simulation.tilesize * 2, yindex, Simulation.tilesize * 2, Simulation.tilesize * 2);

                int xsource = (idx % (tex.Width / Simulation.tilesize)) * Simulation.tilesize;
                int ysource = (int)Math.Floor((decimal)(idx) / (tex.Width / Simulation.tilesize)) * Simulation.tilesize;

                Rectangle sourcerect = new Rectangle(xsource, ysource, Simulation.tilesize, Simulation.tilesize);

                sb.Draw(tex, destrect, sourcerect, Color.White);
            }
        }

        public int TileSelected(int idx)
        {
            return tiles[idx];
        }
    }
}