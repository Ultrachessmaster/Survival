﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    public class Entity {
	    public int Sprite
        {
            get { return sprite; }
            set { sprite = value; }
        }
        private int sprite;
        private int width = 8;
        private int height = 8;
        
        public Vector2 pos;
        public Vector2 scale = new Vector2(1, 1);
        public float rotation;

        //Kinda kludgy
        public string Tag { get { return tag; } }
        protected string tag = "";

        protected bool visible = true;

        public Action<GameTime> Update { get { return update; } }
        protected Action<GameTime> update;

        public Action<SpriteBatch, int, Texture2D, Rectangle> Draw { get { return draw; } }
        protected Action<SpriteBatch, int, Texture2D, Rectangle> draw;

        protected void Drw (SpriteBatch sb, int pxlratio, Texture2D tex, Rectangle sourcerec)
        {
            if (visible)
            {
                sb.Draw(tex, new Rectangle(((int)Math.Round(pos.X) - Camera.X) * pxlratio, ((int)Math.Round(pos.Y) - Camera.Y) * pxlratio, width * pxlratio, height * pxlratio), sourcerec, Color.White);

            }
        }
    }
}