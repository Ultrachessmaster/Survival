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
        public static int TotalHours { get { return (int)Math.Floor(halfminutes / 120d); } }
        public static int Hours { get { return (int)Math.Floor(halfminutes / 120d) % 24; } }
        public static int Minutes { get { return halfminutes / 2; } }
        static int halfminutes = 0;
        public void Update ()
        {
            halfminutes++;
        }
    }
}
