using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;

namespace FiniteStateMachine
{
    public class Tile
    {
        private static float[] TILE_COSTS = { 1.0f, 1.5f, 2.5f };
 
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

        private Vector2 tilePosition;
        public Vector2 Position
        {
            get { return tilePosition; }
            set { tilePosition = value; }
        }

        private float tileCost;
        public float TileCost
        {
            get { return tileCost; }
            set { tileCost = value; }
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


        public Tile(int tileID, Vector2 position)
        {
            this.tileID = tileID;

            tilePosition = position;
            tileCost = TILE_COSTS[tileID];
            
            tintColor = Color.White;
            tintAlpha = 0.0f;
            locationID = -1;
        }
    }
}
