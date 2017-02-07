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
        public Crops (Area area)
        {
            this.area = area;
        }
        public void Upd ()
        {
            if (TimeCycle.Hours == (Colonist.wakehour - 1))
            {
                for (int i = plantedcrops.Count - 1; i >= 0; i--)
                {
                    XY place = plantedcrops[i];
                    if (Area.tiles[place.X, place.Y, 0] != Tile.Seed)
                        continue;
                    Area.tiles[place.X, place.Y, 0] = Tile.Crop;
                }
                plantedcrops = new List<XY>();
            }
        }

        public static void AddCrop(XY crop)
        {
            plantedcrops.Add(crop);
        }
    }
}
