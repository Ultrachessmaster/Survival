using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class Crops
    {
        Area area;
        static List<Tuple<int, int>> plantedcrops = new List<Tuple<int, int>>();
        public static int harvestedcrops = 0;
        float deathtime = 5f;
        public Crops (Area area)
        {
            this.area = area;
            Timer time = new Timer(DestroyCrops, deathtime);
        }
        public void Upd ()
        {
            if (TimeCycle.Hours == (Colonist.wakehour - 1))
            {
                foreach(Tuple<int, int> place in plantedcrops)
                {
                    if (area.tiles[place.Item1, place.Item2, 0] != Tile.Seed)
                        continue;
                    area.tiles[place.Item1, place.Item2, 0] = Tile.Crop;
                }
                plantedcrops = new List<Tuple<int, int>>();
            }
        }
        public void DestroyCrops(float overtime)
        {
            if(plantedcrops.Count > 0)
            {
                var crop = plantedcrops.First();
                if (area.NumberSurrounding(crop.Item1, crop.Item2, 0, 4) == 0)
                {
                    area.tiles[crop.Item1, crop.Item2, 0] = Tile.TilledLand;
                    plantedcrops.RemoveAt(0);
                }
            }

            new Timer(DestroyCrops, deathtime);
        }

        public static void AddCrop(Tuple<int, int> crop)
        {
            plantedcrops.Add(crop);
        }
    }
}
