using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_475W_Discrete_Event_Simulator
{
    class Event_Priority_Queue
    {
        private Dictionary<int, List<Discrete_Event>> TimeOfToEventDictionary = new Dictionary<int, List<Discrete_Event>>();
        private int timeCount = 0;

        public Event_Priority_Queue()
        {

        }

        public List<Discrete_Event> GetNextEvents()
        {
            while (TimeOfToEventDictionary.ContainsKey(timeCount) == false) 
            {
                timeCount++;
            }
            List<Discrete_Event> currentEvents = TimeOfToEventDictionary[timeCount];
            TimeOfToEventDictionary.Remove(timeCount);
            timeCount++;
            return currentEvents;
        }

        public bool HasEvents()
        {
            return TimeOfToEventDictionary.Count > 0 ? true : false;
        }

        public void AddEvent(Discrete_Event newEvent)
        {
            if (!TimeOfToEventDictionary.ContainsKey(newEvent.TimeOf))
            {
                TimeOfToEventDictionary.Add(newEvent.TimeOf, new List<Discrete_Event>());
            }
            TimeOfToEventDictionary[newEvent.TimeOf].Add(newEvent);
        }

    }
}
