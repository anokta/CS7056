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
        public static float SENSE_RANGE = 2.0f;

        public static void UpdateSensors()
        {
            for (int i = 0; i < AgentManager.GetCount(); ++i)
            {
                Agent a1 = AgentManager.GetAgent(i);
                for (int j = 0; j < AgentManager.GetCount(); ++j)
                {
                    Agent a2 = AgentManager.GetAgent(j);

                    if (Vector2.Distance(a1.CurrentPosition, a2.CurrentPosition) < SENSE_RANGE)
                    {
                        // TODO : ASTAR STUFF
                        Sense sense = new Sense(a2.Id, a1.Id, SenseType.Sight);
                        a1.HandleSenseEvent(sense);
                    }
                }
            }
        }
    }
}
