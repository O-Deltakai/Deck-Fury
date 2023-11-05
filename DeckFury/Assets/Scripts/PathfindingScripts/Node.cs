using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Node : IComparable<Node>
{
    public int X { get; }
    public int Y { get; }
    public int F { get; set; }
    public int G { get; set; }
    public int H { get; set; }
    public Node Parent { get; set; }

    public Node(int x, int y, Node parent = null)
    {
        X = x;
        Y = y;
        F = 0;
        G = 0;
        H = 0;
        Parent = parent;
    }

    public int CompareTo(Node other)
    {
        return F.CompareTo(other.F);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Node))
        {
            return false;
        }

        Node other = obj as Node;
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (X * 397) ^ Y;
        }
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}