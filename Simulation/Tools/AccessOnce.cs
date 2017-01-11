using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class AccessOnce<T1, T2>
    {
        Dictionary<T1, T2> inputs = new Dictionary<T1, T2>();
        T2 fa;
        T2 la;
        public AccessOnce(T2 firstaccess, T2 lateraccesses)
        {
            fa = firstaccess;
            la = lateraccesses;
        }

        public T2 Access(T1 key)
        {
            if(!inputs.ContainsKey(key))
                inputs[key] = la;
            if (inputs[key].Equals(la))
            {
                inputs[key] = fa;
                return fa;
            }
            return la;
        }

        public bool ContainsKey(T1 key)
        {
            return inputs.ContainsKey(key);
        }

        public void Set(T1 key, T2 value)
        {
            inputs[key] = value;
        }

    }
}
