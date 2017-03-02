using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    interface IAnimal
    {
        float Intimidation { get; }
        float Satiation { get; }
        bool Dead { get; }
        void TakeDamage(float bloodlossrate);
    }
}
