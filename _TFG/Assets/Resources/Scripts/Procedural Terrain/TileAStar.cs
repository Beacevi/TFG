/**
 * @file TileAStar.cs
 * @brief Calcula rutas sobre la rejilla de tiles mediante A* para conectar puntos del terreno procedural.
 * @author Hortensia Studio - Alejandro Romero Burgada
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

/// <summary>
/// Calcula rutas sobre la rejilla de tiles mediante A* para conectar puntos del terreno procedural.
/// </summary>
public class TileAStar : MonoBehaviour
{
    /// <summary>
    /// Tilemap sobre el que se dibuja o consulta el terreno.
    /// </summary>
    [SerializeField] Tilemap tilemap;
    /// <summary>
    /// Referencia al jugador o a su objeto asociado en la escena.
    /// </summary>
    [SerializeField] Transform player;
    /// <summary>
    /// Campo de tipo IsometricCamera utilizado para almacenar o configurar isometric camera.
    /// </summary>
    [SerializeField] IsometricCamera isometricCamera;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar move speed.
    /// </summary>
    [SerializeField] float moveSpeed = 3f;
    /// <summary>
    /// Campo de tipo PCGtiles_IsometricPerlin utilizado para almacenar o configurar map generator.
    /// </summary>
    [SerializeField] PCGtiles_IsometricPerlin mapGenerator;

    /// <summary>
    /// Campo de tipo ChangeScene utilizado para almacenar o configurar cambia escenas.
    /// </summary>
    public ChangeScene cambiaEscenas;

    /// <summary>
    /// Campo de tipo SpriteRenderer utilizado para almacenar o configurar imagen pajaro conseguido.
    /// </summary>
    public SpriteRenderer imagenPajaroConseguido;
    
    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar texto nombre pajaro.
    /// </summary>
    public string textoNombrePajaro;

    /// <summary>
    /// Colección de path utilizada por este componente.
    /// </summary>
    private List<Vector3> path = new List<Vector3>();
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar current index.
    /// </summary>
    private int currentIndex = 0;
    /// <summary>
    /// Indica si moving está activo o habilitado.
    /// </summary>
    public bool moving = false;

    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar inf.
    /// </summary>
    private const int INF = int.MaxValue / 4;

    /// <summary>
    /// Campo de tipo Node utilizado para almacenar o configurar last path node.
    /// </summary>
    private Node lastPathNode = null;
    /// <summary>
    /// Campo de tipo Node utilizado para almacenar o configurar interactable node.
    /// </summary>
    private Node interactableNode = null;

    /// <summary>
    /// Campo de tipo Image utilizado para almacenar o configurar pasos bar.
    /// </summary>
    [SerializeField] private Image pasosBar;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar steps available.
    /// </summary>
    public int stepsAvailable = 30;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar initial steps.
    /// </summary>
    private int initialSteps;

    /// <summary>
    /// Indica si can move está activo o habilitado.
    /// </summary>
    private bool canMove = false;

    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar input blocked until.
    /// </summary>
    private static float inputBlockedUntil = 0f;

    /// <summary>
    /// Referencia cacheada al script Sounds para evitar llamadas repetidas a GetComponent
    /// al reproducir el clic de mapa y los pasos del jugador.
    /// </summary>
    private Sounds _soundsCache;

    /// <summary>
    /// AudioSource dedicado para el bucle de pasos del jugador.
    /// Se crea en Start() y se reutiliza en cada movimiento.
    /// </summary>
    private AudioSource _footstepsSource;

    /// <summary>
    /// Obtiene la instancia de Sounds del GameManager de forma perezosa y cacheada.
    /// Devuelve null si el GameManager aún no está disponible.
    /// </summary>
    private Sounds GetSounds()
    {
        if (_soundsCache != null) return _soundsCache;
        if (GameManager.Instance == null) return null;
        _soundsCache = GameManager.Instance.GetComponent<Sounds>();
        return _soundsCache;
    }

    /// <summary>
    /// Arranca (si no está sonando) el bucle de pasos del jugador.
    /// </summary>
    private void StartFootsteps()
    {
        if (_footstepsSource == null) return;
        var s = GetSounds();
        if (s == null || s.sonidoPasos == null) return;

        if (_footstepsSource.clip != s.sonidoPasos)
            _footstepsSource.clip = s.sonidoPasos;

        if (!_footstepsSource.isPlaying)
            _footstepsSource.Play();
    }

    /// <summary>
    /// Detiene el bucle de pasos del jugador (si está sonando).
    /// </summary>
    private void StopFootsteps()
    {
        if (_footstepsSource != null && _footstepsSource.isPlaying)
            _footstepsSource.Stop();
    }

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    private void Start()
    {
        inputBlockedUntil = 0f; // reset al cargar escena

        if (!isometricCamera)
        {
            isometricCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<IsometricCamera>();
        }
        
        if (!tilemap)
        {
            tilemap = GameObject.FindGameObjectWithTag("MainTileMap").GetComponent<Tilemap>();
        }
            
        if (!mapGenerator)
        {
            mapGenerator = GameObject.FindGameObjectWithTag("MapGenerator").GetComponent<PCGtiles_IsometricPerlin>();
        }
            

        if (!cambiaEscenas)
        {
            cambiaEscenas = GameObject.FindGameObjectWithTag("SceneChanger").GetComponent<ChangeScene>();
        }

        initialSteps = stepsAvailable;
        UpdateStepsUI();

        // AudioSource propio para el bucle de pasos. No se inicia hasta que el
        // jugador empieza a moverse (StartFootsteps).
        _footstepsSource = gameObject.GetComponent<AudioSource>();
        if (_footstepsSource == null)
            _footstepsSource = gameObject.AddComponent<AudioSource>();
        _footstepsSource.playOnAwake = false;
        _footstepsSource.loop = true;
    }

    /// <summary>
    /// Actualiza steps ui para reflejar el estado actual del sistema.
    /// </summary>
    public void UpdateStepsUI()
    {
        if (pasosBar == null)
        {
            pasosBar = GameObject.FindGameObjectWithTag("PasosBar").GetComponent<Image>();
        } 

        pasosBar.fillAmount = (float)stepsAvailable / initialSteps;
    }

    /// <summary>
    /// Actualiza la lógica del componente en cada fotograma.
    /// </summary>
    void Update()
    {
        if (SimonGameManagerPajaro.IsActive)
            return;

        HandleMovement();
    }
    /// <summary>
    /// Ejecuta la lógica asociada a handle movement dentro de TileAStar.
    /// </summary>
    void HandleMovement()
    {
        if (moving && path.Count > 0)
        {
            var target = path[currentIndex];
            player.position = Vector3.MoveTowards(player.position, target, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(player.position, target) < 0.05f)
            {
                currentIndex++;

                if (currentIndex >= path.Count)
                {
                    ResolveArrival();
                }
            }
        }
    }
    /// <summary>
    /// Ejecuta la lógica asociada a resolve arrival dentro de TileAStar.
    /// </summary>
    void ResolveArrival()
    {
        // Cortar el bucle de pasos al llegar al destino, antes de cualquier interacción.
        StopFootsteps();

        if (lastPathNode != null)
        {
            if (lastPathNode.hasObject)
            {
                var interactable = lastPathNode.Interactable?.GetComponent<InteractableGameObject>();

                if (interactable != null)
                {
                    interactableNode = lastPathNode;
                    interactable.Interact(this);

                }
                else
                {
                    Debug.Log("InteractableGameObject no encontrado.");
                }
            }
        }

        moving = false;
        path.Clear();
    }

    /// <summary>
    /// Ejecuta la lógica asociada a process click dentro de TileAStar.
    /// </summary>
    /// <param name="screenPos">Parámetro screen pos empleado por el método.</param>
    public void ProcessClick(Vector3 screenPos)
    {
        //BLOQUEO GLOBAL
        if (Time.time < inputBlockedUntil) return;

        if (!canMove) return;

        // Si está abierto el panel de confirmación de salida de la isla, ignorar clics.
        if (IslandExitButton.IsConfirmOpen) return;

        if (IsPointerOverUI(screenPos)) return;

        Vector3 w = Camera.main.ScreenToWorldPoint(screenPos);
        w.z = 0;

        Vector3Int clicked = tilemap.WorldToCell(w);
        Vector3Int start = tilemap.WorldToCell(player.position);

        path = FindPath(start, clicked);

        // ¿Es el tile destino un interactuable (pájaro / moneda)?
        bool targetHasObject = path.Count > 0
            && mapGenerator.nodes != null
            && InBounds(clicked, mapGenerator.width, mapGenerator.height)
            && mapGenerator.nodes[clicked.x, clicked.y].hasObject;

        // Si el destino tiene un pájaro, el jugador se para en el tile adyacente
        // (lastPathNode sigue apuntando al nodo del pájaro para disparar la interacción).
        if (path.Count > 1 && targetHasObject)
        {
            path.RemoveAt(path.Count - 1);
        }

        if (path.Count > stepsAvailable)
        {
            // No hay pasos para llegar: truncamos la ruta y, si íbamos a un
            // interactuable, anulamos lastPathNode para que ResolveArrival no
            // dispare la interacción con un objeto al que no hemos llegado.
            path = path.GetRange(0, stepsAvailable);
            if (targetHasObject)
            {
                lastPathNode = null;
            }
        }

        if (path.Count > 0)
        {
            stepsAvailable -= path.Count;
            stepsAvailable = Mathf.Max(0, stepsAvailable);

            UpdateStepsUI();

            // Sonido de clic en el mapa: solo si el clic se traduce en un movimiento real.
            GetSounds()?.SonidoClickMapa();

            // Bucle de pasos mientras dure el desplazamiento.
            StartFootsteps();

            moving = true;
            currentIndex = 0;
            Debug.Log("Llamando AttachToPlayer");
            isometricCamera.AttachToPlayer();
        }
    }
    /// <summary>
    /// Ejecuta la lógica asociada a is pointer over ui dentro de TileAStar.
    /// </summary>
    /// <param name="screenPos">Parámetro screen pos empleado por el método.</param>
    /// <returns>true si la operación se ha completado correctamente; false en caso contrario.</returns>
    bool IsPointerOverUI(Vector3 screenPos)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = screenPos;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("MainTileMap"))
            {
                return false; // tocó el tilemap → movimiento válido
            }
            // Si antes de llegar al tilemap hay otro objeto, es UI
            return true;
        }

        return false; // no tocó nada
    }

    /// <summary>
    /// Establece o actualiza can move dentro del sistema.
    /// </summary>
    /// <param name="b">Parámetro b empleado por el método.</param>
    public void SetCanMove(bool b)
    {
        canMove = b;
    }


    /// <summary>
    /// Elimina bird at last node del estado gestionado por el componente.
    /// </summary>
    public void RemoveBirdAtLastNode()
    {
        if (interactableNode == null) return;

        if (interactableNode.hasObject && interactableNode.Interactable != null)
        {
            if (ScenePersistentManager.instance.interactedBird != null)
            {
                ScenePersistentManager.instance.interactedBird.obtenido = true;
            }

            foreach (Transform hijo in ScenePersistentManager.instance.interactedBird.birdPrefab.transform)
            {
                if (hijo.name == "Normal")//El sprite de cuando no estan detras de un arbol
                {
                    imagenPajaroConseguido = hijo.GetComponent<SpriteRenderer>();
                    textoNombrePajaro = interactableNode.Interactable.GetComponent<InteractableGameObject>().birdData.birdName;

                }
            }
            
            //cambiaEscenas.StartCoroutine("StartAnimation");
            
            ScenePersistentManager.instance.collectedBirdsList.Add(ScenePersistentManager.instance.interactedBird);

            StartCoroutine(isometricCamera.MostrarPanelPajaroConseguido(imagenPajaroConseguido,textoNombrePajaro));

            Destroy(interactableNode.Interactable);
            interactableNode.Interactable = null;
            interactableNode.hasObject = false;
            Debug.Log("[TileAStar] Pajaro eliminado del nodo.");
            GameManager.Instance.GetComponent<Sounds>().SonidoRecolectarPajaro();

            isometricCamera.cantidadPajaros++;
        }
        else
        {
            Debug.Log("[TileAStar] No hay pajaro en interactableNode.");
        }
    }

    /// <summary>
    /// Elimina coin at last node del estado gestionado por el componente.
    /// </summary>
    public void RemoveCoinAtLastNode()
    {
        if (interactableNode == null)
        {
            return;
        }
        Destroy(interactableNode.Interactable);
        interactableNode.Interactable = null;
        interactableNode.hasObject = false;

        isometricCamera.cantidadMonedas++;

        Debug.Log("[TileAStar] Moneda eliminada del nodo.");
        GameManager.Instance.GetComponent<Sounds>().SonidoRecolectarPajaro();
    }

    /// <summary>
    /// Ejecuta la lógica asociada a disable bird interaction dentro de TileAStar.
    /// </summary>
    public void DisableBirdInteraction()
    {
        if (interactableNode == null) return;

        if (interactableNode.Interactable != null)
        {
            var interactable = interactableNode.Interactable.GetComponent<InteractableGameObject>();
            if (interactable != null) interactable.enabled = false;
        }
        interactableNode.hasObject = false;
    }

    /// <summary>
    /// Ejecuta la lógica asociada a find path dentro de TileAStar.
    /// </summary>
    /// <param name="startCell">Parámetro start cell empleado por el método.</param>
    /// <param name="targetCell">Parámetro target cell empleado por el método.</param>
    /// <returns>Instancia o valor de tipo List<Vector3> resultante de la operación.</returns>
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

    /// <summary>
    /// Ejecuta la lógica asociada a in bounds dentro de TileAStar.
    /// </summary>
    /// <param name="cell">Parámetro cell empleado por el método.</param>
    /// <param name="w">Parámetro w empleado por el método.</param>
    /// <param name="h">Parámetro h empleado por el método.</param>
    /// <returns>true si la operación se ha completado correctamente; false en caso contrario.</returns>
    bool InBounds(Vector3Int cell, int w, int h)
        => cell.x >= 0 && cell.y >= 0 && cell.x < w && cell.y < h;

    /// <summary>
    /// Ejecuta la lógica asociada a is diagonal dentro de TileAStar.
    /// </summary>
    /// <param name="a">Parámetro a empleado por el método.</param>
    /// <param name="b">Parámetro b empleado por el método.</param>
    /// <returns>true si la operación se ha completado correctamente; false en caso contrario.</returns>
    bool IsDiagonal(Node a, Node b)
        => a.position.x != b.position.x && a.position.y != b.position.y;

    /// <summary>
    /// Obtiene neighbors a partir del estado actual del sistema.
    /// </summary>
    /// <param name="node">Parámetro node empleado por el método.</param>
    /// <param name="Node[">Parámetro node empleado por el método.</param>
    /// <param name="nodes">Parámetro nodes empleado por el método.</param>
    /// <returns>Instancia o valor de tipo List<Node> resultante de la operación.</returns>
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

    /// <summary>
    /// Ejecuta la lógica asociada a heuristic dentro de TileAStar.
    /// </summary>
    /// <param name="a">Parámetro a empleado por el método.</param>
    /// <param name="b">Parámetro b empleado por el método.</param>
    /// <returns>Valor numérico calculado o consultado por el método.</returns>
    int Heuristic(Node a, Node b)
    {
        int dx = Mathf.Abs(a.position.x - b.position.x);
        int dy = Mathf.Abs(a.position.y - b.position.y);
        return 14 * Mathf.Min(dx, dy) + 10 * Mathf.Abs(dx - dy);
    }

    /// <summary>
    /// Ejecuta la lógica asociada a retrace dentro de TileAStar.
    /// </summary>
    /// <param name="start">Parámetro start empleado por el método.</param>
    /// <param name="end">Parámetro end empleado por el método.</param>
    /// <returns>Instancia o valor de tipo List<Vector3> resultante de la operación.</returns>
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


    /// <summary>
    /// Ejecuta la lógica asociada a on draw gizmos dentro de TileAStar.
    /// </summary>
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

    /// <summary>
    /// Ejecuta la lógica asociada a block input for seconds dentro de TileAStar.
    /// </summary>
    /// <param name="seconds">Parámetro seconds empleado por el método.</param>
    public static void BlockInputForSeconds(float seconds)
    {
        inputBlockedUntil = Time.time + seconds;
    }

    /// <summary>
    /// Añade steps al estado gestionado por el componente.
    /// </summary>
    /// <param name="amount">Cantidad que se debe aplicar en la operación.</param>
    public void AddSteps(int amount)
    {
        stepsAvailable += amount;
        UpdateStepsUI();
    }

}
