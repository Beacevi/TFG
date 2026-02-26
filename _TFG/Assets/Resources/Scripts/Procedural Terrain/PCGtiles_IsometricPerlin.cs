using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] Tilemap tilemap;

    [Header("Tiles (RuleTile or TileBase)")]
    [SerializeField] TileBase ruleGrass;
    [SerializeField] TileBase ruleSand;
    [SerializeField] TileBase ruleWater;

    [Header("Map Settings")]
    public int width = 50;
    public int height = 50;

    [Header("Perlin Noise Settings")]
    [SerializeField] float noiseScale = 10f;
    [SerializeField] int seed = -1;

    [Header("Island Shape Settings")]
    [SerializeField] float islandFalloffPower = 2.5f;   //cuanto mas alto, mas suave el borde
    [SerializeField] float islandSizeFactor = 0.75f;    //cuanto mas bajo, mas pequeña la isla
    [SerializeField] float coastRoughness = 0.25f;      //cuanto mas alto, mas irregular el contorno

    [Header("Cellular Automata")]
    [Range(0, 5)] public int smoothingIterations = 2;

    [Header("Interactable Objects")]
    //[SerializeField] LayerMask interactableGameObjectsLayerMask;
    [SerializeField] Bird[] interactableGameObjects;
    [SerializeField] int interactableGameObjectsCount = 10;
    //[Range(0f, 1f)]
    //[SerializeField] float spawnChance = 0.1f;


    [Header("Object Prefabs (per biome)")]
    //[SerializeField] LayerMask terrainLayerMask;
    [SerializeField] GameObject[] grassObjects;
    [SerializeField] GameObject[] sandObjects;
    [Range(0f, 1f)]
    [SerializeField] float spawnChance = 0.1f;

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
        SpawnInteractablesObjectsFromNodes();
        SpawnDecorationFromNodes();
        SpawnPlayer(10);
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

    private void SpawnInteractablesObjectsFromNodes()
    {
        Transform parent = transform.Find("SpawnedInteractableObjects");
        if (parent != null)
            DestroyImmediate(parent.gameObject);

        parent = new GameObject("SpawnedInteractableObjects").transform;
        parent.parent = transform;

        if (interactableGameObjects == null || interactableGameObjects.Length == 0)
            return;

        List<Vector2Int> validNodes = new List<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node node = nodes[x, y];
                if (node.walkable && !node.hasObject)
                    validNodes.Add(new Vector2Int(x, y));
            }
        }

        int amountToSpawn = Mathf.Min(interactableGameObjectsCount, validNodes.Count);

        validNodes = validNodes.OrderBy(v => Random.value).ToList(); //los mezcla

        for (int i = 0; i < amountToSpawn; i++)
        {
            Vector2Int pos = validNodes[i];
            Node node = nodes[pos.x, pos.y];

            Vector3Int cell = new Vector3Int(pos.x, pos.y, 0);
            TileBase currentTile = tilemap.GetTile(cell);
            if (currentTile == null)
                continue;

            //GameObject prefab = interactableGameObjects[Random.Range(0, interactableGameObjects.Length)];
            Bird birdSpawned = interactableGameObjects[Random.Range(0, interactableGameObjects.Length)];
            if (birdSpawned.birdPrefab == null)
            {
                Debug.LogWarning($"El prefab del bird {birdSpawned.name} no está asignado");
                continue;
            }

            //Vector3 worldPos = tilemap.CellToWorld(cell);
            Vector3 worldPos = tilemap.CellToWorld(cell);
            GameObject spawned = Instantiate(birdSpawned.birdPrefab, worldPos, Quaternion.identity, parent);
            spawned.name = $"{birdSpawned.birdPrefab.name}_{pos.x}_{pos.y}";

            worldPos += new Vector3(0, 1.25f / 4, 0);

            //GameObject spawned = Instantiate(prefab, worldPos, Quaternion.identity, parent);
            //spawned.name = $"{prefab.name}_{pos.x}_{pos.y}";

            int maxOrder = width + height;
            SpriteRenderer sr = spawned.GetComponent<SpriteRenderer>();

            if (sr != null)
            {
                sr.sortingOrder = maxOrder - (pos.x + pos.y);
            }
            else
            {
                for (int c = 0; c < spawned.transform.childCount; c++)
                {
                    sr = spawned.transform.GetChild(c).GetComponent<SpriteRenderer>();
                    if (sr != null)
                        sr.sortingOrder = maxOrder - (pos.x + pos.y);
                }
            }

            node.hasObject = true;
            node.Interactable = spawned;


        }
    }

    private void SpawnDecorationFromNodes()
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

                if (!node.walkable || node.hasObject)
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
                worldPos += new Vector3(0, 1.25f/4, 0);

                GameObject spawned = Instantiate(prefab, worldPos, Quaternion.identity, parent);
                spawned.name = $"{prefab.name}_{x}_{y}";

                int maxOrder = width + height;
                SpriteRenderer sr = spawned.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingOrder = maxOrder - (x + y);
                    //sr.renderingLayerMask = terrainLayerMask;
                }
                else
                {
                    for (int i = 0; i < spawned.transform.childCount; i++)
                    {
                        sr = spawned.transform.GetChild(i).GetComponent<SpriteRenderer>();
                        //sr.renderingLayerMask = terrainLayerMask;
                        if (sr != null)
                            sr.sortingOrder = maxOrder - (x + y);
                    }
                }

                //si bloquea el paso:
                //node.walkable = false;
            }
        }

    }
    private void SpawnPlayer(int stepsAvailable)
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

        spawnedPlayer.GetComponent<TileAStar>().stepsAvailable = stepsAvailable;

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
            SpawnInteractablesObjectsFromNodes();
            SpawnDecorationFromNodes();
        }
    }
}