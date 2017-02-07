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
        public const int sleephour = 22;
        public const int wakehour = 6;
        const int longestdist = 100;
        const float slowness = 0.001f;
        float movementfoodcost = 0.5f;
        float plantingfoodcost = 0.5f;
        public List<Goal> goals = new List<Goal>();
        List<XY> places = new List<XY>();
        static List<XY> doors = new List<XY>();
        public static int numcolonists;
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
            UpdateDescription();
        }

        public void UpdateDescription()
        {
            StringBuilder sb = new StringBuilder("--- Colonist ---\n");
            sb.AppendLine("Satiation: " + satiation + " / " + maxsatiation);
            sb.AppendLine("Water: " + water + " / " + maxwater);
            description = sb.ToString();
        }

        public void upd (GameTime g)
        {
            if (satiation <= 0 || water <= 0)
                enabled = new RefWrapper<bool>(false);
            UpdateDescription();
        }

        void Weaken(float overtime)
        {
            satiation--;
            water -= 4;
            Timer t = new Timer(Weaken, 1f, enabled);
        }

        void Action (float overtime)
        {
            if(goals.Count == 0)
            {
                Timer t0 = new Timer(Action, slowness, enabled);
                return;
            }
            Goal goal = goals.First();
            bool atpoint = (pos - goal.destination).Magnitude() <= 1;
            if(!pos.Equals(goal.destination) && !atpoint)
                GoTo(goal.destination);
            else
            {
                switch (goal.goaltype)
                {
                    case GoalType.HARVESTSEEDS:
                        Entity plant = area.GetEntity(goal.destination, "Plant");
                        if (plant == null)
                        {
                            goals.RemoveAt(0);
                            break;
                        }
                        area.entities.Remove(plant);
                        Inventory.AddItem(ItemType.SEEDS);
                        satiation -= plantingfoodcost;
                        goals.RemoveAt(0);
                        break;
                    case GoalType.HARVESTCROPS:
                        if (Area.tiles[goal.destination.X, goal.destination.Y, 0] == Tile.Crop)
                        {
                            Inventory.AddItem(ItemType.CROPS);
                            Inventory.AddItem(ItemType.SEEDS);
                            Inventory.AddItem(ItemType.SEEDS);
                            Area.tiles[goal.destination.X, goal.destination.Y, 0] = Tile.TilledLand;
                        }
                        goals.RemoveAt(0);
                        break;
                    case GoalType.TILLGROUND:
                        if(Area.tiles[goal.destination.X, goal.destination.Y, 0] == Tile.Vegetation || Area.tiles[goal.destination.X, goal.destination.Y, 0] == Tile.Dirt)
                            Area.tiles[goal.destination.X, goal.destination.Y, 0] = Tile.TilledLand;
                        goals.RemoveAt(0);
                        break;
                    case GoalType.MINE:
                        if(Area.tiles[goal.destination.X, goal.destination.Y, 0] == Tile.Stone)
                        {
                            Area.tiles[goal.destination.X, goal.destination.Y, 0] = Tile.Dirt;
                            Inventory.AddItem(ItemType.STONE);
                        }
                        goals.RemoveAt(0);
                        break;
                    case GoalType.ITEM:
                        ItemType it = Inventory.itemselected;
                        switch(it)
                        {
                            case ItemType.SEEDS:
                                if (Area.tiles[goal.destination.X, goal.destination.Y, 0] == Tile.TilledLand)
                                {
                                    Area.tiles[goal.destination.X, goal.destination.Y, 0] = Tile.Seed;
                                    Crops.AddCrop(goal.destination);
                                    Inventory.RemoveItem(ItemType.SEEDS);
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
                        if(Inventory.ItemCount(Inventory.itemselected) == 0)
                            Inventory.itemselected = ItemType.NONE;
                        goals.RemoveAt(0);
                        break;
                }
            }
            Timer t = new Timer(Action, slowness, enabled);
        }

        void GoTo(XY dest)
        {
            if (pos.Equals(dest))
            {
                return;
            }
            int[,,] tiles = Area.tiles;
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
                    map[x, y] = notwater && notriver && notwall && notstone;
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
            Console.WriteLine(d.ToString());
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
            int tile = Area.tiles[(pos + dir).X, (pos + dir).Y, 0];
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

        XY FindClosest(List<XY> places, XY place)
        {
            XY nplace = new XY(10000000, 100000000);
            foreach (XY pl in places)
                nplace = (Vector2.Distance(new Vector2(pl.X, pl.Y), new Vector2(place.X, place.Y)) > Vector2.Distance(new Vector2(nplace.X, nplace.Y), new Vector2(place.X, place.Y)) ? nplace : pl);
            return nplace;
        }
    }
}
