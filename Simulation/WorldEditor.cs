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
        List<int> farmtools = new List<int>();
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
            farmtools = new List<int>();
            farmtools.Add(Tool.Hoe);
            farmtools.Add(Tool.Grab);
            farmtools.Add(Tool.Walk);
            menu = new Menu(farmtools, screenheight);
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
            if(Input.IsKeyDown(Keys.F))
                con = Context.FARM;
            if (Input.IsKeyDown(Keys.B))
                con = Context.BUILDING;
            if (my >= menutiles && rightb)
            {
                var idx = Math.Min(mx / (Simulation.tilesize * 2), menu.NumTiles - 1);
                tile = menu.ToolSelected(idx);
            }
            if(my < textboxborder)
            {
                switch (con)
                {
                    case Context.FARM:
                        menu = new Menu(farmtools, screenheight);
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
                            bool wateraround = area.TilesSurrounding(mwx, mwy, 0, Tile.Water) > 0;
                            bool riveraround = area.TilesSurrounding(mwx, mwy, 0, Tile.River) > 0;
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
                            bool wallsaround = area.TilesSurrounding(mwx, mwy, 0, Tile.PlasticWall) > 1;
                            bool floorsaround = area.TilesSurrounding(mwx, mwy, 0, Tile.PlasticFloor) > 0;
                            if (leftb && wallsaround && floorsaround)
                            {
                                area.tiles[mwx, mwy, 0] = Tile.PlasticDoor;
                                Colonist.AddDoor(new XY(mwx, mwy));
                            }

                            break;
                    }
                    bool waterortilledland = (area.tiles[mwx, mwy, 0] == Tile.River || area.tiles[mwx, mwy, 0] == Tile.TilledLand || area.tiles[mwx, mwy, 0] == Tile.Seed || area.tiles[mwx, mwy, 0] == Tile.Crop);

                }
            }

            MoveCamera();
            SetLayer();

        }

        void MoveCamera()
        {
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

        void SetLayer()
        {
            if (Input.IsKeyPressed(Keys.E))
                Simulation.currentlayer++;
            if (Input.IsKeyPressed(Keys.Q))
                Simulation.currentlayer--;
            Simulation.currentlayer = Math.Max(0, Simulation.currentlayer);
            Simulation.currentlayer = Math.Min(Simulation.inst.area.tiles.GetUpperBound(2), Simulation.currentlayer);
        }

        public void Draw(SpriteBatch sb)
        {
            menu.Draw(sb);
        }
    }
}
