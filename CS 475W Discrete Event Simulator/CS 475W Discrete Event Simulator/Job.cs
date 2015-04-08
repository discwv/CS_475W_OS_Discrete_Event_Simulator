using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_475W_Discrete_Event_Simulator
{
    class Job
    {
        private int myStartTime;
        public int startTime
        {
            get { return myStartTime; }
            set { myStartTime = value; }
        }
        private List<List<int>> myThreads;

        public Job(string[] inputString)
        {
            int[] firstLine = inputString[0].Split(',').Select(int.Parse).ToArray();
            myStartTime = firstLine[0];
            for (int i = 0; i < firstLine[1]; i++)
            {
                List<int> line = inputString[i+1].Split(',').Select(int.Parse).ToList();
                myThreads.Add(line);
            }
        }
        public List<int> GetThread(int threadNum)
        {
            if (threadNum < 0 || threadNum >= myThreads.Count)
            {
                return null;
            }
            else
            {
                return myThreads[threadNum];
            }
        }
    }
}
