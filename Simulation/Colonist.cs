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
        float energy = 200;
        float maxenergy = 200;
        float satiation = 24 * 21;
        float maxsatiation = 24 * 21;
        public const int sleephour = 22;
        public const int wakehour = 6;
        const int longestdist = 35;
        float slowness = 1f;
        int speed = 8;
        float movementfoodcost = 0.5f;
        float plantingfoodcost = 0.5f;
        int hungry = 300;
        int seeds = 0;
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
            Timer t = new Timer(Action, 1f, enabled);
            Timer t1 = new Timer(Weaken, 1f, enabled);
            UpdateDescription();
        }

        public void UpdateDescription()
        {
            StringBuilder sb = new StringBuilder("--- Colonist ---\n");
            sb.AppendLine("Energy: " + energy + " / " + maxenergy);
            sb.AppendLine("Satiation: " + satiation + " / " + maxsatiation);
            description = sb.ToString();
        }

        public void upd (GameTime g)
        {
            if (satiation <= 0)
                enabled = new RefWrapper<bool>(false);
            if(satiation <= hungry && Crops.harvestedcrops > 0)
            {
                Crops.harvestedcrops--;
                satiation += 4;
            }
            UpdateDescription();
        }


        void FindAllCrops ()
        {
            places = new List<XY>();
            int lowerx = pos.X - longestdist;
            int higherx = pos.X + longestdist;
            int lowery = pos.Y - longestdist;
            int highery = pos.Y + longestdist;
            for (int x = lowerx; x < higherx; x++)
            {
                for (int y = lowery; y < highery; y++)
                {
                    if (Simulation.inst.area.tiles[x, y, 0] == Tile.TilledLand || Simulation.inst.area.tiles[x, y, 0] == Tile.Crop)
                    {
                        places.Add(new XY(x, y));
                    }
                }
            }
        }

        void Weaken(float overtime)
        {
            satiation--;
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
            if(!pos.Equals(goal.destination))
                GoTo(goal.destination);
            else
            {
                switch (goal.goaltype)
                {
                    case GoalType.HARVESTSEEDS:
                        Entity plant = area.GetEntity(pos, "Plant");
                        area.entities.Remove(plant);
                        seeds++;
                        energy--;
                        satiation -= plantingfoodcost;
                        goals.RemoveAt(0);
                        break;
                    case GoalType.TRAVEL:
                        goals.RemoveAt(0);
                        break;
                    case GoalType.HARVESTCROPS:
                        if (area.tiles[pos.X, pos.Y, 0] == Tile.Crop)
                            Crops.harvestedcrops++;
                        area.tiles[pos.X, pos.Y, 0] = Tile.TilledLand;
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
            int[,,] tiles = Simulation.inst.area.tiles;
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
            if (!map[localdest.X, localdest.Y])
            {
                Console.WriteLine("Bad coordinate for pathfinding.");
                Timer t1 = new Timer(Action, slowness, enabled);
                return;
            }

            AStar a = new AStar(new XY(longestdist, longestdist), localdest, map);
            var path = a.CalculatePath();
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
            int tile = area.tiles[(pos + dir).X, (pos + dir).Y, 0];
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
                tile = area.tiles[(pos + newdir).X, (pos + newdir).Y, 0];
                if (tile != Tile.Water && tile != Tile.River && tile != Tile.PlasticWall)
                {
                    TakeMove(newdir);
                }
            }
        }

        void TakeMove(XY dir)
        {
            pos += dir;
            energy--;
            satiation -= movementfoodcost;
            slowness = maxenergy / (speed * energy + 2);
        }

        void Sleep ()
        {
            /*var position = new XY((int)pos.X / Simulation.tilesize, (int)pos.Y / Simulation.tilesize);
            var door = FindClosest(doors, position);
            if(!atdoor)
            {
                Move(new Vector2(door.X * Simulation.tilesize, door.Y * Simulation.tilesize));
                position = new XY((int)pos.X / Simulation.tilesize, (int)pos.Y / Simulation.tilesize);
                if (position.Equals(door))
                    atdoor = true;
            } else
            {
                var destination = new XY(door.X, door.Y + 3);
                Move(new Vector2(destination.X * Simulation.tilesize, destination.Y * Simulation.tilesize));
                position = new XY((int)pos.X / Simulation.tilesize, (int)pos.Y / Simulation.tilesize);
                if(position.Equals(description))
                {
                    energy = maxenergy;
                }

            }*/
            
        }

        public static void AddDoor(XY door)
        {
            doors.Add(door);
        }

        public static void RemoveDoor(XY door)
        {
            doors.Remove(door);
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
