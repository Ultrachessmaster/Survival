using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class Animation
    {
        Texture2D[] texs;
        int idx = 0;
        public bool loop;
        float speedinseconds;

        public Animation (string texturepath, int framelength, bool loop, float speedinseconds)
        {
            this.loop = loop;
            this.speedinseconds = speedinseconds;
            SetTextures(texturepath, framelength);
            Update(0);
        }

        void Update(float overtime)
        {
            if(loop) { idx = (idx + 1) % texs.Length; }
            else { idx = Math.Min(idx + 1, texs.Length - 1); }
            Timer timer = new Timer(Update, speedinseconds);
        }

        public Texture2D CurrentTexture()
        {
            return texs[idx];
        }

        void SetTextures(string animation, int length)
        {
            texs = new Texture2D[length];
            for (int i = 0; i < texs.Length; i++)
            {
                texs[i] = Simulation.CM.Load<Texture2D>(animation + i.ToString());
            }
        }
    }
}
