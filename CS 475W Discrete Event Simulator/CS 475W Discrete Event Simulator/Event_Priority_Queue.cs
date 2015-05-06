using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CS_475W_Discrete_Event_Simulator
{
    class Event_Priority_Queue
    {
        private StreamWriter myOutput;
        private Dictionary<int, List<Discrete_Event>> TimeOfToEventDictionary = new Dictionary<int, List<Discrete_Event>>();
        private int timeCount = 0;

        public Event_Priority_Queue(StreamWriter output)
        {
            myOutput = output;
        }

        public List<Discrete_Event> GetNextEvents()
        {
            while (TimeOfToEventDictionary.ContainsKey(timeCount) == false) 
            {
                timeCount++;
                //if (timeCount > 100000) throw new Exception("boop");
            }
            List<Discrete_Event> currentEvents = TimeOfToEventDictionary[timeCount];
            TimeOfToEventDictionary.Remove(timeCount);
            myOutput.WriteLine("Events: " + timeCount);
            myOutput.Flush();
            timeCount++;
            return currentEvents;
        }

        public bool HasEvents()
        {
            return TimeOfToEventDictionary.Count > 0 ? true : false;
        }

        public void AddEvent(Discrete_Event newEvent)
        {
            if (newEvent.TimeOf == -1) return;
            if (newEvent.TimeOf < timeCount)
            {
                double boop = 1;
            }
            myOutput.WriteLine("New Event: " + newEvent.TimeOf + " Type: " + newEvent.MyType);
            myOutput.Flush();
            if (!TimeOfToEventDictionary.ContainsKey(newEvent.TimeOf))
            {
                TimeOfToEventDictionary.Add(newEvent.TimeOf, new List<Discrete_Event>());
            }
            TimeOfToEventDictionary[newEvent.TimeOf].Add(newEvent);
        }

    }
}
