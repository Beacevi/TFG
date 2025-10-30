using UnityEngine;

public class Node
{
    public Vector2Int position;
    public bool walkable;

    public Node parent;
    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;

    public Node(Vector2Int pos, bool walkable)
    {
        this.position = pos;
        this.walkable = walkable;
    }
}