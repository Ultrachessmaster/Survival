using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class Crop : Entity
    {
        const int growtime = 4;
        public Crop (XY pos)
        {
            this.pos = pos;
            Sprite = 2;
            tag = "Crop";
            draw = Drw;
            if(TimeCycle.Hours > 4)
                new Timer(Grow, TimeCycle.Hours - growtime);
            else
                new Timer(Grow, 24 - TimeCycle.Hours - growtime);
            GetDescription = GenerateDescription;
        }

        public void Grow(float overtime)
        {
            Sprite = 3;
        }

        string GenerateDescription()
        {
            return "--- Crop ---";
        }
    }
}
