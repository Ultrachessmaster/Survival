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
        static List<XY> plantedcrops = new List<XY>();
        public static int harvestedcrops = 0;
        float deathtime = 1f;
        public Crops (Area area)
        {
            this.area = area;
            Timer time = new Timer(DestroyCrops, deathtime);
        }
        public void Upd ()
        {
            if (TimeCycle.Hours == (Colonist.wakehour - 1))
            {
                for (int i = plantedcrops.Count - 1; i >= 0; i--)
                {
                    XY place = plantedcrops[i];
                    if (area.tiles[place.X, place.Y, 0] != Tile.Seed)
                        continue;
                    area.tiles[place.X, place.Y, 0] = Tile.Crop;
                }
                plantedcrops = new List<XY>();
            }
        }
        public void DestroyCrops(float overtime)
        {
            if(plantedcrops.Count > 0)
            {
                var crop = plantedcrops.First();
                if (area.TilesSurrounding(crop.X, crop.Y, 0, 4) == 0)
                {
                    area.tiles[crop.X, crop.Y, 0] = Tile.TilledLand;
                    plantedcrops.RemoveAt(0);
                }
            }

            new Timer(DestroyCrops, deathtime);
        }

        public static void AddCrop(XY crop)
        {
            plantedcrops.Add(crop);
        }
    }
}
