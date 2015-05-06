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
            int lastpid = myCurrentPID != null ? myCurrentPID.ID : -1;
            if (lastpid == 4)
            {
                double pauasee = 0;
            }
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
                    myInternalState = WORKING;
                    break;

                case WORKING:
                    myCurrentPID.EndRunning(simulationTime);
                    string outmessage = null;
                    if (myCurrentPID.CurrentBurstLength != -1)
                    {
                        toIO = myCurrentPID;
                        outmessage = " ";
                    }
                    else
                    {
                        int dob = 0;
                    }
                    
                    if (Threads.Count > 0)
                    {
                        // More threads
                        myCurrentPID = Threads[0];
                        Threads.RemoveAt(0);
                        runTime = myCurrentPID.CurrentBurstLength;
                        myCurrentPID.StartRunning(simulationTime);
                        newEvent = new Discrete_Event(simulationTime + runTime, Discrete_Event.IO_OP_DONE, outmessage);
                    }
                    else
                    {
                        // No more threads
                        newEvent = new Discrete_Event(-1, Discrete_Event.IO_OP_DONE, outmessage);
                        myInternalState = READY;
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
