using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    public class Entity {
        public TextureAtlas tex;
	    public int Sprite
        {
            get { return sprite; }
            set { sprite = value; }
        }
        private int sprite;
        protected int width = 8;
        protected int height = 8;
        
        public XY pos;
        public Vector2 scale = new Vector2(1, 1);
        public float rotation;
        public RefWrapper<bool> enabled = new RefWrapper<bool>(true);

        //Kinda kludgy
        public string Tag { get { return tag; } }
        protected string tag = "";

        public string Description { get { if (GetDescription != null) { return GetDescription(); } else return ""; } }
        protected Func<string> GetDescription;

        protected bool visible = true;

        public Action<GameTime> Update { get { return update; } }
        protected Action<GameTime> update;

        public Action<SpriteBatch, int, int, Texture2D, Texture2D, Color> Draw { get { return draw; } }
        protected Action<SpriteBatch, int, int, Texture2D, Texture2D, Color> draw;

        protected void Drw (SpriteBatch sb, int pxlratio, int tilesize, Texture2D spriteatlas, Texture2D animalatlas, Color col)
        {
            if (enabled.Value)
            {
                int texwidth = 0;
                int texheight = 0;
                switch (tex)
                {
                    case TextureAtlas.SPRITES:
                        texwidth = spriteatlas.Width;
                        texheight = spriteatlas.Height;
                        break;
                    case TextureAtlas.ANIMALS:
                        texwidth = animalatlas.Width;
                        texheight = animalatlas.Height;
                        break;

                }
                int xsource = (Sprite % (texwidth / tilesize)) * tilesize;
                int ysource = (int)Math.Floor((decimal)(Sprite) / (texheight / tilesize)) * tilesize;

                Rectangle sourcerect = new Rectangle(xsource, ysource, tilesize, tilesize);

                if (visible)
                {
                    switch (tex)
                    {
                        case TextureAtlas.SPRITES:
                            sb.Draw(spriteatlas, new Rectangle((pos.X * Simulation.tilesize - Camera.X) * pxlratio, (pos.Y * Simulation.tilesize - Camera.Y) * pxlratio, width * pxlratio, height * pxlratio), sourcerect, col);
                            break;
                        case TextureAtlas.ANIMALS:
                            sb.Draw(animalatlas, new Rectangle((pos.X * Simulation.tilesize - Camera.X) * pxlratio, (pos.Y * Simulation.tilesize - Camera.Y) * pxlratio, width * pxlratio, height * pxlratio), sourcerect, col);
                            break;

                    }
                }
                    
                
                    
            }
            
        }
    }
}
