using System;
using System.Collections.Generic;
using System.Text;

namespace FiniteStateMachine
{
    public static class AgentManager
    {
        static List<Agent> listOfAgents = new List<Agent>();
        public static int AddAgent(Agent agent)
        {
            listOfAgents.Add(agent);
            return listOfAgents.IndexOf(agent);
        }

        public static Agent GetAgent(int id)
        {
            return listOfAgents[id];
        }

        public static void RemoveAgent(Agent agent)
        {
            listOfAgents.Remove(agent);
        }

        public static int GetCount()
        {
            return listOfAgents.Count;
        }
    }
}
