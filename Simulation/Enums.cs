using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    public enum Context
    {
        PICKING,
        FARM,
        COLONIST,
        BUILDING,
    }

    public enum TextureAtlas
    {
        SPRITES,
        TILES,
        ANIMALS
    }
    public enum GoalType
    {
        HARVESTSEEDS,
        HARVESTCROPS,
        TILLGROUND,
        PLANTSEEDS,
        SHOVEL,
        MINE,
        ITEM
    }
    public enum AnimalType
    {
        CARNIVOR,
        HERBAVOR
    }
    public enum ItemType
    {
        SEEDS,
        STONE,
        STONEBUCKET,
        WATERBUCKET,
        CROPS,
        NONE
    }
    public enum Tool
    {
        Hoe = 0,
        Grab = 1,
        Walk = 2,
        PlantSeed = 3,
        Shovel = 4,
        Pickaxe = 5,
        Nothing = 8
    }
}
