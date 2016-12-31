//using Box2DX.Common;
using Microsoft.Xna.Framework;
using System;

namespace Simulation
{
    static class StaticExtension
    {
        static System.Random ra = new System.Random();
        public static float Cross(this Vector2 v, Vector2 a)
        {
            return 0;
        }

        public static void SetZero(this Vector2 v)
        {
            v.X = 0;
            v.Y = 0;
        }

        public static void Set(this Vector2 v, float x, float y)
        {
            v.X = x;
            v.Y = y;
        }
        public static float Range(this Random r, float min, float max)
        {
            return (float)ra.NextDouble() * (max - min) + min;
        }
    }
}
