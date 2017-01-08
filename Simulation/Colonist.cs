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
        float energy = 40;
        float maxenergy = 40;
        float satiation = 24 * 21;
        public const int sleephour = 22;
        public const int wakehour = 6;
        const int longestdist = 6;
        float slowness = 1f;
        int speed = 8;
        float movementfoodcost = 0.5f;
        float plantingfoodcost = 0.5f;
        int hungry = 300;
        List<Tuple<int, int>> places = new List<Tuple<int, int>>();
        static List<Tuple<int, int>> doors = new List<Tuple<int, int>>();
        public static int numcolonists;
        Tuple<int, int> dest;
        bool atdoor = false;
        public Colonist (Vector2 pos)
        {
            this.pos = pos;
            Sprite = 0;
            draw = Drw;
            update = upd;
            tag = "Colonist";
            tex = TextureAtlas.SPRITES;
            Timer t = new Timer(Action, 1f, enabled);
            Timer t1 = new Timer(Weaken, 1f, enabled);
            UpdateDescription();
            var post = postotuple();
            dest = new Tuple<int, int>(post.Item1 - longestdist, post.Item2 - longestdist);
        }

        public void UpdateDescription()
        {
            StringBuilder sb = new StringBuilder("--- Colonist ---\n");
            sb.AppendLine("Energy: " + energy);
            sb.AppendLine("Satiation: " + satiation);
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
            places = new List<Tuple<int, int>>();
            int lowerx = ((int)pos.X / Simulation.tilesize) - longestdist;
            int higherx = ((int)pos.X / Simulation.tilesize) + longestdist;
            int lowery = ((int)pos.Y / Simulation.tilesize) - longestdist;
            int highery = ((int)pos.Y / Simulation.tilesize) + longestdist;
            for (int x = lowerx; x < higherx; x++)
            {
                for (int y = lowery; y < highery; y++)
                {
                    if (Simulation.inst.area.tiles[x, y, 0] == Tile.TilledLand || Simulation.inst.area.tiles[x, y, 0] == Tile.Crop)
                    {
                        places.Add(new Tuple<int, int>(x, y));
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
            /*FindAllCrops();
            bool daytime = TimeCycle.Hours < sleephour && TimeCycle.Hours > wakehour;
            if (places.Count > 0 && daytime)
            {
                atdoor = false;
                PlantCrops();
            } else if (!daytime && doors.Count > 0)
            {
                energy = maxenergy;
                Sleep();
            }*/
            var post = postotuple();
            if (post.Equals(dest))
                return;
            int[,,] tiles = Simulation.inst.area.tiles;
            int xlower = post.Item1 - longestdist;
            int ylower = post.Item2 - longestdist;
            int xhigher = post.Item1 + longestdist;
            int yhigher = post.Item2 + longestdist;
            bool[,] map = new bool[longestdist * 2, longestdist * 2];
            for(int x = 0; x < longestdist * 2; x++)
            {
                for (int y = 0; y < longestdist * 2; y++)
                {
                    var xd = post.Item1 - longestdist + x;
                    var yd = post.Item2 - longestdist + y;
                    bool notwater = tiles[xd, yd, 0] != Tile.Water;
                    bool notwall = tiles[xd, yd, 0] != Tile.PlasticWall;
                    bool notriver = tiles[xd, yd, 0] != Tile.River;
                    bool notstone = tiles[xd, yd, 0] != Tile.Stone;
                    map[x, y] = notwater && notriver && notwall && notstone;
                }
            }
            var localdest = new Tuple<int, int>(-post.Item1 + longestdist + dest.Item1, -post.Item2 + longestdist + dest.Item2);
            if (!map[localdest.Item1, localdest.Item2])
            {
                Console.WriteLine("Bad coordinate for pathfinding.");
                return;
            }
            
            AStar a = new AStar(new Tuple<int, int>(longestdist, longestdist), localdest, map);
            var path = a.CalculatePath();
            var nearestdest = path.First();
            nearestdest = new Tuple<int, int>(nearestdest.Item1 + post.Item1 - longestdist, nearestdest.Item2 + post.Item2 - longestdist);
            
            Tuple<int, int> d = new Tuple<int, int>(nearestdest.Item1 - post.Item1, nearestdest.Item2 - post.Item2);
            Assert.IsTrue(Math.Abs(d.Item1) + Math.Abs(d.Item2) <= 1);
            Console.WriteLine(d.ToString());
            Vector2 nd = new Vector2(nearestdest.Item1 * Simulation.tilesize, nearestdest.Item2 * Simulation.tilesize);
            Move(nd);
            
            Timer t = new Timer(Action, slowness, enabled);
                
        }

        void PlantCrops ()
        {
            Area area = Simulation.inst.area;
            var crop = FindClosest(places, new Tuple<int, int>((int)pos.X / Simulation.tilesize, (int)pos.Y / Simulation.tilesize));

            var distance = Vector2.Distance(new Vector2(crop.Item1, crop.Item2), pos / Simulation.tilesize);
            if (distance < longestdist)
            {
                //dest = new Vector2(crop.Item1 * Simulation.tilesize, crop.Item2 * Simulation.tilesize);
            }

            //Move(dest);

            var posint = postotuple();
            
            if (posint.Equals(crop))
            {
                if (area.tiles[crop.Item1, crop.Item2, 0] == Tile.Crop)
                    Crops.harvestedcrops++;
                area.tiles[crop.Item1, crop.Item2, 0] = Tile.Seed;
                Crops.AddCrop(crop);
                satiation -= plantingfoodcost;
            }
        }

        Tuple<int, int> postotuple()
        {
            return new Tuple<int, int>((int)Math.Round(pos.X) / Simulation.tilesize, (int)Math.Round(pos.Y) / Simulation.tilesize);
        }

        void Move(Vector2 dest)
        {
            Area area = Simulation.inst.area;
            Vector2 dir = (dest - pos);
            dir.Normalize();

            if (float.IsNaN(dir.X) || float.IsNaN(dir.Y))
            {
                dir = Vector2.Zero;
            }
            var absx = Math.Abs(dir.X);
            var absy = Math.Abs(dir.Y);
            if (absx > absy)
            {
                dir.X = Math.Sign(dir.X) * Simulation.tilesize;
                dir.Y = 0f;
            }
            else
            {
                dir.X = 0f;
                dir.Y = Math.Sign(dir.Y) * Simulation.tilesize;
            }
            int tile = area.tiles[(int)(pos + dir).X / Simulation.tilesize, (int)(pos + dir).Y / Simulation.tilesize, 0];
            if (tile != Tile.Water && tile != Tile.PlasticWall && tile != Tile.River && dir != Vector2.Zero)
            {
                TakeMove(dir);
            } else if (dir != Vector2.Zero)
            {
                var newdir = (dest - pos);
                if(dir.X > 0)
                {
                    newdir.X = 0;
                    newdir.Y = Math.Sign(newdir.Y) * Simulation.tilesize;
                }
                if (dir.Y > 0)
                {
                    newdir.Y = 0;
                    newdir.X = Math.Sign(newdir.X) * Simulation.tilesize;
                }
                tile = area.tiles[(int)(pos + newdir).X / Simulation.tilesize, (int)(pos + newdir).Y / Simulation.tilesize, 0];
                if (tile != Tile.Water && tile != Tile.River && tile != Tile.PlasticWall)
                {
                    TakeMove(newdir);
                }
            }
        }

        void TakeMove(Vector2 dir)
        {
            pos += dir;
            energy -= 1f;
            satiation -= movementfoodcost;
            slowness = maxenergy / (speed * energy + 2);
        }

        void Sleep ()
        {
            var position = new Tuple<int, int>((int)pos.X / Simulation.tilesize, (int)pos.Y / Simulation.tilesize);
            var door = FindClosest(doors, position);
            if(!atdoor)
            {
                Move(new Vector2(door.Item1 * Simulation.tilesize, door.Item2 * Simulation.tilesize));
                position = new Tuple<int, int>((int)pos.X / Simulation.tilesize, (int)pos.Y / Simulation.tilesize);
                if (position.Equals(door))
                    atdoor = true;
            } else
            {
                var destination = new Tuple<int, int>(door.Item1, door.Item2 + 3);
                Move(new Vector2(destination.Item1 * Simulation.tilesize, destination.Item2 * Simulation.tilesize));
                position = new Tuple<int, int>((int)pos.X / Simulation.tilesize, (int)pos.Y / Simulation.tilesize);
                if(position.Equals(description))
                {
                    energy = maxenergy;
                }

            }
            
        }

        public static void AddDoor(Tuple<int, int> door)
        {
            doors.Add(door);
        }

        public static void RemoveDoor(Tuple<int, int> door)
        {
            doors.Remove(door);
        }

        Tuple<int, int> FindClosest(List<Tuple<int, int>> places, Tuple<int, int> place)
        {
            Tuple<int, int> nplace = new Tuple<int, int>(10000000, 100000000);
            foreach (Tuple<int, int> pl in places)
                nplace = (Vector2.Distance(new Vector2(pl.Item1, pl.Item2), new Vector2(place.Item1, place.Item2)) > Vector2.Distance(new Vector2(nplace.Item1, nplace.Item2), new Vector2(place.Item1, place.Item2)) ? nplace : pl);
            return nplace;
        }
    }
}
