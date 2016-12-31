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

            /*if (!keys.ContainsKey(key))
                inputs[key] = la;*/

            if (ks.IsKeyUp(key))
            {
                keys.Set(key, false);
            }

            if (ks.IsKeyDown(key)) {
                /*if (!keys[key])
                {
                    keys[key] = true;
                    return true;
                }*/
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
    }
}
