using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class Menu
    {
        Texture2D tex;
        Texture2D selectionbox;
        List<Button> tools = new List<Button>();
        Button clearcraftinglist;
        Button craft;
        int screenheight;
        public Tool tool = Tool.Hoe;

        public Menu (int screenheight)
        {
            XY size = new XY(2 * Simulation.tilesize, 2 * Simulation.tilesize);
            XY pos = new XY(2, screenheight - 2 - size.Y);
            XY offset = new XY(2 * Simulation.tilesize, 0);
            tools.Add(new Button(size, pos, SetTool, (int)Tool.Hoe, (int)Tool.Hoe));
            tools.Add(new Button(size, pos += offset, SetTool, (int)Tool.Shovel, (int)Tool.Shovel));
            tools.Add(new Button(size, pos += offset, SetTool, (int)Tool.Pickaxe, (int)Tool.Pickaxe));
            this.screenheight = screenheight;
            tex = Simulation.CM.Load<Texture2D>("toolmap");
            selectionbox = Simulation.CM.Load<Texture2D>("selection");
            clearcraftinglist = new Button(size, new XY(700, 930), Inventory.ClearCraftingList, 6, 0);
            craft = new Button(size, new XY(700 + size.X + 4, 930), Inventory.Craft, 7, 0);
        }

        public void Update()
        {
            foreach(Button b in tools)
            {
                b.Update();
            }
            clearcraftinglist.Update();
            craft.Update();
        }

        public void Draw(SpriteBatch sb)
        {
            for(int i = 0; i < tools.Count; i++)
            {
                Button btn = tools[i];
                btn.Draw(sb, tex);
                var yindex = screenheight - (Simulation.tilesize * 2);
                Rectangle destrect = new Rectangle(i * Simulation.tilesize * 2 + 2, yindex - 2, Simulation.tilesize * 2, Simulation.tilesize * 2);

                if (btn.id == (int)tool)
                {
                    sb.Draw(selectionbox, destrect, Color.Gold);
                }
            }
            clearcraftinglist.Draw(sb, tex);
            craft.Draw(sb, tex);
        }

        void SetTool(int i)
        {
            tool = (Tool)i;
            Inventory.selecteditem = ItemType.None;
        }
    }
}
