using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FiniteStateMachine
{    
    public enum SenseType
    {
        Sight,
        Hearing,
        Smell
    };

    public struct Sense
    {
        public int Sender;
        public int Receiver;
        public SenseType senseType;

        public Sense(int s, int r, SenseType st)
        {
            Sender = s;
            Receiver = r;
            senseType = st;
        }
    }

    public static class SenseEvent
    {
        static AStar propogator = new AStar(AStar.SearchType.SensePropogation);

        static float SENSE_RANGE = 4.0f;

        public static void UpdateSensors()
        {
            for (int i = 0; i < AgentManager.GetCount(); ++i)
            {
                Agent a1 = AgentManager.GetAgent(i);
                for (int j = 0; j < AgentManager.GetCount(); ++j)
                {
                    if (i != j)
                    {
                        Agent a2 = AgentManager.GetAgent(j);

                        // If close enough
                        if (Vector2.Distance(a1.CurrentPosition, a2.CurrentPosition) < SENSE_RANGE)
                        {
                            // Propogate the sense
                            if (propogator.PropogateSense(a1.CurrentPosition, a2.CurrentPosition))
                            {
                                // Sense the agent
                                Sense sense = new Sense(a2.Id, a1.Id, SenseType.Sight);
                                a1.HandleSenseEvent(sense);
                            }
                        }
                    }
                }
            }
        }
    }
}
