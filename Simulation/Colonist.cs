using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simulation
{
    class Colonist : Entity, IAnimal
    {
        float satiation = 24 * 12;
        float maxsatiation = 24 * 12;
        float water = 24 * 8;
        float maxwater = 24 * 8;
        float blood = 100;
        float maxblood = 100;
        public float bloodlossrate = 0;

        public float Intimidation { get { return intimidation; } }
        float intimidation = 10;
        public float Satiation { get { return satiation / 2; } }
        public bool Dead { get { return dead; } }
        bool dead = false;

        public const int longestdist = 100;
        const float slowness = 0.001f;
        float movementfoodcost = 0.5f;
        float plantingfoodcost = 0.5f;
        public List<Goal> goals = new List<Goal>();
        bool[,] walkmap;
        List<XY> path;
        XY origin;
        public static XY home;
        public bool selected = false;
        Area area;

        public Colonist (XY pos, Area area)
        {
            this.pos = pos;
            this.area = area;
            Sprite = 0;
            draw = Drw;
            update = upd;
            tag = "Colonist";
            tex = TextureAtlas.SPRITES;
            origin = pos;
            Timer t = new Timer(Action, slowness, enabled);
            Timer t1 = new Timer(Weaken, 1f, enabled);
            GetDescription = UpdateDescription;
        }

        string UpdateDescription()
        {
            StringBuilder sb = new StringBuilder("--- Colonist ---\n");
            sb.AppendLine("Satiation: " + Math.Round(satiation) + " / " + maxsatiation);
            sb.AppendLine("Water: " + Math.Round(water) + " / " + maxwater);
            sb.AppendLine("Blood: " + Math.Round(blood) + " / " + maxblood);
            return sb.ToString();
        }

        public void upd (GameTime g)
        {
            blood -= bloodlossrate;
            satiation = Math.Min(satiation, maxsatiation);
            satiation = Math.Max(satiation, 0);
            if (satiation <= 0 || water <= 0 || blood <= 0)
                enabled = new RefWrapper<bool>(false);
        }

        void Weaken(float overtime)
        {
            satiation--;
            water -= 4;
            Timer t = new Timer(Weaken, 1f, enabled);
        }

        void Action (float overtime)
        {
            if(!selected && goals.Count == 0)
            {
                var crops = Area.GetEntities("Crop");
                var plants = Area.GetEntities("Plant");
                for(int i = 0; i < crops.Count; i++)
                {
                    var crop = crops[i];
                    if(XY.Distance(crop.pos, home) < longestdist / 4)
                    {
                        Goal g = new Goal();
                        g.goaltype = GoalType.GOAL;
                        g.destination = crop.pos;
                        goals.Add(g);
                    }
                }

                for (int i = 0; i < plants.Count; i++)
                {
                    var plant = plants[i];
                    if (XY.Distance(plant.pos, home) < longestdist / 4)
                    {
                        Goal g = new Goal();
                        g.goaltype = GoalType.GOAL;
                        g.destination = plant.pos;
                        goals.Add(g);
                    }
                }

            }
            if(goals.Count == 0)
            {
                if(!pos.Equals(home))
                {
                    Goal g = new Goal();
                    g.goaltype = GoalType.GOAL;
                    g.destination = home;
                    goals.Add(g);
                } else
                {
                    Timer t0 = new Timer(Action, slowness, enabled);
                    return;
                }
            }
            Goal goal = goals.First();
            bool atpoint = (pos - goal.destination).Magnitude() <= 1;
            if(!pos.Equals(goal.destination) && !atpoint)
                FollowPath();
            else
            {
                Entity e = Area.GetEntity(goal.destination);
                if (e != null)
                {
                    if (e.Tag == "Plant")
                    {
                        Inventory.AddItem(ItemType.Seed);
                        Area.RemoveEntity(e);
                    }
                    if (e.Tag == "Crop" && e.Sprite == 3)
                    {
                        Inventory.AddItem(ItemType.Crop);
                        Inventory.AddItem(ItemType.Seed);
                        Inventory.AddItem(ItemType.Seed);
                        Area.RemoveEntity(e);
                    }
                    if (e.Tag == "Item")
                    {
                        var item = (e as Item);
                        Inventory.AddItem(item.itemtype);
                        Area.RemoveEntity(e);
                    }
                }
                switch (goal.goaltype)
                {
                    case GoalType.TILLGROUND:
                        if(Area.tiles[goal.destination.X, goal.destination.Y, 0] == Tile.Vegetation || Area.tiles[goal.destination.X, goal.destination.Y, 0] == Tile.Dirt)
                            Area.tiles[goal.destination.X, goal.destination.Y, 0] = Tile.TilledLand;
                        goals.RemoveAt(0);
                        break;
                    case GoalType.MINE:
                        switch(Area.tiles[goal.destination.X, goal.destination.Y, 0])
                        {
                            case Tile.Stone:
                                Area.tiles[goal.destination.X, goal.destination.Y, 0] = Tile.Dirt;
                                Inventory.AddItem(ItemType.Stone);
                                break;
                            case Tile.Copper:
                                Area.tiles[goal.destination.X, goal.destination.Y, 0] = Tile.Dirt;
                                Inventory.AddItem(ItemType.Copper_Ore);
                                break;
                            case Tile.Iron:
                                Area.tiles[goal.destination.X, goal.destination.Y, 0] = Tile.Dirt;
                                Inventory.AddItem(ItemType.Iron_Ore);
                                break;
                            case Tile.Coal:
                                Area.tiles[goal.destination.X, goal.destination.Y, 0] = Tile.Dirt;
                                Inventory.AddItem(ItemType.Coal);
                                break;
                            case Tile.Tin:
                                Area.tiles[goal.destination.X, goal.destination.Y, 0] = Tile.Dirt;
                                Inventory.AddItem(ItemType.Tin_Ore);
                                break;
                        }
                        goals.RemoveAt(0);
                        break;
                    case GoalType.ITEM:
                        ItemType it = Inventory.selecteditem;
                        switch(it)
                        {
                            case ItemType.Seed:
                                if (Area.tiles[goal.destination.X, goal.destination.Y, 0] == Tile.TilledLand)
                                {
                                    Area.AddEntity(new Crop(goal.destination));
                                    Inventory.RemoveItem(ItemType.Seed);
                                    satiation -= plantingfoodcost;
                                }
                                break;
                            case ItemType.Stone_Bucket:
                                if (Area.tiles[goal.destination.X, goal.destination.Y, 0] == Tile.Water)
                                {
                                    Area.tiles[goal.destination.X, goal.destination.Y, 0] = Tile.Dirt;
                                    Inventory.RemoveItem(ItemType.Stone_Bucket);
                                    Inventory.AddItem(ItemType.Water_Bucket);
                                }
                                break;
                            case ItemType.Water_Bucket:
                                Inventory.RemoveItem(ItemType.Water_Bucket);
                                Inventory.AddItem(ItemType.Stone_Bucket);
                                water += 80;
                                water = Math.Min(water, maxwater);
                                break;
                            case ItemType.Crop:
                                Inventory.RemoveItem(ItemType.Crop);
                                satiation += 30;
                                satiation = Math.Min(satiation, maxsatiation);
                                break;
                            case ItemType.Furnace:
                                Inventory.RemoveItem(ItemType.Furnace);
                                Area.AddEntity(new Furnace(goal.destination));
                                break;
                            case ItemType.None:
                                break;
                            default:
                                Area.AddEntity(new Item(goal.destination, it));
                                Inventory.RemoveItem(it);
                                break;
                        }
                        if (Inventory.ItemCount(Inventory.selecteditem) == 0)
                            Inventory.selecteditem = ItemType.None;
                        goals.RemoveAt(0);
                        break;
                    case GoalType.SHOVEL:
                        goals.RemoveAt(0);
                        break;
                    case GoalType.GOAL:
                        goals.RemoveAt(0);
                        break;
                }
                walkmap = null;
            }
            Timer t = new Timer(Action, slowness, enabled);
        }

        void FollowPath()
        {
            var dest = goals.First().destination;
            if (pos.Equals(dest))
                return;
            if(walkmap == null)
            {
                int xlower = pos.X - longestdist;
                int ylower = pos.Y - longestdist;
                int xhigher = pos.X + longestdist;
                int yhigher = pos.Y + longestdist;
                bool[,] map = new bool[longestdist * 2, longestdist * 2];
                for (int x = 0; x < longestdist * 2; x++)
                {
                    for (int y = 0; y < longestdist * 2; y++)
                    {
                        var xd = pos.X - longestdist + x;
                        var yd = pos.Y - longestdist + y;
                        map[x, y] = Area.CanWalk(new XY(xd, yd), 0);
                    }
                }

                walkmap = map;
                
                var localdest = -pos + longestdist + dest;
                AStar a = new AStar(new XY(longestdist, longestdist), localdest, map);
                path = a.CalculatePath();
                origin = pos;
                if (path == null)
                {
                    goals.RemoveAt(0);
                    return;
                }
                Assert.IsTrue(path.Count != 0);
            }

            Assert.IsTrue(path.Count != 0);

            var nearestdest = path.First();
            path.RemoveAt(0);
            nearestdest += origin;
            nearestdest -= longestdist;

            XY d = nearestdest - pos;
            Assert.IsTrue(Math.Abs(d.X) + Math.Abs(d.Y) <= 1);

            Move(nearestdest);
        }

        void Move(XY dest)
        {
            Area area = Simulation.inst.area;
            XY dir = (dest - pos);
            Tile tile = Area.tiles[(pos + dir).X, (pos + dir).Y, 0];
            if (tile != Tile.Water && tile != Tile.TinWall && tile != Tile.River && dir != XY.Zero)
                TakeMove(dir);
            else throw new Exception("Colonist did not move.");
        }

        void TakeMove(XY dir)
        {
            pos += dir;
            satiation -= movementfoodcost;
        }

        public void ClearGoals()
        {
            goals.Clear();
            path.Clear();
            walkmap = null;
        }

        public void TakeDamage(float bloodlossrate)
        {
            this.bloodlossrate += bloodlossrate;
        }

    }
}
