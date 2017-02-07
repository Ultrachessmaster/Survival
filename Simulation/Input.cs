using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Simulation
{
    class Input {
        static AccessOnce<Keys, bool> keyaccess = new AccessOnce<Keys, bool>(true, false);
        static AccessOnce<int, bool> mousebaccess = new AccessOnce<int, bool>(true, false);
        static Dictionary<Keys, bool> keys = new Dictionary<Keys, bool>();
        static Dictionary<int, bool> mousebts = new Dictionary<int, bool>();
        static bool KeyPressed (Keys key) {
            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyUp(key))
            {
                keyaccess.Set(key, false);
            }

            if (ks.IsKeyDown(key)) {
                return keyaccess.Access(key);
            }
            
            return false;
        }

        public static bool IsKeyDown (Keys key)
        {
            KeyboardState ks = Keyboard.GetState();
            return ks.IsKeyDown(key);
        }

        static bool MouseButtonPressed(int button)
        {
            var state = Mouse.GetState();
            ButtonState buttonpressed = ButtonState.Released;
            switch (button)
            {
                case 0: buttonpressed = state.LeftButton; break;
                case 1: buttonpressed = state.RightButton; break;
                case 2: buttonpressed = state.MiddleButton; break;
            }
            if (buttonpressed == ButtonState.Released)
                mousebaccess.Set(button, false);
            
            if(buttonpressed == ButtonState.Pressed)
                return mousebaccess.Access(button);
            var st = Mouse.GetState();
            return false;
            
        }

        public static int MouseTileX()
        {
            var mstate = Mouse.GetState();
            return Math.Max(((mstate.X / Simulation.pxlratio) + Camera.X) / Simulation.tilesize, 0);
        }
        public static int MouseTileY()
        {
            var mstate = Mouse.GetState();
            return Math.Max(((mstate.Y / Simulation.pxlratio) + Camera.Y) / Simulation.tilesize, 0);
        }

        public static void Update()
        {
            keys = new Dictionary<Keys, bool>();
            mousebts = new Dictionary<int, bool>();
        }

        public static bool IsKeyPressed(Keys key)
        {
            if(!keys.ContainsKey(key))
                keys[key] = KeyPressed(key);
            return keys[key];
        }

        public static bool IsMouseButtonPressed(int button)
        {
            if (!mousebts.ContainsKey(button))
                mousebts[button] = MouseButtonPressed(button);
            return mousebts[button];
        } 
    }
}
