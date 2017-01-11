using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    public struct XY
    {
        public int X;
        public int Y;

        public XY(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public static XY operator +(XY a, XY b)
        {
            return new XY(a.X + b.X, a.Y + b.Y);
        }
        public static XY operator +(XY a, int b)
        {
            return new XY(a.X + b, a.Y + b);
        }
        public static XY operator -(XY a, XY b)
        {
            return new XY(a.X - b.X, a.Y - b.Y);
        }
        public static XY operator -(XY a)
        {
            return new XY(-a.X, -a.Y);
        }
        public static XY operator -(XY a, int b)
        {
            return new XY(a.X - b, a.Y - b);
        }
        public static XY Zero { get { return new XY(); } }
        public static bool operator ==(XY a, XY b)
        {
            return (a.X == b.X && a.Y == b.Y);
        }
        public static bool operator !=(XY a, XY b)
        {
            return (a.X != b.X || a.Y != b.Y);
        }

    }
}
