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

        private int mapCols, mapRows;

        private List<List<Tile>> tiles;

        private static Random rand = new Random();

        public TileMap(int mapCols = 19, int mapRows = 13, int tileSize = 48)
        {
            this.mapCols = mapCols;
            this.mapRows = mapRows;

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

        public void Draw(SpriteBatch spriteBatch, Vector2 screenOffset)
        {
            spriteBatch.Begin();

            // tiles
            for (int i = 0; i < mapRows; ++i)
            {
                int y = (int)screenOffset.Y + (i - (mapRows - 1) / 2) * tileSize - tileSize/2;
                for (int j = 0; j < mapCols; ++j)
                {
                    int x = (int)screenOffset.X + (j - (mapCols - 1) / 2) * tileSize - tileSize / 2;

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
                Vector2 pos = AgentManager.GetAgent(i).CurrentPosition;
                int x = (int)screenOffset.X + ((int)pos.X - (mapCols - 1) / 2) * tileSize - tileSize / 2;
                int y = (int)screenOffset.Y + ((int)pos.Y - (mapRows - 1) / 2) * tileSize - tileSize / 2;
                spriteBatch.Draw(characterSet, new Rectangle(x, y, tileSize, tileSize), GetSourceRectangle(i, characterSet.Height), Color.White);
            }

            spriteBatch.End();
        }

        private void GenerateRandomMap()
        {
            // tiles
            for (int i = 0; i < mapRows; ++i)
            {
                for (int j = 0; j < mapCols; ++j)
                {
                    //if (i == 0 || j == 0 || i == mapRows - 1 || j == mapCols - 1)
                    //    tiles[i].Add(new Tile(6));
                    //else
                    //{
                        tiles[i].Add(new Tile(rand.Next(3)));
                    //}
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
                        LocationPropertes.LocationCoords[i] = new Vector2(x, y);
                            //((x - (mapCols - 1) / 2) * tileSize - tileSize / 2, (y - (mapRows - 1) / 2) * tileSize - tileSize / 2);
                        
                        //tiles[y][x].TintAlpha = 0.5f;
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
