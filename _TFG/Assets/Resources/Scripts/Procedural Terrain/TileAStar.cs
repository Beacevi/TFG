using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileAStar : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] Transform player;
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] PCGtiles_IsometricPerlin mapGenerator;

    private List<Vector3> path = new List<Vector3>();
    private int currentIndex = 0;
    public bool moving = false;

    private const int INF = int.MaxValue / 4;

    private Node lastPathNode = null;

    public int stepsAvailable = 30;

    private void Start()
    {
        if(!tilemap)
            tilemap = GameObject.FindGameObjectWithTag("MainTileMap").GetComponent<Tilemap>();
        if (!mapGenerator)
            mapGenerator = GameObject.FindGameObjectWithTag("MapGenerator").GetComponent<PCGtiles_IsometricPerlin>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 w = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            w.z = 0;
            Vector3Int clicked = tilemap.WorldToCell(w);
            Vector3Int start = tilemap.WorldToCell(player.position);

            path = FindPath(start, clicked);

            if (path.Count > stepsAvailable)
            {
                path = path.GetRange(0, stepsAvailable);
            }


            if (path.Count > 0)
            {
                moving = true;
                currentIndex = 0;
            }
        }

        if (moving && path.Count > 0)
        {
            var target = path[currentIndex];
            player.position = Vector3.MoveTowards(player.position, target, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(player.position, target) < 0.05f)
            {
                currentIndex++;
                if (currentIndex >= path.Count)
                {
                    if (lastPathNode != null)
                    {
                        if (lastPathNode.hasObject)
                        {
                            var interactable = lastPathNode.Interactable?.GetComponent<InteractableGameObject>();
                            if (interactable != null)
                            {
                                interactable.Interact(this);
                            }
                            else
                            {
                                Debug.Log("el nodo tiene objeto pero InteractableGameObject no existe en el prefab.");
                                moving = false;
                            }
                        }
                        else
                        {
                            Debug.Log("lastPathNode no tiene objeto.");
                            moving = false;
                        }
                    }
                    else
                    {
                        Debug.Log("lastPathNode es NULL.");
                        moving = false;
                    }

                    stepsAvailable -= path.Count;
                    stepsAvailable = Mathf.Max(0, stepsAvailable);

                    moving = false;
                    path.Clear();
                }
            }
        }
    }

    List<Vector3> FindPath(Vector3Int startCell, Vector3Int targetCell)
    {
        Node[,] nodes = mapGenerator.nodes;
        if (nodes == null) return new List<Vector3>();

        int width = mapGenerator.width;
        int height = mapGenerator.height;

        if (!InBounds(startCell, width, height) || !InBounds(targetCell, width, height))
            return new List<Vector3>();

        Node start = nodes[startCell.x, startCell.y];
        Node target = nodes[targetCell.x, targetCell.y];
        lastPathNode = target;

        if (!start.walkable || !target.walkable)
            return new List<Vector3>();

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                nodes[x, y].gCost = INF;
                nodes[x, y].hCost = 0;
                nodes[x, y].parent = null;
            }

        start.gCost = 0;
        start.hCost = Heuristic(start, target);

        List<Node> open = new List<Node>() { start };
        HashSet<Node> closed = new HashSet<Node>();

        while (open.Count > 0)
        {
            Node cur = open[0];
            for (int i = 1; i < open.Count; i++)
                if (open[i].fCost < cur.fCost ||
                    (open[i].fCost == cur.fCost && open[i].hCost < cur.hCost))
                    cur = open[i];

            open.Remove(cur);
            closed.Add(cur);

            if (cur == target)
                return Retrace(start, target);

            foreach (var neigh in GetNeighbors(cur, nodes))
            {
                if (!neigh.walkable || closed.Contains(neigh))
                    continue;

                if (IsDiagonal(cur, neigh))
                {
                    Vector2Int p = cur.position;
                    Vector2Int n = neigh.position;

                    if (!nodes[p.x, n.y].walkable || !nodes[n.x, p.y].walkable)
                        continue;
                }

                int cost = cur.gCost + (IsDiagonal(cur, neigh) ? 14 : 10);
                if (cost < neigh.gCost)
                {
                    neigh.gCost = cost;
                    neigh.hCost = Heuristic(neigh, target);
                    neigh.parent = cur;

                    if (!open.Contains(neigh))
                        open.Add(neigh);
                }
            }
        }

        return new List<Vector3>();
    }

    bool InBounds(Vector3Int cell, int w, int h)
        => cell.x >= 0 && cell.y >= 0 && cell.x < w && cell.y < h;

    bool IsDiagonal(Node a, Node b)
        => a.position.x != b.position.x && a.position.y != b.position.y;

    List<Node> GetNeighbors(Node node, Node[,] nodes)
    {
        List<Node> r = new();
        int x = node.position.x;
        int y = node.position.y;

        int[,] dirs = {
            {1,0},{-1,0},{0,1},{0,-1},
            {1,1},{1,-1},{-1,1},{-1,-1}
        };

        int W = nodes.GetLength(0);
        int H = nodes.GetLength(1);

        for (int i = 0; i < dirs.GetLength(0); i++)
        {
            int nx = x + dirs[i, 0];
            int ny = y + dirs[i, 1];
            if (nx >= 0 && ny >= 0 && nx < W && ny < H)
                r.Add(nodes[nx, ny]);
        }

        return r;
    }

    int Heuristic(Node a, Node b)
    {
        int dx = Mathf.Abs(a.position.x - b.position.x);
        int dy = Mathf.Abs(a.position.y - b.position.y);
        return 14 * Mathf.Min(dx, dy) + 10 * Mathf.Abs(dx - dy);
    }

    List<Vector3> Retrace(Node start, Node end)
    {
        List<Vector3> p = new();
        Node c = end;

        while (c != start)
        {
            p.Add(tilemap.GetCellCenterWorld(new Vector3Int(c.position.x, c.position.y, 0)));
            c = c.parent;
        }

        p.Reverse();
        return p;
    }


    void OnDrawGizmos()
    {
        if (path == null || path.Count == 0) return;

        Gizmos.color = Color.red;
        foreach (var pos in path)
            Gizmos.DrawSphere(pos, 0.1f);

        Gizmos.color = Color.yellow;
        for (int i = 0; i < path.Count - 1; i++)
            Gizmos.DrawLine(path[i], path[i + 1]);
    }
}
