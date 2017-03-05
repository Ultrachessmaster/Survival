using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;

namespace Simulation
{
    class Inventory
    {
        public static Dictionary<ItemType, int> items = new Dictionary<ItemType, int>();
        public static List<ItemType> craftinglist = new List<ItemType>();
        static List<InventoryRecipe> inventoryrecipes = new List<InventoryRecipe>();
        public static List<FurnaceRecipe> furnacerecipes = new List<FurnaceRecipe>();
        static Dictionary<ItemType, Button> btns = new Dictionary<ItemType, Button>();
        public const int xpos = 540;
        public const int ypos = 833;
        public const int craftingoffset = 155;
        public static ItemType selecteditem = ItemType.None;

        public static void LoadCraftingRecipes()
        {
            string recipetext = File.ReadAllText("craftingrecipes.json");
            JObject jo = JObject.Parse(recipetext);
            var invrecipes = jo.First.First;
            var furnrecipes = jo.Last.First;
            foreach(JToken recipe in invrecipes.Children())
            {
                InventoryRecipe r = new InventoryRecipe();
                r.product = (ItemType)recipe.Last.First.Value<int>();
                var itemids = recipe.First.Children().Values<int>();
                List<ItemType> its = new List<ItemType>();
                foreach(int itemid in itemids)
                {
                    its.Add((ItemType)itemid);
                }
                r.items = its;
                inventoryrecipes.Add(r);
            }
            foreach(JToken recipe in furnrecipes.Children())
            {
                FurnaceRecipe fr = new FurnaceRecipe();
                fr.cookingitem = (ItemType)recipe.First.First.Value<int>();
                fr.product = (ItemType)recipe.Last.First.Value<int>();
                furnacerecipes.Add(fr);
            }

        }

        public static void Craft(int ignore)
        {
            foreach(InventoryRecipe r in inventoryrecipes)
            {
                bool isrecipe = true;
                if (r.items.Count != craftinglist.Count)
                    continue;
                for(int i = 0; i < r.items.Count; i++)
                {
                    if(r.items[i] != craftinglist[i])
                        isrecipe = false;
                }
                if (isrecipe)
                {
                    AddItem(r.product);
                    craftinglist.Clear();
                    return;
                }
            }
        }

        static public void AddItem(ItemType item)
        {
            if (items.ContainsKey(item))
                items[item]++;
            else
            {
                items.Add(item, 1);
                Button b = new Button(new XY(160, 18), new XY(xpos, ypos + (items.Count - 1) * 18), SelectItem, AddCraftingItem, 0, (int)item);
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
            selecteditem = (ItemType)it;
            Interaction.CurrentTool = Tool.None;
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
            if (selecteditem == ItemType.None)
                return;
            if(btns.ContainsKey(selecteditem))
            {
                XY pos = btns[selecteditem].pos;
                XY size = btns[selecteditem].size;
                sb.Draw(tex, new Rectangle(pos.X, pos.Y, size.X, size.Y), Color.Gold);
            }
        }
    }
}
