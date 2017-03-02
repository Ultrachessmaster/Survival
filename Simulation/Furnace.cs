using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class Furnace : Entity
    {
        ItemType fuel = ItemType.None;
        public Furnace(XY pos)
        {
            draw = Drw;
            tag = "Furnace";
            this.pos = pos;
            tex = TextureAtlas.ITEMS;
            Sprite = (int)ItemType.Furnace;
        }

        public void Upd()
        {
            var items = Area.GetEntities("Item");
            foreach(Item it in items)
            {
                if (it.burnable && it.pos == pos + new XY(-1, 0))
                {
                    Area.RemoveEntity(it);
                    fuel = it.itemtype;
                }
            }
        }
    }
}
