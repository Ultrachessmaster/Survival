using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    public class Timer
    {
        Action<float> action;
        float time;
        int currenttime;
        float arg;
        /*public Timer(Action<int> act, float time, int argument = 0)
        {
            action = act;
            this.time = time;
            currenttime = TimeCycle.TotalHours;
            arg = argument;
            Simulation.Timers.Add(this);
        }*/

        public Timer(Action<float> act, float time, float argument = 0)
        {
            action = act;
            this.time = time;
            currenttime = TimeCycle.Minutes;
            arg = argument;
            Simulation.Timers.Add(this);
        }

        public void CheckTime()
        {
            float deltatime = TimeCycle.Minutes - currenttime;
            if ((time * 60f) <= deltatime)
            {
                float overtime = deltatime - (time * 60f);
                action.Invoke(overtime);
                Simulation.Timers.Remove(this);
            }
            
        }

    }
}
