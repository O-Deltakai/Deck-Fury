using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AStarPathfinder
{
    private readonly int[,] grid;
    private readonly OptimizedPriorityQueue<Node> openSet;
    private readonly HashSet<Node> closedSet;

    public AStarPathfinder(int[,] grid)
    {
        this.grid = grid;
        openSet = new OptimizedPriorityQueue<Node>();
        closedSet = new HashSet<Node>();
    }

    public List<Node> FindShortestPath(Node startNode, Node endNode)
    {
        if (!CheckValidity(startNode, nameof(startNode)) || !CheckValidity(endNode, nameof(endNode)))
        {
            return null;
        }

        openSet.Enqueue(startNode, 0);

        while (openSet.Any())
        {
            Node currentNode = openSet.Dequeue();

            if (currentNode.Equals(endNode))
            {
                return ReconstructPath(currentNode);
            }

            closedSet.Add(currentNode);

            foreach (Node neighborNode in GetNeighbors(currentNode))
            {
                if (grid[neighborNode.X, neighborNode.Y] == 0) continue;

                if (closedSet.Contains(neighborNode))
                {
                    continue;
                }

                int newCost = currentNode.F + grid[neighborNode.X, neighborNode.Y];

                if (!openSet.Contains(neighborNode) || newCost < neighborNode.F)
                {
                    neighborNode.F = newCost;
                    neighborNode.G = currentNode.G + 1;
                    neighborNode.H = Heuristic(neighborNode, endNode);

                    if (!openSet.Contains(neighborNode))
                    {
                        openSet.Enqueue(neighborNode, neighborNode.F);
                    }
                }
            }
        }

        return null;
    }

    private bool CheckValidity(Node node, String name)
    {
        if (!IsWithinGrid(node))
        {
            throw new ArgumentException($"{name} {node} is not on the grid!");
        }

        if (grid[node.X, node.Y] == 0)
        {
            throw new ArgumentException($"{name} {node} must be reachable; having value of 1, rather than 0!");
        }

        return true;
    }

    private bool IsWithinGrid(Node node)
    {
        return node.X >= 0 && node.X < grid.GetLength(0) && node.Y >= 0 && node.Y < grid.GetLength(1);
    }

    private List<Node> ReconstructPath(Node currentNode)
    {
        List<Node> path = new List<Node>();

        while (currentNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }

    private IEnumerable<Node> GetNeighbors(Node currentNode)
    {
        int x = currentNode.X;
        int y = currentNode.Y;

        if (x > 0)
        {
            yield return new Node(x - 1, y, currentNode);
        }

        if (x < grid.GetLength(0) - 1)
        {
            yield return new Node(x + 1, y, currentNode);
        }

        if (y > 0)
        {
            yield return new Node(x, y - 1, currentNode);
        }

        if (y < grid.GetLength(1) - 1)
        {
            yield return new Node(x, y + 1, currentNode);
        }
    }

    private int Heuristic(Node currentNode, Node endNode)
    {
        // Manhattan distance
        return Math.Abs(currentNode.X - endNode.X) + Math.Abs(currentNode.Y - endNode.Y);
    }
}
