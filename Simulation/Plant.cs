using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class Plant : Entity
    {
        public float health = 24 * 10f;
        public const float maxhealth = 24 * 10f;
        Area area;
        public Plant(XY pos, Area area)
        {
            this.pos = pos;
            draw = Drw;
            update = Upd;
            Sprite = 4;
            this.area = area;
            tag = "Plant";
            SetHealth();
            GetDescription = GenerateDescription;
            Timer t = new Timer(Action, 1f, enabled);
        }

        void SetHealth()
        {
            float scale = 1 - area.EntitiesSurrounding(pos.X, pos.Y, 0, "Plant") / 8f;
            health = scale * maxhealth;
        }

        void Upd (GameTime gt)
        {
            if(health <= 0)
            {
                enabled = new RefWrapper<bool>(false);
            }
        }

        string GenerateDescription ()
        {
            StringBuilder sb = new StringBuilder("---Plant---\n");
            sb.AppendLine("Health: " + health + " / " + maxhealth);
            return sb.ToString();
        }

        void Action(float timelost)
        {
            health--;
            Timer t = new Timer(Action, 1f, enabled);
        }

    }
}
