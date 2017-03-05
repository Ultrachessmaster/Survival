using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    public class InventoryRecipe
    {
        public List<ItemType> items = new List<ItemType>();
        public ItemType product;
    }
    public class FurnaceRecipe
    {
        public ItemType cookingitem;
        public ItemType product;
    }
}
