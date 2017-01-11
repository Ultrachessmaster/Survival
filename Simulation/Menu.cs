using Microsoft.Xna.Framework;
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
        List<int> tools;
        int screenheight;
        public int NumTiles { get { return tools.Count; } }

        public Menu (List<int> tools, int screenheight)
        {
            this.tools = tools;
            this.screenheight = screenheight;
            tex = Simulation.CM.Load<Texture2D>("toolmap");
        }

        public void Draw(SpriteBatch sb)
        {
            for(int i = 0; i < tools.Count; i++)
            {
                int idx = tools[i];
                var yindex = screenheight - (Simulation.tilesize * 2);

                Rectangle destrect = new Rectangle(i * Simulation.tilesize * 2 + 2, yindex - 2, Simulation.tilesize * 2, Simulation.tilesize * 2);

                int xsource = (idx % (tex.Width / Simulation.tilesize)) * Simulation.tilesize;
                int ysource = (int)Math.Floor((decimal)(idx) / (tex.Width / Simulation.tilesize)) * Simulation.tilesize;

                Rectangle sourcerect = new Rectangle(xsource, ysource, Simulation.tilesize, Simulation.tilesize);

                sb.Draw(tex, destrect, sourcerect, Color.White);
            }
        }

        public int ToolSelected(int idx)
        {
            return tools[idx];
        }
    }
}
