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
        float energy = 15;
        float maxenergy = 15;
        float fullness = 24 * 21;
        public const int sleephour = 22;
        public const int wakehour = 4;
        float longestdist = 4;
        float speed = 1f;
        float movementfoodcost = 0.5f;
        float plantingfoodcost = 0.5f;
        List<Tuple<int, int>> places = new List<Tuple<int, int>>();
        static List<Tuple<int, int>> doors = new List<Tuple<int, int>>();
        Vector2 dest;
        public Colonist (Vector2 pos)
        {
            this.pos = pos;
            Sprite = 0;
            draw = Drw;
            update = upd;
            Timer t = new Timer(Action, 1f);
            Timer t1 = new Timer(Weaken, 1f);
        }

        public void upd (GameTime g)
        {
            
            if (fullness <= 0)
                Simulation.DestroyEntity(this);
        }

        void FindAllCrops ()
        {
            places = new List<Tuple<int, int>>();
            int lowerx = ((int)pos.X / Simulation.tilesize) - (int)longestdist;
            int higherx = ((int)pos.X / Simulation.tilesize) + (int)longestdist;
            int lowery = ((int)pos.Y / Simulation.tilesize) - (int)longestdist;
            int highery = ((int)pos.Y / Simulation.tilesize) + (int)longestdist;
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
            fullness--;
            Timer t = new Timer(Weaken, 1f);
        }

        void Action (float overtime)
        {
            FindAllCrops();
            bool daytime = TimeCycle.Hours < sleephour && TimeCycle.Hours > wakehour;
            if (places.Count > 0 && daytime && energy > 0)
            {
                PlantCrops();
            } else if (!daytime && doors.Count > 0)
            {
                energy = maxenergy;
                Sleep();
            }
            
            Timer t = new Timer(Action, speed);
                
        }

        void PlantCrops ()
        {
            Area area = Simulation.inst.area;
            var crop = FindClosest(places, new Tuple<int, int>((int)pos.X / Simulation.tilesize, (int)pos.Y / Simulation.tilesize));

            var distance = Vector2.Distance(new Vector2(crop.Item1, crop.Item2), pos / Simulation.tilesize);
            if (distance < longestdist)
            {
                dest = new Vector2(crop.Item1 * Simulation.tilesize, crop.Item2 * Simulation.tilesize);
            }

            Move(dest);

            var posint = new Tuple<int, int>((int)Math.Round(pos.X) / Simulation.tilesize, (int)Math.Round(pos.Y) / Simulation.tilesize);
            if (posint.Equals(crop))
            {
                if (area.tiles[crop.Item1, crop.Item2, 0] == Tile.Crop)
                    Crops.harvestedcrops++;
                area.tiles[crop.Item1, crop.Item2, 0] = Tile.Seed;
                Crops.AddCrop(crop);
                fullness -= plantingfoodcost;
            }
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
            if (tile != Tile.Water && tile != Tile.PlasticWall && dir != Vector2.Zero)
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
                if (tile != Tile.Water && tile != Tile.PlasticWall)
                {
                    TakeMove(newdir);
                }
            }
        }

        void TakeMove(Vector2 dir)
        {
            pos += dir;
            energy -= 1f;
            fullness -= movementfoodcost;
            speed = maxenergy / (2 * energy + 2);
        }

        void Sleep ()
        {
            var door = FindClosest(doors, new Tuple<int, int>((int)pos.X / Simulation.tilesize, (int)pos.Y / Simulation.tilesize));
            Move(new Vector2(door.Item1 * Simulation.tilesize, door.Item2 * Simulation.tilesize));
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
