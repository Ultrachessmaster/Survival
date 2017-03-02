using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class Vulpes : Entity, IAnimal
    {
        Area area;
        float speed = 0.003f;
        float blood = 100;
        const float maxblood = 100;
        float bloodlossrate = 0;
        float damage = 0.01f;

        public float Intimidation { get { return intimidation; } }
        float intimidation = 8;
        public float Satiation { get { return satiation / 2; } }
        float satiation = 24 * 9;
        const float maxsatiation = 24 * 9;
        float movesatiationcost = 0.21f;
        public bool Dead { get { return dead; } }
        bool dead = false;

        

        public Vulpes(XY pos, Area area)
        {
            this.pos = pos;
            tex = TextureAtlas.ANIMALS;
            tag = "Vulpes";
            update = Upd;
            Sprite = 0;
            draw = Drw;
            Timer t = new Timer(Move, speed, enabled);
            Timer t1 = new Timer(Weaken, 1f, enabled);
            this.area = area;
            GetDescription = GenerateDescription;
        }

        public void Move(float ot)
        {
            var animals = Area.GetEntities<IAnimal>();
            bool preyfound = false;
            foreach (Entity e in animals)
            {
                if (e.Tag == "Vulpes")
                    continue;
                if ((e.pos - pos).Magnitude() <= 15)
                {
                    var animal = (e as IAnimal);
                    if(animal.Intimidation < 6 + (satiation / maxsatiation) * 4)
                    {
                        preyfound = true;
                        if ((e.pos - pos).Magnitude() <= 1)
                        {
                            animal.TakeDamage(damage);
                            if (animal.Dead)
                            {
                                satiation += animal.Satiation;
                                e.enabled.Value = false;
                            }
                                
                            
                        }
                            
                        var difference = (e.pos - pos);
                        XY movement = new XY();
                        if(Math.Abs(difference.X) > Math.Abs(difference.Y))
                            movement = new XY(Math.Sign(difference.X), 0);
                        else
                            movement = new XY(0, Math.Sign(difference.Y));
                        if (Area.CanWalk(pos + movement, 0))
                        {
                            pos += movement;
                            satiation -= movesatiationcost;
                            break;
                        }
                    }
                }
            }
            if(!preyfound)
            {
                Random r = new Random();
                XY dir = new XY(r.Next(-1, 2), r.Next(-1, 2));
                if (Area.CanWalk(pos + dir, 0))
                {
                    pos += dir;
                    satiation -= movesatiationcost;
                }
                    
            }

            Timer t = new Timer(Move, speed, enabled);

        }

        public void Upd(GameTime gt)
        {
            blood -= bloodlossrate;
            if (blood <= 0)
                enabled.Value = false;
            satiation = Math.Min(satiation, maxsatiation);
            satiation = Math.Max(satiation, 0);
            if (satiation <= 0)
                enabled.Value = false;
        }

        public void TakeDamage(float bloodlossrate)
        {
            this.bloodlossrate += bloodlossrate;
        }

        public void Weaken (float ot)
        {
            satiation--;
            Timer t1 = new Timer(Weaken, 1f, enabled);
        }

        string GenerateDescription()
        {
            StringBuilder sb = new StringBuilder("--- Vulpes ---\n");
            sb.AppendLine("Satiation: " + Math.Round(satiation) + " / " + maxsatiation);
            sb.AppendLine("Satiation: " + Math.Round(blood) + " / " + maxblood);
            return sb.ToString();
        }
    }
}
