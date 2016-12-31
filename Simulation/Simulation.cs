﻿using Microsoft.Xna.Framework;
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
        WorldEditor we;
        TimeCycle timecycle = new TimeCycle();
        Crops crops;
        public static List<Timer> Timers = new List<Timer>();
        public static int tilesize = 8;
        public static int pxlratio = 4;
        public static int windowsize { get; internal set; }
        private Texture2D tileatlas;
        private Texture2D spriteatlas;
        public static Simulation inst;

        public static ContentManager CM { get; internal set; }
        public static GraphicsDevice GD { get; internal set; }

        public Simulation()
        {
            windowsize = 25 * tilesize * pxlratio;
            
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
            var size = new Vector3(512, 512, 1);
            var map = Generation.Generate(size);
            area = new Area(map);
            crops = new Crops(area);
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
            font = Content.Load<SpriteFont>("Font");
            GD = GraphicsDevice;
            we = new WorldEditor(windowsize);
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
            for(int i = 0; i < TimeCycle.UpdatePerFrame; i++)
            {
                SimUpd(gameTime);
            }
            
            we.Update(area);
            base.Update(gameTime);
        }

        public void SimUpd (GameTime gameTime)
        {
            area.Upd();
            crops.Upd();
            
            timecycle.Update();
            for (int i = 0; i < Timers.Count; i++)
            {
                var tim = Timers[i];
                tim.CheckTime();
            }
            for (int i = 0; i < area.entities.Count; i++)
            {
                var ent = area.entities[i];
                if (ent.Update != null)
                    ent.Update.Invoke(gameTime);
            }
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
            int[,,] tilemap = area.tiles;
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
                    for (int layer = 0; layer < tilemap.GetUpperBound(2) + 1; layer++)
                    {
                        Rectangle destrect = new Rectangle((x * tilesize - Camera.X) * pxlratio, (y * tilesize - Camera.Y) * pxlratio, tilesize * pxlratio, tilesize * pxlratio);

                        int xsource = ((tilemap[x, y, layer]) % (tileatlas.Width / tilesize)) * tilesize;
                        int ysource = (int)Math.Floor((decimal)(tilemap[x, y, layer]) / (tileatlas.Width / tilesize)) * tilesize;

                        Rectangle sourcerect = new Rectangle(xsource, ysource, tilesize, tilesize);
                        spriteBatch.Draw(tileatlas, destrect, sourcerect, darkness);
                    }
                }
            }
            for (int i = 0; i < area.entities.Count; i++)
            {
                Entity e = area.entities[i];

                int xsource = (e.Sprite % (tileatlas.Width / tilesize)) * tilesize;
                int ysource = (int)Math.Floor((decimal)(e.Sprite) / (tileatlas.Width / tilesize)) * tilesize;

                Rectangle sourcerect = new Rectangle(xsource, ysource, tilesize, tilesize);

                e.Draw.Invoke(spriteBatch, pxlratio, spriteatlas, sourcerect);
                
            }
            spriteBatch.DrawString(font, "Time: " + TimeCycle.Minutes, Vector2.Zero, Color.White);
            spriteBatch.DrawString(font, "Time Scale: " + (1 + (Mouse.GetState().ScrollWheelValue / 120)), new Vector2(0, 20f), Color.White);
            spriteBatch.DrawString(font, "Crops: " + Crops.harvestedcrops, new Vector2(0, 40f), Color.White);

            we.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
        public static void Pause(float time)
        {
            inst.IsFixedTimeStep = true;
            inst.TargetElapsedTime = new TimeSpan(0, 0, 0, 0, (int)(time * 1000));
        }
        public static void AddEntity (Entity e)
        {
            inst.area.entities.Add(e);
        }
        public static void DestroyEntity(Entity e)
        {
            inst.area.entities.Remove(e);
        }
    }
}