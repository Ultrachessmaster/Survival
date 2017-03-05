using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class Furnace : Entity
    {
        public Furnace(XY pos)
        {
            draw = Drw;
            Timer t = new Timer(Upd, 0.001f, enabled);
            tag = "Furnace";
            this.pos = pos;
            tex = TextureAtlas.ITEMS;
            Sprite = (int)ItemType.Furnace;
        }

        public void Upd(float ot)
        {
            var items = Area.GetEntities("Item");
            Item fuel = null;
            Item cookingitem = null;
            foreach(Item it in items)
            {
                if (it.burnable && it.pos == pos + new XY(0, -1))
                    fuel = it;
                if (it.pos == pos + new XY(-1, 0))
                    cookingitem = it;
            }
            if (fuel != null && cookingitem != null)
            {
                foreach(FurnaceRecipe fr in Inventory.furnacerecipes)
                {
                    if(fr.cookingitem == cookingitem.itemtype)
                    {
                        Area.AddEntity(new Item(pos + new XY(1, 0), fr.product));
                        Area.RemoveEntity(fuel);
                        Area.RemoveEntity(cookingitem);
                    }
                }
            }
            Timer t = new Timer(Upd, 0.001f, enabled);
        }
    }
}
