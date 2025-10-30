using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PCGtiles_IsometricPerlin : MonoBehaviour
{
    //STP 1: SET TILES 
    //STP 2: PERLIN NOISE
    //STP 3: ISLAND (2nd Perlin noise with offset)
    //STP 4: CELULLAR AUTOMATA
    //STP 5: RULE TILES

    [Header("Player")]
    [SerializeField] GameObject player; 

    [Header("Tilemap")]
    public Tilemap tilemap;

    [Header("Tiles (RuleTile or TileBase)")]
    public TileBase ruleGrass;
    public TileBase ruleSand;
    public TileBase ruleWater;

    [Header("Map Settings")]
    public int width = 50;
    public int height = 50;

    [Header("Perlin Noise Settings")]
    public float noiseScale = 10f;
    public int seed = -1;

    [Header("Island Shape Settings")]
    public float islandFalloffPower = 2.5f;   //cuanto mas alto, mas suave el borde
    public float islandSizeFactor = 0.75f;    //cuanto mas bajo, mas pequeña la isla
    public float coastRoughness = 0.25f;      //cuanto mas alto, mas irregular el contorno

    [Header("Cellular Automata")]
    [Range(0, 5)] public int smoothingIterations = 2;

    [Header("Object Prefabs (per biome)")]
    public GameObject[] grassObjects;
    public GameObject[] sandObjects;

    [Range(0f, 1f)]
    public float spawnChance = 0.1f;

    private float seedOffsetX;
    private float seedOffsetY;
    private Transform mapParent;

    private int[,] terrainGrid;

    public Node[,] nodes;


    private void Start()
    {
        if (seed == -1)
            seed = Random.Range(0, 1000000);

        seedOffsetX = Random.Range(0f, 100000f);
        seedOffsetY = Random.Range(0f, 100000f);

        Regenerate();

    }

    [ContextMenu("Regenerate Map")]
    public void Regenerate()
    {
        GenerateMap();
        SpawnObjectsFromNodes();
        SpawnPlayer();
    }

    private void GenerateMap()
    {
        tilemap.ClearAllTiles();
        nodes = new Node[width, height];

        terrainGrid = new int[width, height];

        Vector2 center = new Vector2(width / 2f, height / 2f);
        float maxDistance = Mathf.Min(width, height) * islandSizeFactor / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float baseNoise = Mathf.PerlinNoise(
                    (x + seedOffsetX) / noiseScale,
                    (y + seedOffsetY) / noiseScale
                );

                float distanceFromCenter = Vector2.Distance(new Vector2(x, y), center);
                float falloff = Mathf.Pow(distanceFromCenter / maxDistance, islandFalloffPower);
                float islandMask = Mathf.Clamp01(1f - falloff);

                float roughness = Mathf.PerlinNoise(x * 0.2f + seedOffsetX, y * 0.2f + seedOffsetY) * coastRoughness;
                float combined = baseNoise * (islandMask - roughness);

                if (combined < 0.3f)
                    terrainGrid[x, y] = 0; //water
                else if (combined < 0.45f)
                    terrainGrid[x, y] = 1; //sand
                else
                    terrainGrid[x, y] = 2; //grass
            }
        }

        //cellular automata
        for (int i = 0; i < smoothingIterations; i++)
            terrainGrid = CellularStep(terrainGrid);

        PostProcessTerrain(terrainGrid);

        DrawIsometricGridAndPopulateNodes(terrainGrid);

        Debug.Log($"[PCG] Isla generada con semilla {seed}");
    }

    private int[,] CellularStep(int[,] grid)
    {
        int[,] newGrid = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int waterNeighbors = CountNeighbors(grid, x, y, 0);
                int landNeighbors = 8 - waterNeighbors;

                if (waterNeighbors > 4)
                    newGrid[x, y] = 0;
                else if (landNeighbors > 4)
                    newGrid[x, y] = 2;
                else
                    newGrid[x, y] = grid[x, y];
            }
        }

        return newGrid;
    }

    private int CountNeighbors(int[,] grid, int x, int y, int target)
    {
        int count = 0;
        for (int nx = x - 1; nx <= x + 1; nx++)
        {
            for (int ny = y - 1; ny <= y + 1; ny++)
            {
                if (nx == x && ny == y) continue;
                if (nx < 0 || ny < 0 || nx >= width || ny >= height) continue;
                if (grid[nx, ny] == target) count++;
            }
        }
        return count;
    }

    private void PostProcessTerrain(int[,] grid)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == 2 && CountNeighbors(grid, x, y, 0) > 0)
                    grid[x, y] = 1; //grass next to water -> sand
            }
        }
    }

    private void DrawIsometricGridAndPopulateNodes(int[,] grid)
    {
        tilemap.ClearAllTiles();
        nodes = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileBase tileToPlace = grid[x, y] switch
                {
                    0 => ruleWater,
                    1 => ruleSand,
                    _ => ruleGrass
                };

                Vector3Int cell = new Vector3Int(x, y, 0);
                tilemap.SetTile(cell, tileToPlace);

                //sorting
                int sortingOrder = (x + y);
                float zFix = sortingOrder * 0.0001f;

                Matrix4x4 m = Matrix4x4.TRS(new Vector3(0f, 0f, zFix), Quaternion.identity, Vector3.one);
                tilemap.SetTransformMatrix(cell, m);

                //true para sand y grass, false para water
                bool walkable = grid[x, y] != 0;
                nodes[x, y] = new Node(new Vector2Int(x, y), walkable);
            }
        }

        tilemap.CompressBounds();
    }

    private void SpawnObjectsFromNodes()
    {
        Transform parent = transform.Find("SpawnedObjects");
        if (parent != null)
            DestroyImmediate(parent.gameObject);

        parent = new GameObject("SpawnedObjects").transform;
        parent.parent = transform;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node node = nodes[x, y];

                if (!node.walkable)
                    continue;

                if (Random.value > spawnChance)
                    continue;

                Vector3Int cell = new Vector3Int(x, y, 0);
                TileBase currentTile = tilemap.GetTile(cell);

                GameObject[] pool = null;
                if (currentTile == ruleGrass)
                    pool = grassObjects;
                else if (currentTile == ruleSand)
                    pool = sandObjects;

                if (pool == null || pool.Length == 0)
                    continue;

                GameObject prefab = pool[Random.Range(0, pool.Length)];
                Vector3 worldPos = tilemap.CellToWorld(cell);

                GameObject spawned = Instantiate(prefab, worldPos, Quaternion.identity, parent);
                spawned.name = $"{prefab.name}_{x}_{y}";

                SpriteRenderer sr = spawned.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sortingOrder = x + y;

                //si bloquea el paso:
                //node.walkable = false;
            }
        }

    }
    private void SpawnPlayer()
    {
        if (player == null)
        {
            Debug.LogWarning("No se asignó el prefab del jugador en el inspector.");
            return;
        }

        List<Vector2Int> validPositions = new List<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node node = nodes[x, y];
                if (node.walkable)
                {
                    TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                    if (tile == ruleGrass)
                        validPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        if (validPositions.Count == 0)
        {
            Debug.LogWarning("No se encontraron posiciones válidas para el jugador.");
            return;
        }

        Vector2Int chosenPos = validPositions[Random.Range(0, validPositions.Count)];
        Vector3 worldPos = tilemap.CellToWorld(new Vector3Int(chosenPos.x, chosenPos.y, 0));
        Vector3 spawnPosition = new Vector3(worldPos.x, worldPos.y + 0.25f, 0);

        GameObject spawnedPlayer = Instantiate(player, spawnPosition, Quaternion.identity);
        spawnedPlayer.name = "Player";

        Debug.Log($"Jugador spawneado en {chosenPos}");

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<IsometricCamera>().AssingPlayer(spawnedPlayer);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            seed = Random.Range(0, 1000000);
            seedOffsetX = Random.Range(0f, 100000f);
            seedOffsetY = Random.Range(0f, 100000f);
            GenerateMap();
            SpawnObjectsFromNodes();
        }
    }
}