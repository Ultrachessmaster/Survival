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

        public Animal(XY pos, Color color)
        {
            this.pos = pos;
            tex = TextureAtlas.ANIMALS;
            draw = Dr;
            //update = Upd;
            Timer t = new Timer(Upd, 1);
            col = color;
            
        }

        public void Dr(SpriteBatch sb, int pxlratio, Texture2D tex, Rectangle sourcerec, Color color)
        {
            Drw(sb, pxlratio, tex, sourcerec, col);
        }

        public void Upd (float ot)
        {
            pos += Direction();
            Timer t = new Timer(Upd, 1);
        }

        public XY Direction()
        {
            Random r = new Random((int)DateTime.Now.Ticks * 2);
            var dir = new XY(r.Next(-1, 2), r.Next(-1, 2));
            return dir;
        }


    }
}
