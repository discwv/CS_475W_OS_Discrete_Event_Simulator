using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CS_475W_Discrete_Event_Simulator
{
    class Simulation
    {
        StreamWriter PIDOutput;
        StreamWriter CPUOutput;
        StreamWriter QueueOutput;
        Generated_Jobs MySimulationJobs;
        Event_Priority_Queue MyEvents;
        IO_Queue MyIOQueue;
        List<CPU> MyCPUs;
        int CPURotation;
        int newThreadIDs;

        public Simulation(string inputFile, string PIDOutputFile, string CPUOutputFile, string QueueOutputFile) 
        {
            QueueOutput = new StreamWriter(QueueOutputFile);
            MySimulationJobs = new Generated_Jobs(inputFile);
            MyEvents = new Event_Priority_Queue(QueueOutput);
            MyEvents.AddEvent(new Discrete_Event(MySimulationJobs.NextJobStartTime, Discrete_Event.NEW_JOB, null));
            MyIOQueue = new IO_Queue();
            MyCPUs = new List<CPU>();
            CPURotation = 0;
            newThreadIDs = 0;
            PIDOutput = new StreamWriter(PIDOutputFile);
            CPUOutput = new StreamWriter(CPUOutputFile);
            
            for (int i = 0; i < Constants.NUM_CPU; i++)
            {
                MyCPUs.Add(new CPU(i.ToString(), CPUOutput));
            }
        }

        public void Run()
        {
            while (MyEvents.HasEvents())
            {
                List<Discrete_Event> currentEvents = MyEvents.GetNextEvents();
                int currentTime = currentEvents[0].TimeOf;
                Discrete_Event newEvent;
                for (int eventNum = 0; eventNum < currentEvents.Count; eventNum++)
                {
                    switch (currentEvents[eventNum].MyType)
                    {
                        case(Discrete_Event.NEW_JOB): // New job has been submitted
                            int currtime = MySimulationJobs.NextJobStartTime;
                            while (currtime == MySimulationJobs.NextJobStartTime)
                            {
                                Job newSubmittedJob = MySimulationJobs.GetNextJob();
                                for (int i = 0; newSubmittedJob.GetThread(i) != null; i++)
                                {
                                    newEvent = MyCPUs[CPURotation].AddThread(new PID(newThreadIDs++, newSubmittedJob.GetThread(i), PIDOutput, currtime, CPURotation), currentTime);
                                    if (newEvent != null)
                                    {
                                        newEvent.EventInformation.Add(CPURotation);
                                        MyEvents.AddEvent(newEvent);
                                    }
                                    CPURotation++;
                                    if (CPURotation == MyCPUs.Count) CPURotation = 0;
                                }
                            }
                            if (MySimulationJobs.NextJobStartTime != -1)
                            {
                                MyEvents.AddEvent(new Discrete_Event(MySimulationJobs.NextJobStartTime, Discrete_Event.NEW_JOB, null));
                            }
                            break;

                        case(Discrete_Event.CPU_OP_DONE): // Thread has finished CPU burst
                            int cpuNum = currentEvents[eventNum].EventInformation[currentEvents[eventNum].EventInformation.Count - 1];
                            currentEvents[eventNum].EventInformation.RemoveAt(currentEvents[eventNum].EventInformation.Count - 1);
                            newEvent = MyCPUs[cpuNum].EventReached(currentTime, currentEvents[eventNum]);
                            if (newEvent != null)
                            {
                                if (newEvent.MyType == Discrete_Event.MOVE_THREAD_TO_IO)
                                {
                                    Discrete_Event myIOevent = MyIOQueue.AddNewThread(currentTime, MyCPUs[cpuNum].MovePIDToIO());
                                    if (myIOevent != null)
                                    {
                                        MyEvents.AddEvent(myIOevent);
                                    }
                                    newEvent = new Discrete_Event(newEvent.TimeOf, newEvent.EventInformation[0], newEvent.ID);
                                }
                                newEvent.EventInformation.Add(cpuNum);
                                MyEvents.AddEvent(newEvent);
                            }
                            break;

                        case(Discrete_Event.IO_OP_DONE): // IO operation has finished
                            // TODO: Handle IO operation completion
                            newEvent = MyIOQueue.EventReached(currentTime, currentEvents[eventNum]);
                            if (newEvent.TimeOf != -1)
                            {
                                MyEvents.AddEvent(newEvent);
                            }

                            if (newEvent.ID != null)
                            {
                                // Move things back
                                PID transferPID = MyIOQueue.ReturnToCPU();
                                newEvent = MyCPUs[transferPID.CPUNum].AddThread(transferPID, currentTime);
                                if (newEvent != null && newEvent.TimeOf != -1)
                                {
                                    if (newEvent.EventInformation == null)
                                    {
                                        newEvent.EventInformation = new List<int>();
                                    }
                                    newEvent.EventInformation.Add(transferPID.CPUNum);
                                    MyEvents.AddEvent(newEvent);
                                }
                            }
                            break;

                            //TODO: Add any other cases
                    }
                }
            }

        }


    }
}
