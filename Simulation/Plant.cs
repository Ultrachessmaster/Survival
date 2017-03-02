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
        public float health = 24 * 3f;
        public const float maxhealth = 24 * 3f;
        public Plant(XY pos)
        {
            this.pos = pos;
            draw = Drw;
            update = Upd;
            Sprite = 4;
            tag = "Plant";
            float scale = 1 - Area.EntitiesSurrounding(pos.X, pos.Y, 0, "Plant") / 8f;
            health = scale * maxhealth;
            GetDescription = GenerateDescription;
            Timer t = new Timer(Action, 1f, enabled);
        }

        void Upd (GameTime gt)
        {
            if(health <= 0)
            {
                enabled.Value = false;
                Random r = new Random();
                for(int i = 0; i < 3; i++)
                {
                    var dir = new XY(r.Next(-5, 6), r.Next(-5, 6));
                    if (Area.CanWalk(pos + dir, 0))
                    {
                        Area.AddEntity(new Plant(pos + dir));
                    }
                }
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
            health -= 4;
            Timer t = new Timer(Action, 1f, enabled);
        }

    }
}
