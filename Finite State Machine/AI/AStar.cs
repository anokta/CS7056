using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace FiniteStateMachine
{
    public class AStar
    {
        public enum SearchType { ShortestPath, SensePropogation };
        SearchType type;

        private float PERCEPTION_THRESHOLD = 6.0f;

        // Generic Node class to be used in AStar
        private class Node
        {
            public Tile tile;
            public Node parent;
            public float G;
            public float H;

            public Node(Tile tile, Vector2 targetPosition, Node parent, SearchType type)
            {
                this.tile = tile;
                this.parent = parent;

                if (this.parent == null)
                    this.G = 0;
                else if (type == SearchType.ShortestPath)
                    this.G = this.parent.G + tile.TileCost * (Math.Abs(this.parent.tile.Position.X - tile.Position.X) + Math.Abs(this.parent.tile.Position.Y - tile.Position.Y));
                else if (type == SearchType.SensePropogation)
                    this.G = this.parent.G + tile.TileAttenuation;

                H = Math.Abs(targetPosition.X - tile.Position.X) + Math.Abs(targetPosition.Y - tile.Position.Y);
            }

            public float GetF()
            {
                return G + H;
            }
        };

        private List<Node> openList;
        private List<Node> closedList;

        public AStar(SearchType searchType = SearchType.ShortestPath)
        {
            openList = new List<Node>();
            closedList = new List<Node>();

            type = searchType;
        }

        // Sense propogation using attenuation properties of the tiles
        public bool PropogateSense(Vector2 startPosition, Vector2 targetPosition)
        {
            Tile startTile = TileMap.Tiles[(int)startPosition.Y][(int)startPosition.X];

            Node startNode = new Node(startTile, targetPosition, null, type);

            openList.Clear();
            closedList.Clear();

            openList.Add(startNode);
            while (openList.Count > 0)
            {
                Node currentNode = null;
                float minF = float.MaxValue;
                for (int i = 0; i < openList.Count; ++i)
                {
                    float F = ((Node)openList[i]).GetF();
                    if (F < minF)
                    {
                        currentNode = (Node)openList[i];
                        minF = F;
                    }
                }

                closedList.Add(currentNode);
                openList.Remove(currentNode);

                // Target reached?
                if (currentNode.tile.Position == targetPosition)
                    return true;
                else if (currentNode.G > PERCEPTION_THRESHOLD) // Target out of perception?
                    return false;

                // Check adjacent nodes
                Tile adjTile;
                Vector2 adjPosition;

                adjPosition = new Vector2(currentNode.tile.Position.X - 1, currentNode.tile.Position.Y);
                if (IsGridReachable(adjPosition))
                {
                    adjTile = TileMap.Tiles[(int)adjPosition.Y][(int)adjPosition.X];
                    CheckAdjacentNode(new Node(adjTile, targetPosition, currentNode, type));
                }

                adjPosition = new Vector2(currentNode.tile.Position.X + 1, currentNode.tile.Position.Y);
                if (IsGridReachable(adjPosition))
                {
                    adjTile = TileMap.Tiles[(int)adjPosition.Y][(int)adjPosition.X];
                    CheckAdjacentNode(new Node(adjTile, targetPosition, currentNode, type));
                }

                adjPosition = new Vector2(currentNode.tile.Position.X, currentNode.tile.Position.Y - 1);
                if (IsGridReachable(adjPosition))
                {
                    adjTile = TileMap.Tiles[(int)adjPosition.Y][(int)adjPosition.X];
                    CheckAdjacentNode(new Node(adjTile, targetPosition, currentNode, type));
                }

                adjPosition = new Vector2(currentNode.tile.Position.X, currentNode.tile.Position.Y + 1);
                if (IsGridReachable(adjPosition))
                {
                    adjTile = TileMap.Tiles[(int)adjPosition.Y][(int)adjPosition.X];
                    CheckAdjacentNode(new Node(adjTile, targetPosition, currentNode, type));
                }
            }

            return false;
        }

        // Finds the shortest path with respect to the given tile costs
        public List<Tile> FindPath(Vector2 startPosition, Vector2 targetPosition)
        {
            Tile startTile = TileMap.Tiles[(int)startPosition.Y][(int)startPosition.X];

            Node startNode = new Node(startTile, targetPosition, null, type);

            openList.Clear();
            closedList.Clear();

            openList.Add(startNode);
            while (openList.Count > 0)
            {
                Node currentNode = null;
                float minF = float.MaxValue;
                for (int i = 0; i < openList.Count; ++i)
                {
                    float F = ((Node)openList[i]).GetF();
                    if (F < minF)
                    {
                        currentNode = (Node)openList[i];
                        minF = F;
                    }
                }

                closedList.Add(currentNode);
                openList.Remove(currentNode);

                // Target reached?
                if (currentNode.tile.Position == targetPosition)
                    return GetPathPositions(currentNode);
                else if (closedList.Count > 200)
                    break;

                // Check adjacent nodes
                Tile adjTile;
                Vector2 adjPosition;

                adjPosition = new Vector2(currentNode.tile.Position.X - 1, currentNode.tile.Position.Y);
                if (IsGridReachable(adjPosition))
                {
                    adjTile = TileMap.Tiles[(int)adjPosition.Y][(int)adjPosition.X];
                    CheckAdjacentNode(new Node(adjTile, targetPosition, currentNode, type));
                }

                adjPosition = new Vector2(currentNode.tile.Position.X + 1, currentNode.tile.Position.Y);
                if (IsGridReachable(adjPosition))
                {
                    adjTile = TileMap.Tiles[(int)adjPosition.Y][(int)adjPosition.X];
                    CheckAdjacentNode(new Node(adjTile, targetPosition, currentNode, type));
                }

                adjPosition = new Vector2(currentNode.tile.Position.X, currentNode.tile.Position.Y - 1);
                if (IsGridReachable(adjPosition))
                {
                    adjTile = TileMap.Tiles[(int)adjPosition.Y][(int)adjPosition.X];
                    CheckAdjacentNode(new Node(adjTile, targetPosition, currentNode, type));
                }

                adjPosition = new Vector2(currentNode.tile.Position.X, currentNode.tile.Position.Y + 1);
                if (IsGridReachable(adjPosition))
                {
                    adjTile = TileMap.Tiles[(int)adjPosition.Y][(int)adjPosition.X];
                    CheckAdjacentNode(new Node(adjTile, targetPosition, currentNode, type));
                }
            }

            return GetPathPositions(closedList[closedList.Count - 1]);
        }

        // Checks if the adjacent node is proper
        void CheckAdjacentNode(Node adjNode)
        {
            bool flag = true;

            for (int i = 0; i < openList.Count; ++i)
            {
                if (openList[i].tile == adjNode.tile)
                {
                    if (adjNode.G < openList[i].G)
                    {
                        openList[i] = adjNode;
                    }

                    flag = false;
                    break;
                }
            }
            foreach (Node node in closedList)
            {
                if (adjNode.tile == node.tile)
                {
                    flag = false;
                    break;
                }
            }

            if (flag)
            {
                openList.Add(adjNode);
            }
        }

        // Checks map boundaries
        bool IsGridReachable(Vector2 p)
        {
            int x = (int)p.X;
            int y = (int)p.Y;
            return (x >= 0 && x < TileMap.Width && y >= 0 && y < TileMap.Height);
        }

        // Returns the shortest path as a list
        List<Tile> GetPathPositions(Node node)
        {
            List<Tile> positions = new List<Tile>();

            while (node.parent != null)
            {
                positions.Insert(0, node.tile);
                node = node.parent;
            }

            return positions;
        }
    }
}
