using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KDTree2D
{
    private class KDNode
    {
        public Vector3 point;
        public KDNode left;
        public KDNode right;

        public KDNode(Vector3 point)
        {
            this.point = point;
            left = null;
            right = null;
        }
    }

    private KDNode root;
    private int k = 2;

    public KDTree2D(List<Vector3> points)
    {
        root = BuildTree(points, 0);
    }

    private KDNode BuildTree(List<Vector3> points, int depth)
    {
        if (points.Count == 0)
            return null;

        int axis = depth % k;
        points.Sort((a, b) => a[axis].CompareTo(b[axis]));

        int medianIndex = points.Count / 2;
        KDNode node = new KDNode(points[medianIndex])
        {
            left = BuildTree(points.GetRange(0, medianIndex), depth + 1),
            right = BuildTree(points.GetRange(medianIndex + 1, points.Count - (medianIndex + 1)), depth + 1)
        };

        return node;
    }

    public Vector3 FindNearest(Vector3 target)
    {
        return FindNearest(root, target, 0).point;
    }

    private KDNode FindNearest(KDNode node, Vector3 target, int depth)
    {
        if (node == null)
            return null;

        KDNode nextBranch = null;
        KDNode oppositeBranch = null;
        int axis = depth % k;

        if (target[axis] < node.point[axis])
        {
            nextBranch = node.left;
            oppositeBranch = node.right;
        }
        else
        {
            nextBranch = node.right;
            oppositeBranch = node.left;
        }

        KDNode best = CloserDistance(target, FindNearest(nextBranch, target, depth + 1), node);

        if (Vector3.Distance(new Vector3(target.x, target.y, 0), new Vector3(best.point.x, best.point.y, 0)) > Mathf.Abs(target[axis] - node.point[axis]))
        {
            best = CloserDistance(target, FindNearest(oppositeBranch, target, depth + 1), best);
        }

        return best;
    }

    private KDNode CloserDistance(Vector3 target, KDNode a, KDNode b)
    {
        if (a == null) return b;
        if (b == null) return a;

        float distanceA = Vector3.Distance(new Vector3(target.x, target.y, 0), new Vector3(a.point.x, a.point.y, 0));
        float distanceB = Vector3.Distance(new Vector3(target.x, target.y, 0), new Vector3(b.point.x, b.point.y, 0));

        return distanceA < distanceB ? a : b;
    }
}
