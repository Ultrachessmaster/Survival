using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class TimeCycle
    {
        public static int UpdatePerFrame { get { return updatesperframe; } }
        public static int TotalHours { get { return (int)Math.Floor(minutes / 60d); } }
        public static int Hours { get { return (int)Math.Floor(minutes / 60d) % 24; } }
        public static int Minutes { get { return minutes; } }
        static int minutes = 0;
        static int updatesperframe = 1;
        public void Update ()
        {
            updatesperframe = (Mouse.GetState().ScrollWheelValue / 120) + 1;
            minutes++;
        }
    }
}
