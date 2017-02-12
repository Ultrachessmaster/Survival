using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class Colonist : Entity
    {
        float satiation = 24 * 12;
        float maxsatiation = 24 * 12;
        float water = 24 * 8;
        float maxwater = 24 * 8;
        public const int longestdist = 100;
        const float slowness = 0.001f;
        float movementfoodcost = 0.5f;
        float plantingfoodcost = 0.5f;
        public List<Goal> goals = new List<Goal>();
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
            Timer t = new Timer(Action, slowness, enabled);
            Timer t1 = new Timer(Weaken, 1f, enabled);
            GetDescription = UpdateDescription;
        }

        string UpdateDescription()
        {
            StringBuilder sb = new StringBuilder("--- Colonist ---\n");
            sb.AppendLine("Satiation: " + satiation + " / " + maxsatiation);
            sb.AppendLine("Water: " + water + " / " + maxwater);
            return sb.ToString();
        }

        public void upd (GameTime g)
        {
            if (satiation <= 0 || water <= 0)
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
                int x = 0, y = 0;
                int dx = 0, dy = -1;
                for (int i = 0; i < Math.Pow(longestdist / 4, 2); i++)
                {
                    XY p = new XY(x + pos.X, y + pos.Y);
                    if (XY.Distance(p, home) > longestdist / 4)
                        break;

                    Entity crop = area.GetEntity(p, "Crop");
                    Entity plant = area.GetEntity(p, "Plant");
                    if (crop != null || plant != null)
                    {
                        Goal g = new Goal();
                        g.goaltype = GoalType.GOAL;
                        g.destination = p;
                        goals.Add(g);
                        break;
                    }

                    if(x == y || (x < 0 && y == -x) || (x > 0 && x == (-y + 1)))
                    {
                        int pdx = dx;
                        dx = -dy;
                        dy = pdx;
                    }
                    x += dx;
                    y += dy;
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
                GoTo(goal.destination);
            else
            {
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
                                Inventory.AddItem(ItemType.STONE);
                                break;
                            case Tile.Copper:
                                Area.tiles[goal.destination.X, goal.destination.Y, 0] = Tile.Dirt;
                                Inventory.AddItem(ItemType.COPPER);
                                break;
                            case Tile.Iron:
                                Area.tiles[goal.destination.X, goal.destination.Y, 0] = Tile.Dirt;
                                Inventory.AddItem(ItemType.IRON);
                                break;
                        }
                        goals.RemoveAt(0);
                        break;
                    case GoalType.ITEM:
                        ItemType it = Inventory.selecteditem;
                        switch(it)
                        {
                            case ItemType.SEEDS:
                                if (Area.tiles[goal.destination.X, goal.destination.Y, 0] == Tile.TilledLand)
                                {
                                    Simulation.AddEntity(new Crop(goal.destination));
                                    Inventory.RemoveItem(ItemType.SEEDS);
                                    satiation -= plantingfoodcost;
                                }
                                break;
                            case ItemType.STONEBUCKET:
                                if (Area.tiles[goal.destination.X, goal.destination.Y, 0] == Tile.Water)
                                {
                                    Area.tiles[goal.destination.X, goal.destination.Y, 0] = Tile.Dirt;
                                    Inventory.RemoveItem(ItemType.STONEBUCKET);
                                    Inventory.AddItem(ItemType.WATERBUCKET);
                                }
                                break;
                            case ItemType.WATERBUCKET:
                                Inventory.RemoveItem(ItemType.WATERBUCKET);
                                Inventory.AddItem(ItemType.STONEBUCKET);
                                water += 80;
                                water = Math.Min(water, maxwater);
                                break;
                            case ItemType.CROPS:
                                Inventory.RemoveItem(ItemType.CROPS);
                                satiation += 30;
                                satiation = Math.Min(satiation, maxsatiation);
                                break;  
                        }
                        if (Inventory.ItemCount(Inventory.selecteditem) == 0)
                            Inventory.selecteditem = ItemType.NONE;
                        goals.RemoveAt(0);
                        break;
                    case GoalType.SHOVEL:
                        goals.RemoveAt(0);
                        break;
                    case GoalType.GOAL:
                        goals.RemoveAt(0);
                        break;
                }
                Entity e = area.GetEntity(goal.destination);
                if(e != null)
                {
                    if (e.Tag == "Plant")
                    {
                        Inventory.AddItem(ItemType.SEEDS);
                        Simulation.RemoveEntity(e);
                    }
                    if (e.Tag == "Crop" && e.Sprite == 3)
                    {
                        Inventory.AddItem(ItemType.CROPS);
                        Inventory.AddItem(ItemType.SEEDS);
                        Inventory.AddItem(ItemType.SEEDS);
                        Simulation.RemoveEntity(e);
                    }
                }
            }
            Timer t = new Timer(Action, slowness, enabled);
        }

        void GoTo(XY dest)
        {
            if (pos.Equals(dest))
                return;

            bool xlarge = pos.X + longestdist > Area.tiles.GetUpperBound(0);
            bool ylarge = pos.Y + longestdist > Area.tiles.GetUpperBound(1);
            bool ysmall = pos.Y - longestdist < 0;
            bool xsmall = pos.X - longestdist < 0;
            if (xlarge || ylarge || ysmall || xsmall)
            {
                goals.RemoveAt(0);
                return;
            }
            
            Tile[,,] tiles = Area.tiles;
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
                    bool notwater = tiles[xd, yd, 0] != Tile.Water;
                    bool notwall = tiles[xd, yd, 0] != Tile.PlasticWall;
                    bool notriver = tiles[xd, yd, 0] != Tile.River;
                    bool notstone = tiles[xd, yd, 0] != Tile.Stone;
                    bool notiron = tiles[xd, yd, 0] != Tile.Iron;
                    bool notcopper = tiles[xd, yd, 0] != Tile.Copper;
                    map[x, y] = notwater && notriver && notwall && notstone && notiron && notcopper;
                }
            }
            var localdest = -pos + longestdist + dest;
            AStar a = new AStar(new XY(longestdist, longestdist), localdest, map);
            var path = a.CalculatePath();
            if(path == null)
            {
                goals.RemoveAt(0);
                return;
            }

            var nearestdest = path.First();
            nearestdest += pos;
            nearestdest -= longestdist;

            XY d = nearestdest - pos;
            Assert.IsTrue(Math.Abs(d.X) + Math.Abs(d.Y) <= 1);
            Move(nearestdest);
        }

        void Move(XY dest)
        {
            Area area = Simulation.inst.area;
            XY dir = (dest - pos);

            if (float.IsNaN(dir.X) || float.IsNaN(dir.Y))
            {
                dir = XY.Zero;
            }
            var absx = Math.Abs(dir.X);
            var absy = Math.Abs(dir.Y);
            if (absx > absy)
            {
                dir.X = Math.Sign(dir.X);
                dir.Y = 0;
            }
            else
            {
                dir.X = 0;
                dir.Y = Math.Sign(dir.Y);
            }
            Tile tile = Area.tiles[(pos + dir).X, (pos + dir).Y, 0];
            if (tile != Tile.Water && tile != Tile.PlasticWall && tile != Tile.River && dir != XY.Zero)
            {
                TakeMove(dir);
            } else if (dir != XY.Zero)
            {
                var newdir = (dest - pos);
                if(dir.X > 0)
                {
                    newdir.X = 0;
                    newdir.Y = Math.Sign(newdir.Y);
                }
                if (dir.Y > 0)
                {
                    newdir.Y = 0;
                    newdir.X = Math.Sign(newdir.X);
                }
                tile = Area.tiles[(pos + newdir).X, (pos + newdir).Y, 0];
                if (tile != Tile.Water && tile != Tile.River && tile != Tile.PlasticWall)
                {
                    TakeMove(newdir);
                }
            }
        }

        void TakeMove(XY dir)
        {
            pos += dir;
            satiation -= movementfoodcost;
        }

    }
}
