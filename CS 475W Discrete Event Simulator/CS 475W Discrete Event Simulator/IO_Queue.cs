using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_475W_Discrete_Event_Simulator
{
    class IO_Queue
    {
        // TODO: Implement IO_Queue
        private const int READY = 0;
        private const int WORKING = 1;
        private int myInternalState;
        
        private List<PID> Threads;
        private PID myCurrentPID;
        private PID toIO;

        public IO_Queue()
        {
            myInternalState = READY;
            Threads = new List<PID>();
            myCurrentPID = null;
        }

        public Discrete_Event AddNewThread(int simulationTime, PID newThread)
        {
            Discrete_Event newEvent = null;
            Threads.Add(newThread);
            if (myInternalState == READY)
            {
                newEvent = EventReached(simulationTime, null);
            }
            return newEvent;
        }

        public Discrete_Event EventReached(int simulationTime, Discrete_Event currentEvent)
        {
            Discrete_Event newEvent = null;
            int runTime = 0;
            switch (myInternalState)
            {
                case READY:
                    myCurrentPID = Threads[0];
                    Threads.RemoveAt(0);
                    runTime = myCurrentPID.CurrentBurstLength;
                    myCurrentPID.StartRunning(simulationTime);
                    newEvent = new Discrete_Event(simulationTime + runTime, Discrete_Event.IO_OP_DONE, null);
                    break;

                case WORKING:
                    myCurrentPID.EndRunning(simulationTime);
                    toIO = myCurrentPID;
                    if (Threads.Count > 0)
                    {
                        // More threads
                        myCurrentPID = Threads[0];
                        Threads.RemoveAt(0);
                        runTime = myCurrentPID.CurrentBurstLength;
                        myCurrentPID.StartRunning(simulationTime);
                        newEvent = new Discrete_Event(simulationTime + runTime, Discrete_Event.IO_OP_DONE, " ");
                    }
                    else
                    {
                        // No more threads
                        newEvent = new Discrete_Event(-1, Discrete_Event.IO_OP_DONE, " ");
                    }
                    break;
            }
            return newEvent;
        }

        public PID ReturnToCPU()
        {
            PID toReturn = toIO;
            toIO = null;
            return toReturn;
        }
    }
}
