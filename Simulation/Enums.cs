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
        TILLGROUND,
        SHOVEL,
        MINE,
        ITEM,
        GOAL
    }
    public enum AnimalType
    {
        CARNIVOR,
        HERBAVOR
    }
    public enum ItemType
    {
        CROPS,
        HOE = 0,
        SHOVEL = 1,
        PICKAXE = 2,
        SEEDS = 4,
        STONE = 5,
        STONEBUCKET = 6,
        WATERBUCKET = 7,
        IRON = 8,
        COPPER = 9,
        NONE = 10
    }
    public enum Tool
    {
        Hoe = 0,
        Shovel = 1,
        Pickaxe = 2,
        None = 1000000
    }
    public enum Tile
    {
        TilledLand = 1,
        Seed = 2,
        Crop = 3,
        Water = 4,
        PlasticWall = 5,
        PlasticFloor = 6,
        PlasticDoor = 7,
        Snow = 8,
        Stone = 9,
        Iron = 10,
        Copper = 11,
        Vegetation = 12,
        Slime = 13,
        Sand = 14,
        River = 15,
        Trunk = 16,
        Dirt = 17
}
}
