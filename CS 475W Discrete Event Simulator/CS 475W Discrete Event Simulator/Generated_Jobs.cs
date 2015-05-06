using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CS_475W_Discrete_Event_Simulator
{
    class Generated_Jobs
    {
        private List<Job> AllJobs;

        public int NextJobStartTime
        {
            get { return AllJobs.Count > 0 ? AllJobs[0].startTime : -1; }
            set { }
        }
        public Generated_Jobs(string filename)
        {
            StreamReader inputFile = new StreamReader(filename);
            string lineNumJobs = inputFile.ReadLine();
            int[] firstLine = lineNumJobs.Split(',').Select(int.Parse).ToArray();
            int numJobs = firstLine[0];
            AllJobs = new List<Job>();
            while(numJobs-- > 0)
            {
                List<string> jobStrings = new List<string>();
                jobStrings.Add(inputFile.ReadLine());
                int numThreads = jobStrings[0].Split(',').Select(int.Parse).ToArray()[1];
                while (numThreads-- > 0)
                {
                    jobStrings.Add(inputFile.ReadLine());
                }
                AllJobs.Add(new Job(jobStrings.ToArray()));
            }
        }

        public Job GetNextJob()
        {
            Job nextJob = AllJobs[0];
            AllJobs.RemoveAt(0);
            return nextJob;
        }
    }
}
