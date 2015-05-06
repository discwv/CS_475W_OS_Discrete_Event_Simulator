using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CS_475W_Discrete_Event_Simulator
{
    class PID
    {
        private int submissionTime;
        private int firstResponse;
        private int finishTime;
        private List<int> myOriginalBurstTimes;
        private List<int> myRemainingBurstTimes;
        private int myCurrentBurst;
        private int myLastTimePoint;
        private bool currentlyRunning;
        public int ID { get; private set; }
        public int CPUNum;
        public List<int> extraInformation;
        public StreamWriter myOutput;

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
            if (myCurrentBurst == 0) firstResponse = simulationTime;
            if (currentlyRunning == true)
            {
                throw new Exception("PID #" + ID + " Already Running.");
            }
            currentlyRunning = true;
            //  TODO: Implement time keeping
            myLastTimePoint = simulationTime;
            if (false)
            {
                myOutput.WriteLine("PID #" + ID + ": Started at " + simulationTime + ", remaining burst time: " + myRemainingBurstTimes[myCurrentBurst]);
                myOutput.Flush();
            }
        }

        public void EndRunning(int simulationTime)
        {
            if (false)
            {
                myOutput.WriteLine("finish io");
                myOutput.Flush();
            }
            if (currentlyRunning == false)
            {
                throw new Exception("PID #" + ID + " Not Running.");
            }
            currentlyRunning = false;

            // TODO: Implement time keeping
            int passedTime = simulationTime - myLastTimePoint;
            myLastTimePoint = simulationTime;
            myRemainingBurstTimes[myCurrentBurst] -= passedTime;
            if(false){
            myOutput.WriteLine("PID #" + ID + ": Stopped at " + simulationTime + ", remaining burst time: " + myRemainingBurstTimes[myCurrentBurst]);
            myOutput.Flush();
            }
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
                finishTime = simulationTime;
                myOutput.WriteLine(ID + "," + (firstResponse - submissionTime) + "," + (finishTime - submissionTime));
                myOutput.Flush();
            }
        }

        public PID(int newID, List<int> bursts, StreamWriter output, int submitionTime, int cpu = 0)
        {
            submissionTime = submitionTime;
            ID = newID;
            myOriginalBurstTimes = myRemainingBurstTimes = bursts;
            myCurrentBurst = 0;
            currentlyRunning = false;
            extraInformation = new List<int>();
            CPUNum = cpu;
            myOutput = output; //new StreamWriter(ID + "PID.txt");
            if (false)
            {
                myOutput.WriteLine("even here");
                myOutput.Flush();
            }
        }

        // TODO: Implement time tracking in PID

    }
}
