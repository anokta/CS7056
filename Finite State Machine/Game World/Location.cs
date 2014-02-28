using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FiniteStateMachine
{
    // Here is the list of locations that agents can visit
    public enum Location
    {
        shack = 0,
        goldMine = 1,
        bank = 2,
        saloon = 3,
        outlawCamp = 4, 
        sheriffsOffice = 5, 
        undertakers = 6,
        cemetery = 7 
    }

    public class LocationProperties
    {
        public static Vector2[] LocationCoords = new Vector2[Enum.GetValues(typeof(Location)).Length];

        public static String ToString(Location location)
        {
            switch (location)
            {
                case Location.shack:
                    return "Bob's Shack";
                case Location.goldMine:
                    return "Gold Mine";
                case Location.bank:
                    return "Bank";
                case Location.saloon:
                    return "Saloon";
                case Location.outlawCamp:
                    return "Outlaw Camp";
                case Location.sheriffsOffice:
                    return "Sheriff's Office";
                case Location.undertakers:
                    return "Undertakers";
                case Location.cemetery:
                    return "Cemetery";
                default:
                    return "Unknown Location";
            }
        }
    }
}
