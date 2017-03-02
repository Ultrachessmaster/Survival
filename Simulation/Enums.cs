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
        ANIMALS,
        ITEMS,
        TILES
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
        Crop,
        Hoe,
        Shovel,
        Pickaxe,
        Seed,
        Stone,
        Stone_Bucket,
        Water_Bucket,
        Iron_Ore,
        Copper_Ore,
        Furnace,
        Tin_Ore,
        Coal,
        Tin_Door,
        None
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
        TinWall = 5,
        TinFloor = 6,
        TinDoor = 7,
        Snow = 8,
        Stone = 9,
        Iron = 10,
        Copper = 11,
        Vegetation = 12,
        Slime = 13,
        Sand = 14,
        River = 15,
        Trunk = 16,
        Dirt = 17,
        Coal = 18,
        Tin = 19
    }
}
