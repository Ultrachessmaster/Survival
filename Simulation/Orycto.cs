using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class Orycto : Entity, IAnimal
    {
        public float Intimidation { get { return intimidation; } }
        float intimidation = 2;
        public float Satiation { get { return satiation; } }
        float satiation = 24 * 5;
        public bool Dead { get { return dead; } }
        bool dead = false;

        float maxsatiation = 24 * 5;
        float blood = 60;
        const float maxblood = 60;
        float bloodlossrate = 0;
        float speed = 0.002f;
        XY panicdir;

        public Orycto (XY pos)
        {
            this.pos = pos;
            tag = "Orycto";
            tex = TextureAtlas.ANIMALS;
            Sprite = 1;
            draw = Drw;
            update = Upd;
            GetDescription = GenerateDescription;
            Timer t = new Timer(Move, speed, enabled);
            Timer t1 = new Timer(Weaken, 1f, enabled);
        }

        public void Upd (GameTime gt)
        {
            blood -= bloodlossrate;
            satiation = Math.Min(satiation, maxsatiation);
            satiation = Math.Max(satiation, 0);
            if (blood <= 0 || satiation <= 0)
                dead = true;
        }

        public void Move(float ot)
        {
            if(!dead)
            {
                Random r = new Random();
                var plants = Area.GetEntities("Plant");
                Plant plant = null;
                var full = satiation > maxsatiation * 2f / 3f;
                if(!full)
                {
                    foreach (Plant p in plants)
                    {
                        var distance = (p.pos - pos).Magnitude();
                        if (distance <= 15)
                            plant = p;
                        if (distance <= 1)
                        {
                            p.enabled.Value = false;
                            satiation += 30;
                        }
                    }
                }
                if (panicdir == XY.Zero)
                {
                    if(plant != null)
                    {
                        var difference = (plant.pos - pos);
                        if (Math.Abs(difference.X) > Math.Abs(difference.Y))
                            pos += new XY(Math.Sign(difference.X), 0);
                        else
                            pos += new XY(0, Math.Sign(difference.Y));
                    } else
                    {
                        var direction = new XY(r.Next(-1, 2), r.Next(-1, 2));
                        if (Area.CanWalk(pos + direction, 0))
                        {
                            pos += direction;
                            satiation -= 0.22f;
                        }
                    }
                }
                else
                {
                    if (Area.CanWalk(pos + panicdir, 0))
                    {
                        satiation -= 0.18f;
                        pos += panicdir;
                    }
                }
                if (full && panicdir == XY.Zero)
                {
                    Timer t = new Timer(Move, speed * 35f, enabled);
                } else
                {
                    Timer t = new Timer(Move, speed, enabled);
                }
            }
        }

        public void Weaken(float ot)
        {
            satiation -= 0.5f;
            Timer t = new Timer(Weaken, 1f, enabled);
        }

        public void TakeDamage(float bloodlossrate)
        {
            this.bloodlossrate += bloodlossrate;
            if(panicdir == XY.Zero)
            {
                Random r = new Random();
                panicdir = new XY(r.Next(-1, 2), r.Next(-1, 2));
                Timer t = new Timer(CalmDown, 0.5f, enabled);
            }
        }

        public void CalmDown(float ot) { panicdir = XY.Zero; bloodlossrate = 0; }

        string GenerateDescription()
        {
            StringBuilder sb = new StringBuilder("--- Orycto ---\n");
            sb.AppendLine("Satiation: " + Math.Round(satiation) + " / " + maxsatiation);
            sb.AppendLine("Blood: " + Math.Round(blood) + " / " + maxblood);
            return sb.ToString();
        }
    }
}
