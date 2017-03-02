using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoiseTest;

namespace Simulation
{
    class Generation
    {
        public const float waterbiome = 0.2f;
        public const float sandbiome = 0.25f;
        public const float vegetation = 0.55f;
        public const float dirtbiome = 0.6f;
        public const float stonebiome = 0.78f;
        public const float snowbiome = 1f;
        static Random r = new Random(Simulation.seed);
        public static Area Generate(Vector3 size)
        {
            Area area = new Area();
            Tile[,] map = new Tile[(int)size.X, (int)size.Y];
            OpenSimplexNoise o = new OpenSimplexNoise(Simulation.seed);
            var inversescale = 35f;
            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    var noisex = (x / size.X) - 0.5f;
                    var noisey = (y / size.Y) - 0.5f;
                    float heightlarge = ((float)o.Evaluate(inversescale * 0.7f * noisex, inversescale * 0.5f * noisey) * 0.5f) + 0.5f;
                    float heightmedium = ((float)o.Evaluate(inversescale * 0.2f * noisex, inversescale * 0.35f * noisey) * 0.5f) + 0.5f;
                    float heightsmall = ((float)o.Evaluate(inversescale * 0.1f * noisex, inversescale * 0.15f * noisey) * 0.5f) + 0.5f;
                    heightlarge *= 0.7f;
                    heightmedium *= 0.2f;
                    heightsmall *= 0.1f;
                    float height = heightlarge + heightmedium + heightsmall;
                    float scaledheight = (float)Math.Pow(height, 1.3f);
                    if (scaledheight <= snowbiome && height > stonebiome)
                    {
                        map[x, y] = Tile.Snow;
                    }
                    if (scaledheight <= stonebiome && height > dirtbiome)
                    {
                        map[x, y] = Tile.Stone;
                        if (r.Range(0f, 1f) < 0.02)
                            map[x, y] = Tile.Iron;
                        if (r.Range(0f, 1f) < 0.01)
                            map[x, y] = Tile.Copper;
                        if (r.Range(0f, 1f) < 0.015)
                            map[x, y] = Tile.Coal;
                        if (r.Range(0f, 1f) < 0.03)
                            map[x, y] = Tile.Tin;
                    }
                    if (scaledheight <= dirtbiome && height > vegetation)
                    {
                        map[x, y] = Tile.Dirt;
                    }
                    if (scaledheight <= vegetation && height > sandbiome)
                    {
                        map[x, y] = Tile.Vegetation;
                    }
                    if (scaledheight <= sandbiome && height > waterbiome)
                    {
                        map[x, y] = Tile.Sand;
                    }
                    if (scaledheight < waterbiome)
                    {
                        map[x, y] = Tile.Water;
                    }
                }
            }

            CreateBase(area, map, new XY((int)size.X, (int)size.Y));

            Tile[,,] fullmap = new Tile[(int)size.X, (int)size.Y, (int)size.Z];
            for (int x = 0; x < size.X; x++)
            {
                for(int y = 0; y < size.Y; y++)
                {
                    fullmap[x, y, 0] = map[x, y];
                }
            }

            Area.tiles = fullmap;
            Area.AddRangeE(GenerateAnimals(fullmap, area));
            Area.AddRangeE(GeneratePlants(fullmap, area));
            return area;
            
        }

        static void CreateBase(Area area, Tile[,] map, XY size)
        {
            bool foundplace = false;
            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {

                    bool xgreater = x > (size.X / 3f);
                    bool xsmaller = x < (size.X - (size.X / 3f));
                    bool ygreater = y > (size.Y / 3f);
                    bool ysmaller = y < (size.Y - (size.Y / 3f));

                    if (xgreater && xsmaller && ygreater && ysmaller && !foundplace)
                    {
                        int xoffset = 10;
                        bool corner1notwater = map[x + xoffset, y] != Tile.Water;
                        bool corner2notwater = map[x + 7, y + 5] != Tile.Water;
                        bool outsidedoornotwater = map[x + 4, y - 1] != Tile.Water;
                        if (map[x, y] == Tile.Water && corner1notwater && outsidedoornotwater)
                        {
                            for (int xb = x + xoffset; xb < x + xoffset + 7; xb++)
                            {
                                for (int yb = y; yb < y + 8; yb++)
                                {
                                    map[xb, yb] = Tile.TinWall;
                                }
                            }
                            for (int xb = x + xoffset + 1; xb < x + +xoffset + 6; xb++)
                            {
                                for (int yb = y + 3; yb < y + 7; yb++)
                                {
                                    map[xb, yb] = Tile.TinFloor;
                                }
                            }
                            map[x + xoffset + 3, y] = Tile.TinDoor;
                            map[x + xoffset + 3, y + 1] = Tile.TinFloor;
                            map[x + xoffset + 3, y + 2] = Tile.TinFloor;
                            XY home = new XY(x + xoffset + 3, (y + 3));
                            Area.AddEntity(new Colonist(home, area));
                            Area.AddEntity(new Colonist(new XY((x + xoffset + 1), (y + 3)), area));
                            Area.AddEntity(new Colonist(new XY((x + xoffset + 5), (y + 3)), area));
                            Colonist.home = home;
                            Camera.X = (x + xoffset - 10) * Simulation.tilesize;
                            Camera.Y = (y - 10) * Simulation.tilesize;
                            foundplace = true;
                        }
                    }
                }
            }
        }

        static List<Entity> GenerateAnimals(Tile[,,] map, Area area)
        {
            List<Entity> ent = new List<Entity>();

            Color c = new Color(r.Range(0f, 1f), r.Range(0f, 1f), r.Range(0f, 1f));
            int sprite = r.Next(0, 4);

            for (int x = 0; x < map.GetUpperBound(0) + 1; x++)
            {
                for (int y = 0; y < map.GetUpperBound(1) + 1; y++)
                {
                    
                    float choice = r.Range(0f, 1f);
                    bool notwall = map[x, y, 0] != Tile.TinWall;
                    bool notfloor = map[x, y, 0] != Tile.TinFloor;
                    bool notdoor = map[x, y, 0] != Tile.TinDoor;
                    bool notwater = map[x, y, 0] != Tile.Water;
                    if (choice < 0.00025 && notwall && notfloor && notdoor && notwater)
                        ent.Add(new Vulpes(new XY(x, y), area));
                    else if (notwall && notfloor && notdoor && notwater)
                    {
                        if (r.Range(0f, 1f) < 0.00095)
                            ent.Add(new Orycto(new XY(x, y)));
                    }

                }
            }
            
            return ent;
        }

        static List<Entity> GeneratePlants(Tile[,,] map, Area area)
        {
            List<Entity> ent = new List<Entity>();

            for (int x = 0; x < map.GetUpperBound(0) + 1; x++)
            {
                for (int y = 0; y < map.GetUpperBound(1) + 1; y++)
                {

                    float choice = r.Range(0f, 1f);
                    bool notwall = map[x, y, 0] != Tile.TinWall;
                    bool notfloor = map[x, y, 0] != Tile.TinFloor;
                    bool notdoor = map[x, y, 0] != Tile.TinDoor;
                    bool vegetation = map[x, y, 0] == Tile.Vegetation;
                    if (choice < 0.005 && notwall && notfloor && notdoor && vegetation)
                    {
                        ent.Add(new Plant(new XY(x, y)));
                    }
                }
            }

            return ent;
        }
    }
}
