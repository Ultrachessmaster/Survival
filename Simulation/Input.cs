using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Simulation
{
    class Input {
        static AccessOnce<Keys, bool> keys = new AccessOnce<Keys, bool>(true, false);
        static AccessOnce<int, bool> mouseb = new AccessOnce<int, bool>(true, false);
        public static bool IsKeyPressed (Keys key) {
            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyUp(key))
            {
                keys.Set(key, false);
            }

            if (ks.IsKeyDown(key)) {
                return keys.Access(key);
            }
            
            return false;
        }

        public static bool IsKeyDown (Keys key)
        {
            KeyboardState ks = Keyboard.GetState();
            return ks.IsKeyDown(key);
        }

        public static bool IsMouseButtonPressed(int button)
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
                mouseb.Set(button, false);
            
            if(buttonpressed == ButtonState.Pressed)
                return mouseb.Access(button);

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
    }
}
