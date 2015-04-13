using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_475W_Discrete_Event_Simulator
{
    class PID
    {
        private List<int> myOriginalBurstTimes;
        private List<int> myRemainingBurstTimes;
        private int myCurrentBurst;
        private int myLastTimePoint;
        private bool currentlyRunning;
        public int ID { get; private set; }
        public int CPUNum;
        public List<int> extraInformation;

        // NOTE: Value of -1 means thread has completed.
        public int CurrentBurstLength {
            get { return myRemainingBurstTimes[myCurrentBurst]; }
            private set { }
        }

        public bool CurrentBurstIsCPUBurst
        {
            get { return myCurrentBurst % 2 == 0 ? true : false; }
            private set { }
        }

        public void StartRunning(int simulationTime)
        {
            if (currentlyRunning == true)
            {
                throw new Exception("PID #" + ID + " Already Running.");
            }
            currentlyRunning = true;

            //  TODO: Implement time keeping
            myLastTimePoint = simulationTime;
        }

        public void EndRunning(int simulationTime)
        {
            if (currentlyRunning == false)
            {
                throw new Exception("PID #" + ID + " Not Running.");
            }
            currentlyRunning = false;

            // TODO: Implement time keeping
            int passedTime = simulationTime - myLastTimePoint;
            myLastTimePoint = simulationTime;
            myRemainingBurstTimes[myCurrentBurst] -= passedTime;
            if (myRemainingBurstTimes[myCurrentBurst] == 0)
            {
                myCurrentBurst++;
            }
            else if (myRemainingBurstTimes[myCurrentBurst] < 0)
            {
                throw new Exception("PID #" + ID + " Passed Completion.");
            }

            if (myCurrentBurst == myRemainingBurstTimes.Count)
            {
                myRemainingBurstTimes.Add(-1);
            }
        }

        public PID(int newID, List<int> bursts, int cpu = 0)
        {
            ID = newID;
            myOriginalBurstTimes = myRemainingBurstTimes = bursts;
            myCurrentBurst = 0;
            currentlyRunning = false;
            extraInformation = new List<int>();
            CPUNum = cpu;
        }

        // TODO: Implement time tracking in PID

    }
}
