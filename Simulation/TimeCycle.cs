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
        public static int TotalHours { get { return (int)Math.Floor(eighthminutes / (8*60d)); } }
        public static int Hours { get { return (int)Math.Floor(eighthminutes / (8*60d)) % 24; } }
        public static int Minutes { get { return eighthminutes / 8; } }
        static int eighthminutes = 6 * 60 * 8;
        public void Update ()
        {
            eighthminutes++;
        }
    }
}
