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
        }

        public void Update (Area area)
        {
            var mstate = Mouse.GetState();
            var mwx = Math.Max(((mstate.X / Simulation.pxlratio) + Camera.X) / Simulation.tilesize, 0);
            var mwy = Math.Max(((mstate.Y / Simulation.pxlratio) + Camera.Y) / Simulation.tilesize, 0);
            var mx = mstate.X;
            var my = mstate.Y;
            var menutiles = screenheight - (Simulation.tilesize * 2);
            
            var left1stb = Input.IsMouseButtonPressed(0);
            var right1stb = Input.IsMouseButtonPressed(1);
            var leftb = mstate.LeftButton == ButtonState.Pressed;
            var rightb = mstate.RightButton == ButtonState.Pressed;
            if (Input.IsKeyDown(Keys.C))
                con = Context.COLONIST;
            if(Input.IsKeyDown(Keys.F))
                con = Context.FARM;
            if (Input.IsKeyDown(Keys.B))
                con = Context.BUILDING;
            if (my >= menutiles && rightb)
            {
                var idx = Math.Min(mx / (Simulation.tilesize * 2), menu.NumTiles - 1);
                tile = menu.TileSelected(idx);
            }
                
            switch (con)
            {
                case Context.FARM:
                    menu = new Menu(farmtiles, screenheight);
                    placingtiles = true;
                    break;
                case Context.COLONIST:
                    placingtiles = false;
                    if (left1stb)
                        Simulation.AddEntity(new Colonist(new Vector2((float)Math.Round((double)mwx * Simulation.tilesize), (float)Math.Round((double)mwy * Simulation.tilesize))));
                    break;
                case Context.BUILDING:
                    menu = new Menu(buildingtiles, screenheight);
                    placingtiles = true;
                    break;
            }
            if(placingtiles)
            {
                switch (tile)
                {
                    case 1:
                        if (leftb && area.tiles[mwx, mwy, 0] == Tile.Sand)
                            area.tiles[mwx, mwy, 0] = Tile.TilledLand;
                        break;
                    case 4:
                        bool wateraround = area.NumberSurrounding(mwx, mwy, 0, Tile.Water) > 0;
                        if (leftb && wateraround)
                        {
                            area.tiles[mwx, mwy, 0] = Tile.Water;
                        }

                        break;
                    case 5:
                        if (leftb)
                            area.tiles[mwx, mwy, 0] = Tile.PlasticWall;
                        break;
                    case 6:
                        if (leftb)
                            area.tiles[mwx, mwy, 0] = Tile.PlasticFloor;
                        break;
                    case 7:
                        bool wallsaround = area.NumberSurrounding(mwx, mwy, 0, Tile.PlasticWall) > 1;
                        bool floorsaround = area.NumberSurrounding(mwx, mwy, 0, Tile.PlasticFloor) > 0;
                        if (leftb && wallsaround && floorsaround)
                        {
                            area.tiles[mwx, mwy, 0] = Tile.PlasticDoor;
                            Colonist.AddDoor(new Tuple<int, int>(mwx, mwy));
                        }
                            
                        break;
                }
                if (rightb)
                    area.tiles[mwx, mwy, 0] = Tile.Sand;
                    
            }
            if (Input.IsKeyPressed(Keys.I))
                Simulation.pxlratio++;
            if (Input.IsKeyPressed(Keys.K))
                Simulation.pxlratio--;
            Simulation.inst.Content.Equals(1);
            int vx = Simulation.GD.Viewport.X;
            int vy = Simulation.GD.Viewport.Y;
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
