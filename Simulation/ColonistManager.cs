using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class ColonistManager
    {
        public static List<Colonist> colonists = new List<Colonist>();
        public Colonist SelectedColonist { get { return selectedcol; } }
        Colonist selectedcol;
        Area area;
        XY previousmousepos;
        public ColonistManager(Area area)
        {
            this.area = area;
        }
        public void Update()
        {
            GetColonists();
            int xtile = Input.MouseTileX();
            int ytile = Input.MouseTileY();
            var state = Mouse.GetState();
            int my = state.Y;

            if (Input.IsMouseButtonPressed(0) && my < (Simulation.windowsize - 128 * 2 ))
            {
                selectedcol = null;
                foreach(Colonist c in colonists)
                {
                    c.selected = false;
                    if(c.pos.Equals(new XY(xtile, ytile)))
                    {
                        selectedcol = c;
                        c.ClearGoals();
                        c.selected = true;
                        break;
                    }
                }
            }
            if(Input.IsMouseButtonPressed(1) && selectedcol != null && my < (Simulation.windowsize - 134))
            {
                XY mouse = new XY();
                mouse.X = Input.MouseTileX();
                mouse.Y = Input.MouseTileY();
                if (Input.IsMouseButtonPressed(1))
                    previousmousepos = mouse;
                
                Goal g = new Goal();
                g.destination = new XY(xtile, ytile);
                switch(Interaction.CurrentTool)
                {
                    case Tool.Hoe:
                        g.goaltype = GoalType.TILLGROUND;
                        break;
                    case Tool.Pickaxe:
                        g.goaltype = GoalType.MINE;
                        break;
                    case Tool.Shovel:
                        g.goaltype = GoalType.SHOVEL;
                        break;
                    case Tool.None:
                        g.goaltype = GoalType.ITEM;
                        break;
                }
                selectedcol.goals.Add(g);
            }
        }

        void GetColonists()
        {
            Area area = Simulation.inst.area;
            var ents = Area.GetEntities("Colonist");
            foreach (Entity e in ents)
                colonists.Add((Colonist)e);
        }

        public static int NumColonists()
        {
            return colonists.Count;
        }

        public void Draw(SpriteBatch sb, Texture2D tex)
        {
            Rectangle rect = new Rectangle((previousmousepos.X * Simulation.tilesize - Camera.X) * Simulation.pxlratio, (previousmousepos.Y * Simulation.tilesize - Camera.Y) * Simulation.pxlratio, Simulation.tilesize * Simulation.pxlratio, Simulation.tilesize * Simulation.pxlratio);
            sb.Draw(tex, rect, Color.Purple);
        }
    }
}
