using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class Inventory
    {
        public static Dictionary<ItemType, int> items = new Dictionary<ItemType, int>();
        public static List<ItemType> craftinglist = new List<ItemType>();
        static List<ItemType> bucketrecipe = new List<ItemType>();
        static Dictionary<ItemType, Button> btns = new Dictionary<ItemType, Button>();
        public const int xpos = 4;
        public const int ypos = 833;
        public static ItemType itemselected = ItemType.NONE;

        public static void Craft(int j)
        {
            bucketrecipe = new List<ItemType>();
            bucketrecipe.Add(ItemType.STONE);
            bucketrecipe.Add(ItemType.STONE);
            bucketrecipe.Add(ItemType.STONE);

            if (bucketrecipe.Count != craftinglist.Count)
                return;
            for(int i = 0; i <  craftinglist.Count; i++)
            {
                if (bucketrecipe[i] != craftinglist[i])
                    return;
            }
            AddItem(ItemType.STONEBUCKET);
            craftinglist = new List<ItemType>();
        }

        static public void AddItem(ItemType item)
        {
            if (items.ContainsKey(item))
                items[item]++;
            else
            {
                items.Add(item, 1);
                Button b = new Button(new XY(160, 18), new XY(xpos, ypos + (items.Count - 1) * 18), AddCraftingItem, SelectItem, 0, (int)item);
                btns.Add(item, b);
                ReOrderButtons();
            }
        }

        static void ReOrderButtons()
        {
            int i = 0;
            foreach (Button button in btns.Values)
            {
                button.pos.Y = ypos + i * 18;
                i++;
            }

        }

        public static void Update()
        {
            var buttons = new List<Button>(btns.Values);
            for(int i = buttons.Count - 1; i >= 0; i--)
                buttons[i].Update();
        }

        static void AddCraftingItem(int it)
        {
            ItemType itt = (ItemType)it;
            if(items[itt] > 0 && craftinglist.Count <= 4)
            {
                craftinglist.Add(itt);
                items[itt]--;
                if (items[itt] == 0)
                {
                    items.Remove(itt);
                    btns.Remove(itt);
                    ReOrderButtons();
                }
                    
            }
        }

        static void SelectItem(int it)
        {
            itemselected = (ItemType)it;
        }

        public static void ClearCraftingList(int i)
        {
            foreach(ItemType it in craftinglist)
            {
                AddItem(it);
            }
            craftinglist = new List<ItemType>();
        }

        public static int ItemCount(ItemType it)
        {
            if (items.ContainsKey(it))
                return items[it];
            return 0;
        }

        public static void RemoveItem(ItemType it)
        {
            if (items.ContainsKey(it))
            {
                items[it]--;
                if (items[it] == 0)
                {
                    items.Remove(it);
                    btns.Remove(it);
                    ReOrderButtons();
                }
            }
        }

        public static void Draw(SpriteBatch sb, Texture2D tex)
        {
            if (itemselected == ItemType.NONE)
                return;
            XY pos = btns[itemselected].pos;
            XY size = btns[itemselected].size;
            sb.Draw(tex, new Rectangle(pos.X, pos.Y, size.X, size.Y), Color.Gold);
        }
    }
}
