/**
 * @file SimonGameManagerPajaro.cs
 * @brief Controla la variante del minijuego de Simón dice utilizada para la domesticación de aves.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Rendering;

/// <summary>
/// Controla la variante del minijuego de Simón dice utilizada para la domesticación de aves.
/// </summary>
public class SimonGameManagerPajaro : MonoBehaviour
{
    /// <summary>
    /// Propiedad que expone o modifica instance.
    /// </summary>
    public static SimonGameManagerPajaro Instance { get; private set; }
    /// <summary>
    /// Indica si is active está activo o habilitado.
    /// </summary>
    public static bool IsActive = false;

    /// <summary>
    /// Colección de circles utilizada por este componente.
    /// </summary>
    public CircleButtonPajaro[] circles;
    /// <summary>
    /// Campo de tipo LevelIndicator utilizado para almacenar o configurar level indicator.
    /// </summary>
    public LevelIndicator levelIndicator;
    /// <summary>
    /// Colección de circle sounds utilizada por este componente.
    /// </summary>
    public AudioClip[] circleSounds;
    /// <summary>
    /// Colección de letter sprites utilizada por este componente.
    /// </summary>
    public Sprite[] letterSprites;
    /// <summary>
    /// Fuente de audio utilizada para reproducir sonidos del componente.
    /// </summary>
    private AudioSource audioSource;
    /// <summary>
    /// Referencia de interfaz utilizada para mostrar o actualizar contador text.
    /// </summary>
    public TextMeshProUGUI contadorText;

    /// <summary>
    /// Tiempo o duración asociado a flash duration.
    /// </summary>
    public float flashDuration = 1f;
    /// <summary>
    /// Tiempo o duración asociado a time between flashes.
    /// </summary>
    public float timeBetweenFlashes = 1f;

    /// <summary>
    /// Colección de pattern utilizada por este componente.
    /// </summary>
    private List<int> pattern = new List<int>();
    /// <summary>
    /// Colección de player input utilizada por este componente.
    /// </summary>
    private List<int> playerInput = new List<int>();

    /// <summary>
    /// Indica si is player turn está activo o habilitado.
    /// </summary>
    private bool isPlayerTurn = false;
    /// <summary>
    /// Indica si can press está activo o habilitado.
    /// </summary>
    private bool canPress = false;
    /// <summary>
    /// Indica si game started está activo o habilitado.
    /// </summary>
    private bool gameStarted = false;

    /// <summary>
    /// Botón de interfaz asociado a start button.
    /// </summary>
    public Button startButton;
    /// <summary>
    /// Botón de interfaz asociado a back button.
    /// </summary>
    public Button backButton;

    /// <summary>
    /// Panel de interfaz asociado a exit confirm panel.
    /// </summary>
    [Header("Confirmación de salida")]
    [Tooltip("Panel con los botones Sí/No que pide confirmación al pulsar el botón de salir.")]
    [SerializeField] private GameObject exitConfirmPanel;

    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar level.
    /// </summary>
    private int level = 0;
    /// <summary>
    /// Valor numérico que limita o define fail count.
    /// </summary>
    private int failCount = 0;
    /// <summary>
    /// Valor numérico que limita o define max fails.
    /// </summary>
    public int maxFails = 3;

    /// <summary>
    /// Color utilizado para representar fail color.
    /// </summary>
    public Color failColor = Color.red; // Color flash fallo
    /// <summary>
    /// Color utilizado para representar success color.
    /// </summary>
    public Color successColor = Color.green; // Color flash acierto
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar circle flashes.
    /// </summary>
    public int circleFlashes = 2;         // Cuántas veces parpadea

    /// <summary>
    /// Campo de tipo Image utilizado para almacenar o configurar pajaro.
    /// </summary>
    public Image pajaro;
    /// <summary>
    /// Campo de tipo Bird utilizado para almacenar o configurar selected bird.
    /// </summary>
    private Bird selectedBird;
    

    /// <summary>
    /// Inicializa referencias internas antes de que comience la ejecución normal del componente.
    /// </summary>
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    void Start()
    {
         // Obtener pájaro seleccionado
         if (ScenePersistentManager.instance.interactedBird != null)
         {
             selectedBird = ScenePersistentManager.instance.interactedBird;
         }
         

        isPlayerTurn = false;
        canPress = false;

        // GetComponentInChildren busca en hijos (el sprite real está en el hijo "Normal", no en la raíz)
        SpriteRenderer sr = selectedBird.birdPrefab.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            pajaro.sprite = sr.sprite;
        }
        else
        {
            Debug.LogWarning("No se encontró SpriteRenderer en el prefab del pájaro ni en sus hijos.");
        }

        // El minijuego arranca automáticamente, por eso se oculta el botón de Start.
        // El botón de salir (backButton) sí se muestra para que el jugador pueda abandonar.
        if (startButton != null) startButton.gameObject.SetActive(false);
        if (backButton != null)  backButton.gameObject.SetActive(true);
        if (exitConfirmPanel != null) exitConfirmPanel.SetActive(false);

        OnStartButtonPressed();
    }

    /// <summary>
    /// Ejecuta la lógica asociada a on start button pressed dentro de SimonGameManagerPajaro.
    /// </summary>
    public void OnStartButtonPressed()
    {
        if (gameStarted) return; // Evita doble click

        gameStarted = true;
        IsActive = true;

        if (startButton != null)
        {
            startButton.gameObject.SetActive(false);
        }

        StartCoroutine(StartNewRound());
    }

    /// <summary>
    /// Ejecuta la lógica asociada a start new round dentro de SimonGameManagerPajaro.
    /// </summary>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    IEnumerator StartNewRound()
    {
        isPlayerTurn = false;
        canPress = false; //
        playerInput.Clear();  

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < selectedBird.notasPorTurno; i++)
        {
            int newIndex = Random.Range(0, circles.Length);
            pattern.Add(newIndex);
        }

        level++;

        contadorText.enabled = true;
        contadorText.text = $"Round {level}";

        flashDuration = Mathf.Max(0.05f, flashDuration - 0.08f); // Aumenta la dificultad reduciendo el tiempo de flash
        timeBetweenFlashes = Mathf.Max(0.05f, timeBetweenFlashes - 0.06f); // Aumenta la dificultad reduciendo el tiempo entre flashes
                
        //flashDuration = Mathf.Clamp(flashDuration, 0.05f, 1f);
        //timeBetweenFlashes = Mathf.Clamp(timeBetweenFlashes, 0.05f, 1f);
            
        
        // Mostrar el patrón
        yield return StartCoroutine(PlayPattern());

        isPlayerTurn = true;
        canPress = true;
    }

    /// <summary>
    /// Reproduce o inicia pattern.
    /// </summary>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    IEnumerator PlayPattern()
    {
        canPress = false;

        // Apagar visualmente los círculos durante la secuencia: el jugador ve que
        // no son pulsables. El flash de cada nota vuelve por sí solo al color atenuado.
        foreach (var c in circles) c.SetIdleDimmed(true);

        foreach (int index in pattern)
        {
            PlaySound(index);
            ShowLetter(index); // QUITAR ESTO PARA PONERSELO AL PAJARO
            yield return StartCoroutine(circles[index].Flash(flashDuration));
            yield return new WaitForSeconds(timeBetweenFlashes);
        }

        // Devolver los círculos a su color normal para indicar que es el turno del jugador.
        foreach (var c in circles) c.SetIdleDimmed(false);

        canPress = true;
    }
    /// <summary>
    /// Ejecuta la lógica asociada a on circle pressed dentro de SimonGameManagerPajaro.
    /// </summary>
    /// <param name="index">Parámetro index empleado por el método.</param>
    public void OnCirclePressed(int index)
    {
        if (!isPlayerTurn || !canPress) return;

        canPress = false;
        StartCoroutine(HandlePlayerPress(index));
    }

    /// <summary>
    /// Muestra letter en la interfaz o en la escena.
    /// </summary>
    /// <param name="index">Parámetro index empleado por el método.</param>
    private void ShowLetter(int index)
    {
        if (index < letterSprites.Length)
        {
            LetterDisplayUI.Instance.ShowLetter(letterSprites[index]);
        }
    }

    /// <summary>
    /// Ejecuta la lógica asociada a handle player press dentro de SimonGameManagerPajaro.
    /// </summary>
    /// <param name="index">Parámetro index empleado por el método.</param>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    IEnumerator HandlePlayerPress(int index)
    {
        PlaySound(index);
        ShowLetter(index);
        StartCoroutine(circles[index].Flash(0.2f));

        playerInput.Add(index);

        int currentStep = playerInput.Count - 1;

        // Fallo
        if (playerInput[currentStep] != pattern[currentStep])
        {
            failCount++;
            Debug.Log($"Fallo {failCount}/{selectedBird.maxFallos}");

            if (failCount >= selectedBird.maxFallos)
            {
                EndGameFail();
                yield break;
            }

            yield return StartCoroutine(HandleFail());
            yield break;
        }

        // Si completó correctamente la secuencia
        if (playerInput.Count == pattern.Count)
        {
            Debug.Log($"Ronda completada: {level}");

            //if (levelIndicator != null)
            //{
            //    if (!hasFailedCurrentLevel)
            //    {
            //        levelIndicator.SetLevelSuccess(level - 1);
            //    }
            //    else
            //    {
            //        levelIndicator.SetLevelFail(level - 1);
            //    }
            //}

            //hasFailedCurrentLevel = false;
            yield return StartCoroutine(HandleSuccess());

            if (level >= selectedBird.rondasTotales)
            {
                CompleteMiniGame();
                yield break;
            }

            StartCoroutine(StartNewRound());
            yield break;
        }

        yield return new WaitForSeconds(0.2f);
        canPress = true;
    }

    /// <summary>
    /// Ejecuta la lógica asociada a handle success dentro de SimonGameManagerPajaro.
    /// </summary>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    IEnumerator HandleSuccess()
    {
        isPlayerTurn = false;
        canPress = false;

        for (int i = 0; i < circleFlashes; i++)
        {
            foreach (var circle in circles)
            {
                circle.SetColorInstant(successColor);
            }

            yield return new WaitForSeconds(0.2f);

            foreach (var circle in circles)
            {
                circle.RestoreOriginalColor();
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
    /// <summary>
    /// Ejecuta la lógica asociada a handle fail dentro de SimonGameManagerPajaro.
    /// </summary>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    IEnumerator HandleFail()
    {
        isPlayerTurn = false;
        canPress = false;
        playerInput.Clear();

        for (int i = 0; i < circleFlashes; i++)
        {
            foreach (var circle in circles)
            {
                circle.SetColorInstant(failColor);
            }
            yield return new WaitForSeconds(0.2f);

            foreach (var circle in circles)
            {
                circle.RestoreOriginalColor();
            }
            yield return new WaitForSeconds(0.2f);
        }

        // Repite el mismo patrón
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(PlayPattern());

        isPlayerTurn = true;
        canPress = true;
        //hasFailed = false;
    }

 
    /// <summary>
    /// Reproduce o inicia sound.
    /// </summary>
    /// <param name="circleIndex">Parámetro circle index empleado por el método.</param>
    private void PlaySound(int circleIndex)
    {
        if (audioSource != null && circleIndex < circleSounds.Length && circleSounds[circleIndex] != null)
        {
            audioSource.PlayOneShot(circleSounds[circleIndex]);
        }
    }

    /// <summary>
    /// Ejecuta la lógica asociada a complete mini game dentro de SimonGameManagerPajaro.
    /// </summary>
    private void CompleteMiniGame()
    {
        StartCoroutine(CompleteRoutine());
    }

    /// <summary>
    /// Ejecuta la lógica asociada a complete routine dentro de SimonGameManagerPajaro.
    /// </summary>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    IEnumerator CompleteRoutine()
    {
        Debug.Log("MINIJUEGO COMPLETADO");

        // Eliminar pájaro del mapa y reproducir animación de nubes
        TileAStar tileAstar = GameObject.FindGameObjectWithTag("Player")?.GetComponent<TileAStar>();
        if (tileAstar != null) tileAstar.RemoveBirdAtLastNode();

        if (selectedBird != null)
        {
            selectedBird = null;
            ScenePersistentManager.instance.interactedBird = null;
        }

        int energiaGanada = level;
        LetterDisplayUI.Instance.ShowEnergy(energiaGanada);

        yield return new WaitForSeconds(2f);

        IsActive = false;
        Destroy(transform.root.gameObject);
    }

    /// <summary>
    /// Ejecuta la lógica asociada a end game fail dentro de SimonGameManagerPajaro.
    /// </summary>
    private void EndGameFail()
    {
        StartCoroutine(FailRoutine());
    }

    /// <summary>
    /// Ejecuta la lógica asociada a fail routine dentro de SimonGameManagerPajaro.
    /// </summary>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    IEnumerator FailRoutine()
    {
        Debug.Log("MINIJUEGO FALLADO");

        // Desactivar interacción del pájaro (ya no es interactuable)
        TileAStar tileAstar = GameObject.FindGameObjectWithTag("Player")?.GetComponent<TileAStar>();
        if (tileAstar != null) tileAstar.DisableBirdInteraction();

        int energiaGanada = level - 1;
        LetterDisplayUI.Instance.ShowEnergy(energiaGanada);

        yield return new WaitForSeconds(2f);

        IsActive = false;
        Destroy(transform.root.gameObject);
    }
    /// <summary>
    /// Ejecuta la lógica asociada a can player press dentro de SimonGameManagerPajaro.
    /// </summary>
    /// <returns>true si la operación se ha completado correctamente; false en caso contrario.</returns>
    public bool CanPlayerPress()
    {
        return isPlayerTurn && canPress;
    }

    // -------- Salida con confirmación --------

    /// <summary>Botón de salir del minijuego. Muestra el panel de confirmación.</summary>
    public void OnBackButtonPressed()
    {
        if (exitConfirmPanel != null)
        {
            exitConfirmPanel.SetActive(true);
            return;
        }

        // Sin panel, sale directamente (no debería ocurrir si está bien configurado en el Inspector).
        ConfirmExit();
    }

    /// <summary>Botón "Sí" del panel de confirmación: aborta el minijuego sin penalizar al pájaro.</summary>
    public void ConfirmExit()
    {
        if (exitConfirmPanel != null) exitConfirmPanel.SetActive(false);

        // Bloquear el input del mapa durante un instante para evitar que el clic
        // sobre el botón "Sí" se propague al tilemap (mismo frame, LateUpdate de
        // IsometricCamera) y mueva al jugador a la casilla bajo el botón.
        TileAStar.BlockInputForSeconds(0.3f);

        // Detener cualquier corrutina en curso (secuencia, flashes, esperas).
        StopAllCoroutines();

        // No se marca el pájaro como fallado: el jugador podrá volver a intentarlo.
        selectedBird = null;
        if (ScenePersistentManager.instance != null)
        {
            ScenePersistentManager.instance.interactedBird = null;
        }

        IsActive = false;
        Destroy(transform.root.gameObject);
    }

    /// <summary>Botón "No" del panel de confirmación: cierra el panel y deja seguir la partida.</summary>
    public void CancelExit()
    {
        if (exitConfirmPanel != null) exitConfirmPanel.SetActive(false);
    }
}
