using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Pathfinding
{
    public class Pathfinder : MonoBehaviour
    {
        public Tilemap tilemap;
        private TilemapGrid grid;

        private void Awake()
        {
            grid = new TilemapGrid(tilemap, tilemap.cellSize.x, tilemap.origin);
        }

        public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 targetWorldPosition)
        {
            startWorldPosition.y = 0;
            targetWorldPosition.y = 0;
            
            print("FindPath: " + startWorldPosition + " " + targetWorldPosition);
            
            // Check if the target is within 0.4 units from the start position
            if (Vector3.Distance(startWorldPosition, targetWorldPosition) < 0.4f)
            {
                return new List<Vector3> { targetWorldPosition };
            }

            Vector3Int start = grid.WorldToCell(startWorldPosition);
            Vector3Int target = grid.WorldToCell(targetWorldPosition);

            Node startNode = new Node(start);
            Node targetNode = new Node(target);

            List<Node> openList = new List<Node> { startNode };
            HashSet<Node> closedList = new HashSet<Node>();

            while (openList.Count > 0)
            {
                Node currentNode = openList[0];

                for (int i = 1; i < openList.Count; i++)
                {
                    if (openList[i].FCost < currentNode.FCost || (openList[i].FCost == currentNode.FCost && openList[i].HCost < currentNode.HCost))
                    {
                        currentNode = openList[i];
                    }
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                if (currentNode.Position == targetNode.Position)
                {
                    return RetracePath(startNode, currentNode);
                }

                foreach (Vector3Int neighborPosition in GetNeighbors(currentNode.Position))
                {
                    if (!grid.HasTile(neighborPosition) || closedList.Contains(new Node(neighborPosition)))
                    {
                        continue;
                    }

                    float newGCost = currentNode.GCost + GetDistance(currentNode.Position, neighborPosition);
                    Node neighborNode = new Node(neighborPosition)
                    {
                        GCost = newGCost,
                        HCost = GetHeuristic(neighborPosition, targetNode.Position),
                        Parent = currentNode
                    };

                    if (openList.Exists(n => n.Position == neighborNode.Position && n.GCost <= newGCost))
                    {
                        continue;
                    }

                    openList.Add(neighborNode);
                }
            }

            return new List<Vector3>();  // No path found
        }

        private List<Vector3> RetracePath(Node startNode, Node endNode)
        {
            List<Vector3> path = new List<Vector3>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(tilemap.GetCellCenterWorld(currentNode.Position));
                currentNode = currentNode.Parent;
            }

            path.Reverse();
            return path;
        }

        private float GetHeuristic(Vector3Int from, Vector3Int to)
        {
            return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
        }

        private float GetDistance(Vector3Int from, Vector3Int to)
        {
            return Vector3Int.Distance(from, to);
        }

        private IEnumerable<Vector3Int> GetNeighbors(Vector3Int position)
        {
            List<Vector3Int> neighbors = new List<Vector3Int>
            {
                new Vector3Int(position.x + 1, position.y, position.z),
                new Vector3Int(position.x - 1, position.y, position.z),
                new Vector3Int(position.x, position.y + 1, position.z),
                new Vector3Int(position.x, position.y - 1, position.z)
            };

            return neighbors;
        }

        public Vector3 GetRandomTarget(Vector3 position, int minDistance, int maxDistance)
        {
            int maxAttempts = 100;
            int attempts = 0;
            
            Vector3 lastPosition = position;
            while (attempts < maxAttempts)
            {
                attempts++;
                
                float randomX = Random.Range(-maxDistance, maxDistance);
                float randomZ = Random.Range(-maxDistance, maxDistance);
                
                Vector3 randomOffset = new Vector3(randomX, position.y, randomZ);
                
                Vector3 potentialTarget = position + randomOffset;
                Vector3Int targetCell = grid.WorldToCell(potentialTarget);
                
                if (!tilemap.cellBounds.Contains(targetCell) || !tilemap.HasTile(targetCell)) continue;
                
                float distance = Vector3.Distance(position, tilemap.GetCellCenterWorld(targetCell));
                
                if (distance < minDistance || distance > maxDistance) continue;
                
                List<Vector3> path = FindPath(position, tilemap.GetCellCenterWorld(targetCell));

                if (path.Count > 0)
                {
                    if (lastPosition != potentialTarget)
                    {
                        return potentialTarget;
                    }
                }
            }
            
            return position;
        }
    }
}
