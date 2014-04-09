using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FiniteStateMachine
{
    public class TileMap
    {
        private Texture2D terrainSet, locationSet, characterSet;
        private Texture2D overlay;

        private int tileSize;

        private static int mapCols, mapRows;

        private static List<List<Tile>> tiles;
        public static List<List<Tile>> Tiles
        {
            get { return tiles; }
        }
        public static int Width
        {
            get { return mapCols; }
        }
        public static int Height
        {
            get { return mapRows; }
        }

        private static Random rand = new Random();

        public TileMap(int mapcols = 25, int maprows = 19, int tileSize = 48)
        {
            mapCols = mapcols;
            mapRows = maprows;

            tiles = new List<List<Tile>>();
            for (int i = 0; i < mapRows; ++i)
                tiles.Add(new List<Tile>());

            this.tileSize = tileSize;

            GenerateRandomMap();
        }

        public void SetContent(Texture2D terrainset, Texture2D locationset, Texture2D characterset, Texture2D overlay)
        {
            this.terrainSet = terrainset;
            this.locationSet = locationset;
            this.characterSet = characterset;
            this.overlay = overlay;
        }

        public void Draw(SpriteBatch spriteBatch, int screenOffsetX, int screenOffsetY)
        {
            spriteBatch.Begin();

            // tiles
            for (int i = 0; i < mapRows; ++i)
            {
                int y = screenOffsetY + (i - (mapRows - 1) / 2) * tileSize - tileSize/2;
                for (int j = 0; j < mapCols; ++j)
                {
                    int x = screenOffsetX + (j - (mapCols - 1) / 2) * tileSize - tileSize / 2;

                    // terrain        
                    spriteBatch.Draw(terrainSet, new Rectangle(x, y, tileSize, tileSize), GetSourceRectangle(tiles[i][j].TileID, terrainSet.Height), Color.White);

                    // location
                    if (tiles[i][j].LocationID >= 0)
                    {
                        spriteBatch.Draw(locationSet, new Rectangle(x, y, tileSize, tileSize), GetSourceRectangle(tiles[i][j].LocationID, locationSet.Height), Color.White);
                    }

                    // overlay
                    spriteBatch.Draw(overlay, new Rectangle(x, y, tileSize, tileSize), tiles[i][j].TintColor * tiles[i][j].TintAlpha);
                }
            }

            // characters
            for (int i = 0; i < AgentManager.GetCount(); ++i)
            {
                Vector2 position = AgentManager.GetAgent(i).CurrentPosition;
                int x = screenOffsetX + ((int)position.X - (mapCols - 1) / 2) * tileSize - tileSize / 2;
                int y = screenOffsetY + ((int)position.Y - (mapRows - 1) / 2) * tileSize - tileSize / 2;

                spriteBatch.Draw(characterSet, new Rectangle(x, y, tileSize, tileSize), GetSourceRectangle(i, characterSet.Height), Color.White);
            }

            spriteBatch.End();
        }

        public void Update()
        {
            for (int i = 0; i < Height; ++i)
                for (int j = 0; j < Width; ++j)
                    Tiles[i][j].TintAlpha = 0.0f;
        }

        private void GenerateRandomMap()
        {
            // terrain
            for (int i = 0; i < mapRows; ++i)
            {
                for (int j = 0; j < mapCols; ++j)
                {
                    tiles[i].Add(new Tile(rand.Next(3), new Vector2(j, i)));
                }
            }

            // locations
            for (int i = 0; i < Enum.GetValues(typeof(Location)).Length; ++i)
            {
                while (true)
                {
                    int y = rand.Next(mapRows);
                    int x = rand.Next(mapCols);
                    if (tiles[y][x].LocationID < 0)
                    {
                        tiles[y][x].LocationID = i;
                        LocationProperties.LocationCoords[i] = new Vector2(x, y);

                        break;
                    }
                }
            }
        }

        private Rectangle GetSourceRectangle(int tileID, int size)
        {
            return new Rectangle(size * tileID, 0, size, size);
        }
    }
}
