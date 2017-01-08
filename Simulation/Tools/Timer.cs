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
        RefWrapper<bool> enabled;
        /*public Timer(Action<int> act, float time, int argument = 0)
        {
            action = act;
            this.time = time;
            currenttime = TimeCycle.TotalHours;
            arg = argument;
            Simulation.Timers.Add(this);
        }*/

        public Timer(Action<float> act, float time, RefWrapper<bool> enabled = null)
        {
            action = act;
            this.time = time;
            currenttime = TimeCycle.Minutes;
            this.enabled = enabled;
            Simulation.Timers.Add(this);
        }

        public void CheckTime()
        {
            float deltatime = TimeCycle.Minutes - currenttime;
            if ((time * 60f) <= deltatime)
            {
                float overtime = deltatime - (time * 60f);
                if(enabled == null || enabled.Value)
                    action.Invoke(overtime);
                Simulation.Timers.Remove(this);
            }
            
        }

    }
}
