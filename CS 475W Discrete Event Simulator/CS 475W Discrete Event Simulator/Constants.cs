using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_475W_Discrete_Event_Simulator
{
    class Constants
    {
        public const int NUM_MULTILEVEL_QUEUES = 2;
        public int[] TIME_QUANTUM_PER_LEVEL = { 2, 4, 8 };
        public const int TIME_QUANTUM_TO_SAVE_THREAD = 2;
        public const int TIME_QUANTUM_TO_LOAD_THREAD = 2;
        public const int NUM_CPU = 4;

    }
}
