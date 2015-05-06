using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CS_475W_Discrete_Event_Simulator
{
    class CPU
    {
        //TODO: Implement CPU

        private const int READY = 0;
        private const int REPLACING_THREAD = 1;
        private const int ADDING_NEW_THREAD = 2;
        private const int REMOVING_LAST_THREAD = 3;
        private const int RUNNING_THREAD = 4;
        private int myInternalState;

        private StreamWriter myOutputFile;
        private int nextThreadQueueNum;
        private int newEventID;
        private List<List<PID>> myWaitingQueues;
        private List<int> myTimeQuantums;
        private PID myCurrentlyRunningPID;
        private PID moveToIO;
        private string myNextEventID;
        public string ID { get; private set; }

        public CPU(string id, StreamWriter outputFile)
        {
            ID = id;
            myWaitingQueues = new List<List<PID>>();
            myTimeQuantums = new List<int>();
            myOutputFile = outputFile;
            for (int i = 0; i < Constants.NUM_MULTILEVEL_QUEUES; i++)
            {
                myWaitingQueues.Add(new List<PID>());
                myTimeQuantums.Add((new Constants()).TIME_QUANTUM_PER_LEVEL[i]);
            }
            myInternalState = READY;
            newEventID = 0;
            nextThreadQueueNum = -1;
        }

        public Discrete_Event AddThread(PID newThread, int simulationTime)
        {
            Discrete_Event newEvent = null;
            newThread.extraInformation = new List<int>();
            newThread.extraInformation.Add(0);
            myWaitingQueues[0].Add(newThread);
            // TODO: Check if there's a process running currently
            if (myInternalState == READY)
            {
                newEvent = EventReached(simulationTime, null);
            }
            return newEvent;
        }

        public Discrete_Event EventReached(int simulationTime, Discrete_Event currentEvent)
        {
            myOutputFile.Write(myInternalState + " - ");
            int lastState = myInternalState;
            Discrete_Event newEvent = null;
            int newEventType = -1;
            List<int> newEventInfo = null;
            int newRunTime = 0;
            if (currentEvent != null && currentEvent.ID != myNextEventID) return newEvent;
            if (myCurrentlyRunningPID != null && myCurrentlyRunningPID.ID == 10)
            {
                double tester = 0;
            }
            switch (myInternalState)
            {
                case READY: // Load new thread (Should only occur when new thread is added)
                    if (myCurrentlyRunningPID == null)
                    {
                        // Load new thread
                        myInternalState = ADDING_NEW_THREAD;
                        nextThreadQueueNum = FindNextThread();
                        myNextEventID = ID + newEventID++;
                        newEvent = new Discrete_Event(simulationTime + Constants.TIME_QUANTUM_TO_LOAD_THREAD, Discrete_Event.CPU_OP_DONE, myNextEventID);
                    }
                    else
                    {
                        nextThreadQueueNum = FindNextThread();
                        myCurrentlyRunningPID.EndRunning(simulationTime);


                        // Remove finished thread
                        if (myCurrentlyRunningPID.CurrentBurstLength == -1) // Thread is finished
                        {
                            myCurrentlyRunningPID = null;
                        }
                        else if (myCurrentlyRunningPID.CurrentBurstIsCPUBurst == false) // Push thread to IO
                        {
                            newEventType = Discrete_Event.CPU_OP_DONE;
                            moveToIO = myCurrentlyRunningPID;
                            myCurrentlyRunningPID = null;
                        }
                        else // Move thread to correct queue
                        {
                            PlaceThreadInCorrectQueue(myCurrentlyRunningPID);
                            myCurrentlyRunningPID = null;
                        }

                        if (nextThreadQueueNum == -1 && newEventType != -1) // Save thread to push to IO
                        {
                            myInternalState = REMOVING_LAST_THREAD;
                            newEventInfo = new List<int>();
                            newEventInfo.Add(Discrete_Event.MOVE_THREAD_TO_IO);
                            myNextEventID = ID + newEventID++;
                            newEvent = new Discrete_Event(simulationTime + Constants.TIME_QUANTUM_TO_SAVE_THREAD, newEventType, myNextEventID, newEventInfo);
                        }
                        else if (nextThreadQueueNum != -1)
                        {
                            myInternalState = REPLACING_THREAD;
                            myNextEventID = ID + newEventID++;
                            if (newEventType == -1) // Swap in new thread
                            {
                                newEvent = new Discrete_Event(simulationTime + Constants.TIME_QUANTUM_TO_SAVE_THREAD + Constants.TIME_QUANTUM_TO_LOAD_THREAD,
                                Discrete_Event.CPU_OP_DONE, myNextEventID);
                            }
                            else // Push to IO
                            {
                                newEventInfo = new List<int>();
                                newEventInfo.Add(Discrete_Event.MOVE_THREAD_TO_IO);
                                newEvent = new Discrete_Event(simulationTime + Constants.TIME_QUANTUM_TO_SAVE_THREAD,
                                newEventType, myNextEventID, newEventInfo);
                            }
                        }
                        else
                        {
                            newEvent = new Discrete_Event(-1, -1, " ");
                        }
                    }
                    break;

                case REPLACING_THREAD: // Activate new thread
                    if (currentEvent.EventInformation.Count > 0 && currentEvent.EventInformation[0] == Discrete_Event.MOVE_THREAD_TO_IO)
                    {
                        // Continue loading but push to IO
                        myInternalState = ADDING_NEW_THREAD;
                        myNextEventID = ID + newEventID++;
                        newEventInfo = new List<int>();
                        newEventInfo.Add(Discrete_Event.CPU_OP_DONE);
                        newEventType = Discrete_Event.MOVE_THREAD_TO_IO;
                        newEvent = new Discrete_Event(simulationTime + Constants.TIME_QUANTUM_TO_LOAD_THREAD, newEventType, myNextEventID, newEventInfo);
                    }
                    else
                    {
                        // Finished loading
                        myInternalState = RUNNING_THREAD;
                        newRunTime = Math.Min(myTimeQuantums[nextThreadQueueNum], myWaitingQueues[nextThreadQueueNum][0].CurrentBurstLength);
                        myCurrentlyRunningPID = myWaitingQueues[nextThreadQueueNum][0];
                        myWaitingQueues[nextThreadQueueNum].RemoveAt(0);
                        nextThreadQueueNum = -1;
                        myCurrentlyRunningPID.StartRunning(simulationTime);
                        myNextEventID = ID + newEventID++;
                        newEvent = new Discrete_Event(simulationTime + newRunTime, Discrete_Event.CPU_OP_DONE, myNextEventID);
                    }
                    break;

                case ADDING_NEW_THREAD: // Activate new thread
                    myInternalState = RUNNING_THREAD;
                    newRunTime = Math.Min(myTimeQuantums[nextThreadQueueNum], myWaitingQueues[nextThreadQueueNum][0].CurrentBurstLength);
                    myCurrentlyRunningPID = myWaitingQueues[nextThreadQueueNum][0];
                    myWaitingQueues[nextThreadQueueNum].RemoveAt(0);
                    nextThreadQueueNum = -1;
                    myCurrentlyRunningPID.StartRunning(simulationTime);
                    myNextEventID = ID + newEventID++;
                    newEvent = new Discrete_Event(simulationTime + newRunTime, Discrete_Event.CPU_OP_DONE, myNextEventID);
                    break;

                case REMOVING_LAST_THREAD: // Check for new thread or enter ready state
                    newEventInfo = null;
                    if (currentEvent.EventInformation.Count > 0 && currentEvent.EventInformation[0] == Discrete_Event.MOVE_THREAD_TO_IO)
                    {
                        myOutputFile.Write("0");
                        // Push to IO
                        newEventType = Discrete_Event.MOVE_THREAD_TO_IO;
                        newEventInfo = new List<int>();
                        newEventInfo.Add(Discrete_Event.CPU_OP_DONE);
                    }
                    nextThreadQueueNum = FindNextThread();
                    if (nextThreadQueueNum == -1)
                    {
                        myOutputFile.Write("1");
                        // No more threads
                        myInternalState = READY;
                        myNextEventID = ID + newEventID++;
                        newEvent = new Discrete_Event(-1, newEventType, myNextEventID, newEventInfo);
                    }
                    else
                    {
                        myOutputFile.Write("2");
                        // More threads
                        myInternalState = ADDING_NEW_THREAD;
                        myNextEventID = ID + newEventID++;
                        if (newEventInfo == null)
                        {
                            myOutputFile.Write("4");
                            newEventType = Discrete_Event.CPU_OP_DONE;
                        }
                        newEvent = new Discrete_Event(simulationTime + Constants.TIME_QUANTUM_TO_LOAD_THREAD, newEventType, myNextEventID, newEventInfo);
                    }
                    break;

                case RUNNING_THREAD: // Remove running thread and find new thread
                    nextThreadQueueNum = FindNextThread();
                    myNextEventID = ID + newEventID++;
                    myCurrentlyRunningPID.EndRunning(simulationTime);
                    if (nextThreadQueueNum == -1)
                    {
                        // No more threads

                        if (myCurrentlyRunningPID.CurrentBurstLength == -1)
                        {
                            // Done
                            myInternalState = REMOVING_LAST_THREAD;
                            myCurrentlyRunningPID = null;
                            newEvent = new Discrete_Event(simulationTime + Constants.TIME_QUANTUM_TO_SAVE_THREAD, Discrete_Event.CPU_OP_DONE, myNextEventID);
                        }
                        else if (myCurrentlyRunningPID.CurrentBurstIsCPUBurst == false)
                        {
                            // Move to IO
                            myInternalState = REMOVING_LAST_THREAD;
                            moveToIO = myCurrentlyRunningPID;
                            myCurrentlyRunningPID = null;
                            newEventInfo = new List<int>();
                            newEventInfo.Add(Discrete_Event.MOVE_THREAD_TO_IO);
                            newEvent = new Discrete_Event(simulationTime + Constants.TIME_QUANTUM_TO_SAVE_THREAD, Discrete_Event.CPU_OP_DONE, myNextEventID, newEventInfo);
                        }
                        else
                        {
                            // Just keep going
                            myInternalState = READY;
                            newRunTime = myCurrentlyRunningPID.CurrentBurstLength;
                            myCurrentlyRunningPID.StartRunning(simulationTime);
                            newEvent = new Discrete_Event(simulationTime + newRunTime, Discrete_Event.CPU_OP_DONE, myNextEventID);
                        }
                    }
                    else
                    {
                        // More threads
                        myInternalState = REPLACING_THREAD;
                        if (myCurrentlyRunningPID.CurrentBurstLength == -1)
                        {
                            // Done
                            newEventInfo = null;
                            newEvent = new Discrete_Event(simulationTime + Constants.TIME_QUANTUM_TO_SAVE_THREAD + Constants.TIME_QUANTUM_TO_LOAD_THREAD,
                                Discrete_Event.CPU_OP_DONE, myNextEventID);
                        }
                        else if (myCurrentlyRunningPID.CurrentBurstIsCPUBurst == false)
                        {
                            // Move to IO
                            moveToIO = myCurrentlyRunningPID;
                            newEventInfo = new List<int>();
                            newEventInfo.Add(Discrete_Event.MOVE_THREAD_TO_IO);
                            newEvent = new Discrete_Event(simulationTime + Constants.TIME_QUANTUM_TO_SAVE_THREAD, Discrete_Event.CPU_OP_DONE, myNextEventID, newEventInfo);
                        }
                        else
                        {
                            // Move to correct queue
                            PlaceThreadInCorrectQueue(myCurrentlyRunningPID);
                            myCurrentlyRunningPID = null;
                            newEvent = new Discrete_Event(simulationTime + Constants.TIME_QUANTUM_TO_SAVE_THREAD + Constants.TIME_QUANTUM_TO_LOAD_THREAD,
                                Discrete_Event.CPU_OP_DONE, myNextEventID, newEventInfo);
                        }
                        myCurrentlyRunningPID = null;
                    }
                    break;
            }
            myOutputFile.WriteLine("CPU " + ID + ": " + myInternalState + " - Type: " + newEvent.MyType + " Time: " + newEvent.TimeOf);
            myOutputFile.Flush();
            if (myCurrentlyRunningPID != null && myCurrentlyRunningPID.ID == 4)
            {
                double boop = 1;
            }
            return newEvent;
        }

        public PID MovePIDToIO()
        {
            PID toMove = moveToIO;
            moveToIO = null;
            return toMove;
        }

        private int FindNextThread()
        {
            int nextThread = -1;
            for (int i = 0; i < myWaitingQueues.Count && nextThread == -1; i++)
            {
                if (myWaitingQueues[i].Count > 0) nextThread = i;
            }
            return nextThread;
        }

        private void PlaceThreadInCorrectQueue(PID thread)
        {
            int placeThreadIn = thread.extraInformation[0]++;
            if (thread.extraInformation[0] == myWaitingQueues.Count)
            {
                thread.extraInformation[0]--;
            }
            myWaitingQueues[placeThreadIn].Add(thread);
        }

    }
}
