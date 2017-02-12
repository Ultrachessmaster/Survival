using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Simulation
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Simulation : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        public Area area;
        Interaction interaction;
        TimeCycle timecycle = new TimeCycle();
        ColonistManager colm;
        public static List<Timer> Timers = new List<Timer>();
        public static int tilesize = 8;
        public static int pxlratio = 4;
        public static int currentlayer = 0;
        int updatesperframe = 1;
        int initscrollwheel = 0;
        public static int windowsize { get; internal set; }
        private Texture2D tileatlas;
        private Texture2D spriteatlas;
        private Texture2D animalatlas;
        private Texture2D textbox;
        private Texture2D selectionbox;
        public static Simulation inst;

        public static ContentManager CM { get; internal set; }
        public static GraphicsDevice GD { get; internal set; }

        public static int seed = (int)DateTime.Now.Ticks;

        public Simulation()
        {
            windowsize = 30 * tilesize * pxlratio;
            
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = windowsize;
            graphics.PreferredBackBufferWidth = windowsize;
            Content.RootDirectory = "Content";
            CM = Content;
            
            inst = this;
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsMouseVisible = true;
            base.Initialize();

            var size = new Vector3(512, 512, 2);
            area = Generation.Generate(size);
            colm = new ColonistManager(area);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            tileatlas = Content.Load<Texture2D>("texturemap");
            spriteatlas = Content.Load<Texture2D>("spritemap");
            animalatlas = Content.Load<Texture2D>("animalmap");
            textbox = Content.Load<Texture2D>("textbox");
            selectionbox = Content.Load<Texture2D>("selection");
            font = Content.Load<SpriteFont>("Font");

            GD = GraphicsDevice;
            interaction = new Interaction(windowsize);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();
            var state = Mouse.GetState();
            if (Input.IsKeyPressed(Keys.I))
                updatesperframe++;
            if (Input.IsKeyPressed(Keys.K))
                updatesperframe--;
            updatesperframe = Math.Max(updatesperframe, 1);
            updatesperframe = Math.Min(updatesperframe, 10);
            initscrollwheel = state.ScrollWheelValue;
            for(int i = 0; i < updatesperframe; i++)
            {
                SimUpd(gameTime);
            }
            
            interaction.Update();
            Inventory.Update();
            Input.Update();
            base.Update(gameTime);
        }

        public void SimUpd (GameTime gameTime)
        {
            timecycle.Update();
            for (int i = 0; i < Timers.Count; i++)
            {
                var tim = Timers[i];
                tim.CheckTime();
            }
            colm.Update();
            area.Update(gameTime);
            //if(col == 0)
            //{
            //    Exit();
            //}
            

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            var dm = GraphicsDevice.DisplayMode;
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.NonPremultiplied);
            Tile[,,] tilemap = Area.tiles;
            var scale = 2.5f;
            var alpha = ((float)Math.Sin( 2 * (TimeCycle.TotalHours / 24f)*Math.PI - (Math.PI / 2)) / scale) + 1 - (1f/scale);
            Color darkness = new Color(255, 255, 255, alpha);
            var windowoffset = (windowsize * 4 / pxlratio);
            var unitnormalizer = tilesize * pxlratio;
            int startingx = Math.Max(0, (Camera.X * pxlratio) - windowoffset) / unitnormalizer;
            int endx = Math.Min((tilemap.GetUpperBound(0) + 1) * pxlratio * tilesize, (Camera.X * pxlratio) + windowoffset) / unitnormalizer;
            int startingy = Math.Max(0, (Camera.Y * pxlratio) - windowoffset) / (tilesize * pxlratio);
            int endy = Math.Min((tilemap.GetUpperBound(0) + 1) * pxlratio * tilesize, (Camera.Y * pxlratio) + windowoffset) / unitnormalizer;
            for (int x = startingx; x < endx; x++)
            {
                for (int y = startingy; y < endy; y++)
                {
                    //for (int layer = 0; layer < tilemap.GetUpperBound(2) + 1; layer++)
                    //{
                        Rectangle destrect = new Rectangle((x * tilesize - Camera.X) * pxlratio, (y * tilesize - Camera.Y) * pxlratio, tilesize * pxlratio, tilesize * pxlratio);

                        int xsource = ((int)tilemap[x, y, currentlayer] % (tileatlas.Width / tilesize)) * tilesize;
                        int ysource = (int)Math.Floor((decimal)(tilemap[x, y, currentlayer]) / (tileatlas.Width / tilesize)) * tilesize;

                        Rectangle sourcerect = new Rectangle(xsource, ysource, tilesize, tilesize);
                        spriteBatch.Draw(tileatlas, destrect, sourcerect, darkness);
                    //}
                }
            }

            area.Draw(spriteBatch, pxlratio, tilesize, spriteatlas, animalatlas, Color.White);

            var colonist = colm.SelectedColonist;
            if(colonist != null)
            {
                var pos = colonist.pos;
                Rectangle desrect = new Rectangle((pos.X * tilesize - Camera.X) * pxlratio, (pos.Y * tilesize - Camera.Y) * pxlratio, tilesize * pxlratio, tilesize * pxlratio);
                spriteBatch.Draw(selectionbox, desrect, Color.Gold);
            }

            spriteBatch.DrawString(font, "Time: " + TimeCycle.Hours + ":" + (TimeCycle.Minutes - (TimeCycle.Hours * 60)), Vector2.Zero, Color.White);
            spriteBatch.DrawString(font, "Time Scale: " + updatesperframe, new Vector2(0, 20f), Color.White);

            spriteBatch.Draw(textbox, new Rectangle(0, windowsize - (tilesize * 16), windowsize, tilesize * 16), Color.White);
            var ent = CurrentEntity(area);
            if(ent != null)
            {
                spriteBatch.DrawString(font, ent.Description, new Vector2(308, windowsize - (tilesize * 16) + 1), Color.White);
            }
            spriteBatch.DrawString(font, "X: " + Input.MouseTileX().ToString(), new Vector2(458, windowsize - (tilesize * 16) + 1), Color.White);
            spriteBatch.DrawString(font, "Y: " + Input.MouseTileY().ToString(), new Vector2(458, windowsize - (tilesize * 16) + 19), Color.White);
            var items = new List<ItemType>(Inventory.items.Keys);
            var amounts = new List<int>(Inventory.items.Values);
            for (int i = 0; i < Inventory.items.Count; i++)
            {
                if(amounts[i] == 1)
                    spriteBatch.DrawString(font, items[i].ToString(), new Vector2(Inventory.xpos, Inventory.ypos + i * 18), Color.White);
                else
                    spriteBatch.DrawString(font, items[i] + " x " + amounts[i], new Vector2(Inventory.xpos, Inventory.ypos + i * 18), Color.White);
            }
                

            for (int i = 0; i < Inventory.craftinglist.Count; i++)
            {
                var it = Inventory.craftinglist[i];
                spriteBatch.DrawString(font, it.ToString(), new Vector2(Inventory.xpos + Inventory.craftingoffset, Inventory.ypos + i * 18), Color.White);
            }

            interaction.Draw(spriteBatch);
            Inventory.Draw(spriteBatch, selectionbox);

            colm.Draw(spriteBatch, selectionbox);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public Entity CurrentEntity(Area area)
        {
            var state = Mouse.GetState();
            XY camerapos = new XY();
            camerapos.X = state.X / Simulation.pxlratio;
            camerapos.Y = state.Y / Simulation.pxlratio;
            camerapos.X += Camera.X;
            camerapos.Y += Camera.Y;
            camerapos.X /= Simulation.tilesize;
            camerapos.Y /= Simulation.tilesize;

            var entity = area.GetEntity(camerapos);
            return entity;
        }

        public static void Pause(float time)
        {
            inst.IsFixedTimeStep = true;
            inst.TargetElapsedTime = new TimeSpan(0, 0, 0, 0, (int)(time * 1000));
        }
        public static void AddEntity (Entity e)
        {
            inst.area.AddEntity(e);
        }
        public static void RemoveEntity(Entity e)
        {
            inst.area.RemoveEntity(e);
        }
    }
}
