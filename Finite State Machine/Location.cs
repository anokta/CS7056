using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiniteStateMachine
{
    // Here is the list of locations that agents can visit
    public enum Location
    {
        shack,
        goldMine,
        bank,
        saloon,
        outlawCamp, 
        sheriffsOffice, 
        undertakers,
        cemetery 
    }

    public class LocationName
    {
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
