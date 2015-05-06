using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_475W_Discrete_Event_Simulator
{
    class Discrete_Event
    {
        /* Event Definers */
        public const int NEW_JOB = 0;
        public const int CPU_OP_DONE = 1;
        public const int IO_OP_DONE = 2;
        public const int MOVE_THREAD_TO_IO = 3;
        // TODO: Add any other event cases

        public List<int> EventInformation;
        public string ID { get; private set; }
        public int TimeOf { get; private set; }
        public int MyType { get; private set; }
        public Discrete_Event(int timeOf, int jobDefinition, string id, List<int> eventInformation = null)
        {
            TimeOf = timeOf;
            MyType = jobDefinition;
            ID = id;
            EventInformation = eventInformation;
            if (EventInformation == null) 
            { 
                EventInformation = new List<int>(); 
            }
        }
    }
}
