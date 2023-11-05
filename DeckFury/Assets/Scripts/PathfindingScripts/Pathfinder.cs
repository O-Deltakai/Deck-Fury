using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Tilemaps;
using UnityEngine;

public class Pathfinder
{
    public static List<Vector3Int> FindPath(Tilemap tilemap, StageManager stageManager, Vector3Int start, Vector3Int end, bool printGraph = false)
    {
        int[,] graph = GenerateGraph(tilemap, stageManager);

        Node startNode = ConvertPositionToNode(start);
        Node endNode = ConvertPositionToNode(end);

        graph[startNode.X, startNode.Y] = 1;
        graph[endNode.X, endNode.Y] = 1;

        if (printGraph)
        {
            Debug.Log(PrintGraph(graph));
        }

        AStarPathfinder aStarPathfinder = new AStarPathfinder(graph);

        List<Node> path = aStarPathfinder.FindShortestPath(startNode, endNode);

        if (printGraph)
        {
            if (path != null)
            {
                Debug.Log(PrintGraphWithPath(graph, path));
            }
            else
            {
                Debug.Log($"There is no path from {startNode} to {endNode}");
            }
        }

        return path?.Select(ConvertNodeToPosition).ToList();
    }

    private static int[,] GenerateGraph(Tilemap tilemap, StageManager stageManager)
    {
        int[,] graph = new int[13, 11];

        List<Vector3Int> positions = MapHelperFunctions.GetValidTilePositions(tilemap, stageManager);

        foreach (var position in positions)
        {
            Node node = ConvertPositionToNode(position);
            graph[node.X, node.Y] = 1;
        }

        return graph;
    }

    private static Node ConvertPositionToNode(Vector3Int position)
    {
        return new Node(position.x + 6, position.y + 5);
    }

    private static Vector3Int ConvertNodeToPosition(Node node)
    {
        return new Vector3Int(node.X - 6, node.Y - 5, 0);
    }

    private static string PrintGraph(int[,] graph)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < graph.GetLength(0); i++)
        {
            for (int j = 0; j < graph.GetLength(1); j++)
            {
                sb.Append($"{graph[i, j]} ");
            }
            sb.Append("\n");
        }
        return sb.ToString();
    }

    private static string PrintGraphWithPath(int[,] graph, List<Node> path)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < graph.GetLength(0); i++)
        {
            for (int j = 0; j < graph.GetLength(1); j++)
            {
                sb.Append(path.Contains(new Node(i, j)) ? $"* " : $"{graph[i, j]} ");
            }
            sb.Append("\n");
        }
        return sb.ToString();
    }

}