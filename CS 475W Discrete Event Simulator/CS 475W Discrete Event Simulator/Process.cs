using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_475W_Discrete_Event_Simulator
{
    public class Process
    {
        int PID;
        int priority;
        int wait_time;
        int response_time;
        int turnaround;
        public struct burst
        {
            public int time;
            public bool CPU;
        }
        Queue<burst> burstqueue;
        int state;
    }
}
