using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class EntityHighlight
    {
        public static string CurrentDesc(List<Entity> entities)
        {
            var state = Mouse.GetState();
            var cameraposx = state.X / Simulation.pxlratio;
            var cameraposy = state.Y / Simulation.pxlratio;
            cameraposx += Camera.X;
            cameraposy += Camera.Y;
            cameraposx /= Simulation.tilesize;
            cameraposy /= Simulation.tilesize;
            Entity currententity = null;
            if (entities == null)
                Console.WriteLine("Entity list null!");
            if (entities.Count == 0)
                return "";
            foreach (Entity e in entities)
            {
                if(e == null)
                {
                    Console.WriteLine("Entity null!");
                }
                int ex = (int)Math.Floor(e.pos.X / Simulation.tilesize);
                int ey = (int)Math.Floor(e.pos.Y / Simulation.tilesize);
                if (ex == cameraposx && ey == cameraposy)
                {
                    currententity = e;
                }
            }
            if(currententity == null)
            {
                return "";
            }
            return currententity.Description;
        }
    }
}
