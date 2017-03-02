using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    public class Item : Entity
    {
        public ItemType itemtype;
        public bool burnable;
        public Item(XY pos, ItemType it)
        {
            this.pos = pos;
            tag = "Item";
            draw = Drw;
            Sprite = (int)it;
            tex = TextureAtlas.ITEMS;
            itemtype = it;
            switch(it)
            {
                case ItemType.Coal:
                    burnable = true;
                    break;
                case ItemType.Crop:
                    burnable = true;
                    break;
            }
        }
    }
}
