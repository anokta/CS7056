using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;

namespace FiniteStateMachine
{
    public class Tile
    {

        // tile id for the image offset
        private int tileID;
        public int TileID
        {
            get { return tileID; }
            set { tileID = value; }
        }

        private int locationID;
        public int LocationID
        {
            get { return locationID; }
            set { locationID = value; }
        }

        // overlay
        private Color tintColor;
        public Color TintColor
        {
            get { return tintColor; }
            set { tintColor = value; }
        }

        private float tintAlpha;
        public float TintAlpha
        {
            get { return tintAlpha; }
            set { tintAlpha = value; }
        }


        public Tile(int tileID)
        {
            this.tileID = tileID;
            tintColor = Color.White;
            tintAlpha = 0.0f;
            locationID = -1;
        }
    }
}
