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
    class Interaction
    {
        int screenheight;
        static Menu menu;
        public static Tool CurrentTool { get { return menu.tool; } set { menu.tool = value; } }
        int textboxborder;
        int previouswheelv = 0;

        public Interaction (int screenheight)
        {
            this.screenheight = screenheight;
            menu = new Menu(screenheight);
            textboxborder = screenheight - (Simulation.tilesize * 16);
        }

        public void Update()
        {
            menu.Update();

            MoveCamera();
            SetLayer();
        }

        void MoveCamera()
        {
            var sw = Mouse.GetState().ScrollWheelValue;
            Simulation.pxlratio += ((sw - previouswheelv) / 120);
            previouswheelv = sw;
            Simulation.pxlratio = Math.Min(4, Simulation.pxlratio);
            Simulation.pxlratio = Math.Max(1, Simulation.pxlratio);
            if (Input.IsKeyDown(Keys.D))
                Camera.X += Simulation.tilesize;
            if (Input.IsKeyDown(Keys.A))
                Camera.X -= Simulation.tilesize;
            if (Input.IsKeyDown(Keys.W))
                Camera.Y -= Simulation.tilesize;
            if (Input.IsKeyDown(Keys.S))
                Camera.Y += Simulation.tilesize;
        }

        void SetLayer()
        {
            if (Input.IsKeyPressed(Keys.E))
                Simulation.currentlayer++;
            if (Input.IsKeyPressed(Keys.Q))
                Simulation.currentlayer--;
            Simulation.currentlayer = Math.Max(0, Simulation.currentlayer);
            Simulation.currentlayer = Math.Min(Area.tiles.GetUpperBound(2), Simulation.currentlayer);
        }

        public void Draw(SpriteBatch sb)
        {
            menu.Draw(sb);
        }
    }
}
