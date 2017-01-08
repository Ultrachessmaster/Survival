using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class Animal : Entity
    {
        Color col;

        public Animal(Vector2 pos, Color color)
        {
            this.pos = pos;
            tex = TextureAtlas.ANIMALS;
            draw = Dr;
            //update = Upd;
            Timer t = new Timer(Upd, 1);
            col = color;
            
        }

        public void Dr(SpriteBatch sb, int pxlratio, Texture2D tex, Rectangle sourcerec)
        {
            sb.Draw(tex, new Rectangle(((int)Math.Round(pos.X) - Camera.X) * pxlratio, ((int)Math.Round(pos.Y) - Camera.Y) * pxlratio, width * pxlratio, height * pxlratio), sourcerec, col);
        }

        public void Upd (float ot)
        {
            pos += Direction();
            Timer t = new Timer(Upd, 1);
        }

        public Vector2 Direction()
        {
            Random r = new Random((int)DateTime.Now.Ticks * 2);
            var dir = new Vector2(r.Next(-1, 2) * Simulation.tilesize, r.Next(-1, 2) * Simulation.tilesize);
            return dir;
        }


    }
}
