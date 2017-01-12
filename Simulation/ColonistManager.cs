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
                switch(Interaction.CurrentTool)
                {
                    case Tool.Hoe:
                        g.goaltype = GoalType.TILLGROUND;
                        break;
                    case Tool.Grab:
                        if(area.tiles[xtile, ytile, 0] == Tile.Crop)
                            g.goaltype = GoalType.HARVESTCROPS;
                        else
                            g.goaltype = GoalType.HARVESTSEEDS;
                        
                        break;
                    case Tool.Walk:
                        g.goaltype = GoalType.TRAVEL;
                        break;
                    case Tool.PlantSeed:
                        g.goaltype = GoalType.PLANTSEEDS;
                        break;
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
