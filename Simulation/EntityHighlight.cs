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
        public static Entity CurrentEntity(List<Entity> entities)
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
                return null;
            foreach (Entity e in entities)
            {
                int ex = e.pos.X;
                int ey = e.pos.Y;
                if (ex == cameraposx && ey == cameraposy)
                {
                    currententity = e;
                }
            }
            if(currententity == null)
            {
                return null;
            }
            return currententity;
        }
    }
}
