/**
 * @file PCGtiles_IsometricPerlin.cs
 * @brief Genera una isla isométrica mediante ruido procedural, máscara insular, autómata celular y colocación de objetos.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Genera una isla isométrica mediante ruido procedural, máscara insular, autómata celular y colocación de objetos.
/// </summary>
public class PCGtiles_IsometricPerlin : MonoBehaviour
{
    //STP 1: SET TILES 
    //STP 2: PERLIN NOISE
    //STP 3: ISLAND (2nd Perlin noise with offset)
    //STP 4: CELULLAR AUTOMATA
    //STP 5: RULE TILES

    /// <summary>
    /// Referencia al jugador o a su objeto asociado en la escena.
    /// </summary>
    [Header("Player")]
    [SerializeField] GameObject player;

    /// <summary>
    /// Tilemap sobre el que se dibuja o consulta el terreno.
    /// </summary>
    [Header("Tilemap")]
    [SerializeField] Tilemap tilemap;

    /// <summary>
    /// Campo de tipo TileBase utilizado para almacenar o configurar rule grass.
    /// </summary>
    [Header("Tiles (RuleTile or TileBase)")]
    [SerializeField] TileBase ruleGrass;
    /// <summary>
    /// Campo de tipo TileBase utilizado para almacenar o configurar rule sand.
    /// </summary>
    [SerializeField] TileBase ruleSand;
    /// <summary>
    /// Campo de tipo TileBase utilizado para almacenar o configurar rule water.
    /// </summary>
    [SerializeField] TileBase ruleWater;

    /// <summary>
    /// Anchura del mapa o área generada.
    /// </summary>
    [Header("Map Settings")]
    public int width = 50;
    /// <summary>
    /// Altura del mapa o área generada.
    /// </summary>
    public int height = 50;

    /// <summary>
    /// Escala del ruido procedural utilizada para modular la generación.
    /// </summary>
    [Header("Perlin Noise Settings")]
    [SerializeField] float noiseScale = 10f;
    /// <summary>
    /// Semilla utilizada para obtener resultados procedurales reproducibles.
    /// </summary>
    [SerializeField] int seed = -1;

    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar island falloff power.
    /// </summary>
    [Header("Island Shape Settings")]
    [SerializeField] float islandFalloffPower = 2.5f;   //cuanto mas alto, mas suave el borde
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar island size factor.
    /// </summary>
    [SerializeField] float islandSizeFactor = 0.75f;    //cuanto mas bajo, mas peque�a la isla
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar coast roughness.
    /// </summary>
    [SerializeField] float coastRoughness = 0.25f;      //cuanto mas alto, mas irregular el contorno

    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar smoothing iterations.
    /// </summary>
    [Header("Cellular Automata")]
    [Range(0, 5)] public int smoothingIterations = 2;

    [Header("Interactable Objects")]
    //[SerializeField] LayerMask interactableGameObjectsLayerMask;
    /// <summary>
    /// Colección de interactable game objects utilizada por este componente.
    /// </summary>
    [SerializeField] Bird[] interactableGameObjects;
    /// <summary>
    /// Valor numérico que limita o define interactable game objects count.
    /// </summary>
    [SerializeField] int interactableGameObjectsCount = 10;
    //[Range(0f, 1f)]
    //[SerializeField] float spawnChance = 0.1f;


    [Header("Object Prefabs (per biome)")]
    //[SerializeField] LayerMask terrainLayerMask;
    /// <summary>
    /// Colección de grass objects utilizada por este componente.
    /// </summary>
    [SerializeField] GameObject[] grassObjects;
    /// <summary>
    /// Colección de sand objects utilizada por este componente.
    /// </summary>
    [SerializeField] GameObject[] sandObjects;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar spawn chance.
    /// </summary>
    [Range(0f, 1f)]
    [SerializeField] float spawnChance = 0.1f;

    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar beach min width.
    /// </summary>
    [Header("Beach Settings")]
    [SerializeField] int beachMinWidth = 1;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar beach max width.
    /// </summary>
    [SerializeField] int beachMaxWidth = 4;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar beach noise scale.
    /// </summary>
    [SerializeField] float beachNoiseScale = 6f;

    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar seed offset x.
    /// </summary>
    private float seedOffsetX;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar seed offset y.
    /// </summary>
    private float seedOffsetY;
    /// <summary>
    /// Campo de tipo Transform utilizado para almacenar o configurar map parent.
    /// </summary>
    private Transform mapParent;

    /// <summary>
    /// Campo de tipo int[,] utilizado para almacenar o configurar terrain grid.
    /// </summary>
    private int[,] terrainGrid;

    /// <summary>
    /// Campo de tipo Node[,] utilizado para almacenar o configurar nodes.
    /// </summary>
    public Node[,] nodes;
    /// <summary>
    /// Cantidad de monedas almacenadas o mostradas por el sistema.
    /// </summary>
    [Header("Coins")]
    [SerializeField] Coin[] coins;
    /// <summary>
    /// Valor numérico que limita o define coins count.
    /// </summary>
    [SerializeField] int coinsCount = 15;
    /// <summary>
    /// Indica si coins on grass only está activo o habilitado.
    /// </summary>
    [SerializeField] bool coinsOnGrassOnly = false; // o filtra por bioma que quieras


    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    private void Start()
    {
        if (seed == -1)
            seed = Random.Range(0, 1000000);

        seedOffsetX = Random.Range(0f, 100000f);
        seedOffsetY = Random.Range(0f, 100000f);

        Regenerate();

    }

    /// <summary>
    /// Ejecuta la lógica asociada a regenerate dentro de PCGtiles_IsometricPerlin.
    /// </summary>
    [ContextMenu("Regenerate Map")]
    public void Regenerate()
    {
        GenerateMap();
        SpawnInteractablesObjectsFromNodes();
        SpawnCoinsFromNodes();
        SpawnDecorationFromNodes();
        SpawnPlayer(10);
    }

    /// <summary>
    /// Genera map utilizando los parámetros configurados.
    /// </summary>
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

    /// <summary>
    /// Ejecuta la lógica asociada a cellular step dentro de PCGtiles_IsometricPerlin.
    /// </summary>
    /// <param name="int[">Parámetro int empleado por el método.</param>
    /// <param name="grid">Parámetro grid empleado por el método.</param>
    /// <returns>Instancia o valor de tipo int[,] resultante de la operación.</returns>
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

    /// <summary>
    /// Ejecuta la lógica asociada a count neighbors dentro de PCGtiles_IsometricPerlin.
    /// </summary>
    /// <param name="int[">Parámetro int empleado por el método.</param>
    /// <param name="grid">Parámetro grid empleado por el método.</param>
    /// <param name="x">Posición o coordenada utilizada en el cálculo.</param>
    /// <param name="y">Posición o coordenada utilizada en el cálculo.</param>
    /// <param name="target">Parámetro target empleado por el método.</param>
    /// <returns>Valor numérico calculado o consultado por el método.</returns>
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

    /// <summary>
    /// Ejecuta la lógica asociada a post process terrain dentro de PCGtiles_IsometricPerlin.
    /// </summary>
    /// <param name="int[">Parámetro int empleado por el método.</param>
    /// <param name="grid">Parámetro grid empleado por el método.</param>
    private void PostProcessTerrain(int[,] grid)
    {
        int[,] originalGrid = (int[,])grid.Clone();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (originalGrid[x, y] != 2)
                    continue;

                int distanceToWater = GetDistanceToWater(originalGrid, x, y, beachMaxWidth);

                if (distanceToWater == -1)
                    continue;

                float beachNoise = Mathf.PerlinNoise(
                    (x + seedOffsetX) / beachNoiseScale,
                    (y + seedOffsetY) / beachNoiseScale
                );

                float localBeachWidth = Mathf.Lerp(beachMinWidth, beachMaxWidth, beachNoise);

                if (distanceToWater <= localBeachWidth)
                    grid[x, y] = 1;
            }
        }
    }

    /// <summary>
    /// Obtiene distance to water a partir del estado actual del sistema.
    /// </summary>
    /// <param name="int[">Parámetro int empleado por el método.</param>
    /// <param name="grid">Parámetro grid empleado por el método.</param>
    /// <param name="x">Posición o coordenada utilizada en el cálculo.</param>
    /// <param name="y">Posición o coordenada utilizada en el cálculo.</param>
    /// <param name="maxDistance">Parámetro max distance empleado por el método.</param>
    /// <returns>Valor numérico calculado o consultado por el método.</returns>
    private int GetDistanceToWater(int[,] grid, int x, int y, int maxDistance)
    {
        for (int distance = 1; distance <= maxDistance; distance++)
        {
            for (int dx = -distance; dx <= distance; dx++)
            {
                for (int dy = -distance; dy <= distance; dy++)
                {
                    if (Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)) != distance)
                        continue;

                    int nx = x + dx;
                    int ny = y + dy;

                    if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                        continue;

                    if (grid[nx, ny] == 0)
                        return distance;
                }
            }
        }

        return -1;
    }

    /// <summary>
    /// Ejecuta la lógica asociada a draw isometric grid and populate nodes dentro de PCGtiles_IsometricPerlin.
    /// </summary>
    /// <param name="int[">Parámetro int empleado por el método.</param>
    /// <param name="grid">Parámetro grid empleado por el método.</param>
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

    /// <summary>
    /// Obtiene random bird by chance a partir del estado actual del sistema.
    /// </summary>
    /// <returns>Instancia o valor de tipo Bird resultante de la operación.</returns>
    private Bird GetRandomBirdByChance()
    {
        float totalChance = 0f;

        foreach (Bird bird in interactableGameObjects)
        {
            totalChance += bird.spawnChance;
        }

        float randomValue = Random.Range(0f, totalChance);

        float currentChance = 0f;

        foreach (Bird bird in interactableGameObjects)
        {
            currentChance += bird.spawnChance;

            if (randomValue <= currentChance)
                return bird;
        }

        return interactableGameObjects[0];
    }

    /// <summary>
    /// Instancia o coloca interactables objects from nodes dentro de la escena.
    /// </summary>
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
            Bird birdSpawned = GetRandomBirdByChance();
            if (birdSpawned.birdPrefab == null)
            {
                Debug.LogWarning($"El prefab del bird {birdSpawned.name} no esta asignado");
                continue;
            }

            //Vector3 worldPos = tilemap.CellToWorld(cell);
            Vector3 worldPos = tilemap.CellToWorld(cell);
            //worldPos.y += 0.75f;
            Vector3 cellSize = tilemap.layoutGrid.cellSize;
            worldPos += new Vector3(0, cellSize.y * 0.5f, 0);
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

    /// <summary>
    /// Obtiene random coin by chance a partir del estado actual del sistema.
    /// </summary>
    /// <returns>Instancia o valor de tipo Coin resultante de la operación.</returns>
    private Coin GetRandomCoinByChance()
    {
        float total = coins.Sum(c => c.spawnChance);
        float roll = Random.Range(0f, total);
        float current = 0f;

        foreach (Coin coin in coins)
        {
            current += coin.spawnChance;
            if (roll <= current) return coin;
        }

        return coins[0];
    }

    /// <summary>
    /// Instancia o coloca coins from nodes dentro de la escena.
    /// </summary>
    private void SpawnCoinsFromNodes()
    {
        Transform parent = transform.Find("SpawnedCoins");
        if (parent != null) DestroyImmediate(parent.gameObject);

        parent = new GameObject("SpawnedCoins").transform;
        parent.parent = transform;

        if (coins == null || coins.Length == 0) return;

        List<Vector2Int> validNodes = new List<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node node = nodes[x, y];
                if (!node.walkable || node.hasObject) continue;

                TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));

                bool validBiome = coinsOnGrassOnly ? tile == ruleGrass
                                                : (tile == ruleGrass || tile == ruleSand);
                if (validBiome)
                    validNodes.Add(new Vector2Int(x, y));
            }
        }

        int amountToSpawn = Mathf.Min(coinsCount, validNodes.Count);
        validNodes = validNodes.OrderBy(_ => Random.value).ToList();

        for (int i = 0; i < amountToSpawn; i++)
        {
            Vector2Int pos = validNodes[i];
            Node node = nodes[pos.x, pos.y];

            Coin coinData = GetRandomCoinByChance();
            if (coinData.coinPrefab == null)
            {
                Debug.LogWarning($"El prefab de la moneda '{coinData.name}' no está asignado.");
                continue;
            }

            Vector3Int cell = new Vector3Int(pos.x, pos.y, 0);
            Vector3 worldPos = tilemap.CellToWorld(cell);
            worldPos.y += 0.75f;

            GameObject spawned = Instantiate(coinData.coinPrefab, worldPos, Quaternion.identity, parent);
            spawned.name = $"{coinData.coinPrefab.name}_{pos.x}_{pos.y}";

            int maxOrder = width + height;
            SpriteRenderer sr = spawned.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = maxOrder - (pos.x + pos.y);
            }
            else
            {
                foreach (Transform child in spawned.transform)
                {
                    sr = child.GetComponent<SpriteRenderer>();
                    if (sr != null) sr.sortingOrder = maxOrder - (pos.x + pos.y);
                }
            }

            node.hasObject = true;
            node.Interactable = spawned;
        }
    }

    /// <summary>
    /// Instancia o coloca decoration from nodes dentro de la escena.
    /// </summary>
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
                //worldPos += new Vector3(0, 1.25f/4, 0); 
                Vector3 cellSize = tilemap.layoutGrid.cellSize;
                worldPos += new Vector3(0, cellSize.y * 0.5f, 0);

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
    /// <summary>
    /// Instancia o coloca player dentro de la escena.
    /// </summary>
    /// <param name="stepsAvailable">Parámetro steps available empleado por el método.</param>
    private void SpawnPlayer(int stepsAvailable)
    {
        if (player == null)
        {
            Debug.LogWarning("No se asign� el prefab del jugador en el inspector.");
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
            Debug.LogWarning("No se encontraron posiciones v�lidas para el jugador.");
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

    /// <summary>
    /// Actualiza la lógica del componente en cada fotograma.
    /// </summary>
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
