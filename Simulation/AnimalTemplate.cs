using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class AnimalTemplate
    {
        Color col;
        int sprite = 0;
        public AnimalTemplate(Color color, int sprite)
        {
            col = color;
            this.sprite = sprite;
        }

        public Animal Create(Vector2 pos)
        {
            Animal a = new Animal(pos, col);
            a.Sprite = sprite;
            return a;
        }
    }
}
