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
        Platinum_Ore,
        Coal,
        Platinum_Wall,
        Platinum_Bar,
        Iron_Bar,
        Copper_Bar,
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
        None = 0,
        TilledLand,
        Seed,
        Crop,
        Water,
        PlatinumWall,
        PlatinumFloor,
        PlatinumDoor,
        Snow,
        Stone,
        Iron,
        Copper,
        Vegetation,
        Slime,
        Sand,
        River,
        Trunk,
        Dirt,
        Coal,
        Platinum
    }
}
