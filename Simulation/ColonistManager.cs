using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class ColonistManager
    {
        List<Colonist> colonists = new List<Colonist>();
        Colonist selectedcol;
        Area area;
        public ColonistManager(Area area)
        {
            this.area = area;
        }
        public void Update()
        {
            GetColonists();
            int xtile = Input.MouseTileX();
            int ytile = Input.MouseTileY();

            if(Input.IsMouseButtonPressed(0))
            {
                selectedcol = null;
                foreach(Colonist c in colonists)
                {
                    if(c.pos.Equals(new XY(xtile, ytile)))
                    {
                        selectedcol = c;
                        break;
                    }
                }
            }
            if(Input.IsMouseButtonPressed(1) && selectedcol != null)
            {
                Goal g = new Goal();
                g.destination = new XY(xtile, ytile);
                Entity plant = area.GetEntity(g.destination, "Plant");
                if (plant != null)
                {
                    g.goaltype = GoalType.HARVESTSEEDS;
                } else
                {
                    g.goaltype = GoalType.TRAVEL;
                }
                selectedcol.goals.Add(g);
            }
        }

        void GetColonists()
        {
            Area area = Simulation.inst.area;
            var ent = area.entities;
            foreach(Entity e in ent)
            {
                var c = e as Colonist;
                if (c != null)
                    colonists.Add(c);
            }
        }
    }
}
