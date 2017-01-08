using System;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Simulation
{
    class WorldEditor
    {
        public Context context { get { return con; } }
        List<int> farmtiles = new List<int>();
        List<int> buildingtiles;
        int screenheight;
        int tile = 0;
        int menutiles;
        int textboxborder;
        bool placingtiles = true;
        Menu menu;
        Context con = Context.FARM;

        public WorldEditor (int screenheight)
        {
            this.screenheight = screenheight;
            farmtiles = new List<int>();
            farmtiles.Add(Tile.TilledLand);
            farmtiles.Add(Tile.Water);
            buildingtiles = new List<int>();
            buildingtiles.Add(Tile.PlasticWall);
            buildingtiles.Add(Tile.PlasticFloor);
            buildingtiles.Add(Tile.PlasticDoor);
            menu = new Menu(farmtiles, screenheight);
            menutiles = screenheight - (Simulation.tilesize * 2);
            textboxborder = screenheight - (Simulation.tilesize * 16);
        }

        public void Update (Area area)
        {
            var mstate = Mouse.GetState();
            var mwx = Input.MouseTileX();
            var mwy = Input.MouseTileX();
            var mx = mstate.X;
            var my = mstate.Y;
            
            
            var left1stb = Input.IsMouseButtonPressed(0);
            var right1stb = Input.IsMouseButtonPressed(1);
            var leftb = mstate.LeftButton == ButtonState.Pressed;
            var rightb = mstate.RightButton == ButtonState.Pressed;
            //if (Input.IsKeyDown(Keys.C))
            //    con = Context.COLONIST;
            if(Input.IsKeyDown(Keys.F))
                con = Context.FARM;
            if (Input.IsKeyDown(Keys.B))
                con = Context.BUILDING;
            if (my >= menutiles && rightb)
            {
                var idx = Math.Min(mx / (Simulation.tilesize * 2), menu.NumTiles - 1);
                tile = menu.TileSelected(idx);
            }
            if(my < textboxborder)
            {
                switch (con)
                {
                    case Context.FARM:
                        menu = new Menu(farmtiles, screenheight);
                        placingtiles = true;
                        break;
                    case Context.COLONIST:
                        menu = new Menu(new List<int>(), screenheight);
                        placingtiles = false;
                        if (left1stb)
                            Simulation.AddEntity(new Colonist(new Vector2((float)Math.Round((double)mwx * Simulation.tilesize), (float)Math.Round((double)mwy * Simulation.tilesize))));
                        break;
                    case Context.BUILDING:
                        menu = new Menu(buildingtiles, screenheight);
                        placingtiles = true;
                        break;
                }
                if (placingtiles)
                {
                    switch (tile)
                    {
                        case Tile.TilledLand:
                            bool notstone = area.tiles[mwx, mwy, 0] != Tile.Stone;
                            bool notwater = area.tiles[mwx, mwy, 0] != Tile.Water;
                            bool notsand = area.tiles[mwx, mwy, 0] != Tile.Sand;
                            if (leftb && notstone && notwater && notsand)
                                area.tiles[mwx, mwy, 0] = Tile.TilledLand;
                            break;
                        case Tile.Water:
                            bool wateraround = area.NumberSurrounding(mwx, mwy, 0, Tile.Water) > 0;
                            bool riveraround = area.NumberSurrounding(mwx, mwy, 0, Tile.River) > 0;
                            if (leftb && (wateraround || riveraround))
                            {
                                area.tiles[mwx, mwy, 0] = Tile.Water;
                            }

                            break;
                        case Tile.PlasticWall:
                            if (leftb)
                                area.tiles[mwx, mwy, 0] = Tile.PlasticWall;
                            break;
                        case Tile.PlasticFloor:
                            if (leftb)
                                area.tiles[mwx, mwy, 0] = Tile.PlasticFloor;
                            break;
                        case Tile.PlasticDoor:
                            bool wallsaround = area.NumberSurrounding(mwx, mwy, 0, Tile.PlasticWall) > 1;
                            bool floorsaround = area.NumberSurrounding(mwx, mwy, 0, Tile.PlasticFloor) > 0;
                            if (leftb && wallsaround && floorsaround)
                            {
                                area.tiles[mwx, mwy, 0] = Tile.PlasticDoor;
                                Colonist.AddDoor(new Tuple<int, int>(mwx, mwy));
                            }

                            break;
                    }
                    bool waterortilledland = (area.tiles[mwx, mwy, 0] == Tile.River || area.tiles[mwx, mwy, 0] == Tile.TilledLand || area.tiles[mwx, mwy, 0] == Tile.Seed || area.tiles[mwx, mwy, 0] == Tile.Crop);

                }
            }
            
            if (Input.IsKeyPressed(Keys.I))
                Simulation.pxlratio = Math.Min(4, Simulation.pxlratio + 1);
            if (Input.IsKeyPressed(Keys.K))
                Simulation.pxlratio = Math.Max(1, Simulation.pxlratio - 1);
            if (Input.IsKeyDown(Keys.Right))
                Camera.X += Simulation.tilesize;
            if (Input.IsKeyDown(Keys.Left))
                Camera.X -= Simulation.tilesize;
            if (Input.IsKeyDown(Keys.Up))
                Camera.Y -= Simulation.tilesize;
            if (Input.IsKeyDown(Keys.Down))
                Camera.Y += Simulation.tilesize;

        }

        public void Draw(SpriteBatch sb)
        {
            menu.Draw(sb);
        }
    }
}
