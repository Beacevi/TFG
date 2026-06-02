/**
 * @file SimonGameManager.cs
 * @brief Controla la lógica principal del minijuego de memoria musical tipo Simón dice.
 * @author Hortensia Studio - David Díaz Espinosa de los Monteros
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Controla la lógica principal del minijuego de memoria musical tipo Simón dice.
/// </summary>
public class SimonGameManager : MonoBehaviour
{
    /// <summary>
    /// Propiedad que expone o modifica instance.
    /// </summary>
    public static SimonGameManager Instance { get; private set; }

    /// <summary>
    /// Colección de circles utilizada por este componente.
    /// </summary>
    public CircleButton[] circles;
    /// <summary>
    /// Colección de circle sounds utilizada por este componente.
    /// </summary>
    public AudioClip[] circleSounds;
    /// <summary>
    /// Colección de letter sprites utilizada por este componente.
    /// </summary>
    public Sprite[] letterSprites;

    /// <summary>
    /// Canvas de interfaz asociado al sistema.
    /// </summary>
    public Canvas canvas;
    /// <summary>
    /// Referencia de interfaz utilizada para mostrar o actualizar contador text.
    /// </summary>
    public TextMeshProUGUI contadorText;
    /// <summary>
    /// Referencia de interfaz utilizada para mostrar u ocultar el container de contador text.
    /// </summary>
    public GameObject contadorContainer;
    /// <summary>
    /// Botón de interfaz asociado a start button.
    /// </summary>
    public Button startButton, backButton;

    /// <summary>
    /// Panel de interfaz asociado a exit confirm panel.
    /// </summary>
    [Header("Confirmación de salida")]
    [Tooltip("Panel con los botones Sí/No que pide confirmación al pulsar el botón Atrás.")]
    [SerializeField] private GameObject exitConfirmPanel;

    /// <summary>
    /// Campo de tipo ChangeScene utilizado para almacenar o configurar change scene.
    /// </summary>
    [Tooltip("ChangeScene al que llamar para cambiar de escena al confirmar la salida. Si se deja vacío, se busca por tag SceneChanger.")]
    [SerializeField] private ChangeScene changeScene;
    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar exit scene.
    /// </summary>
    [Tooltip("Nombre de la escena a la que volver al confirmar la salida del minijuego.")]
    [SerializeField] private string exitScene = "UI";

    /// <summary>
    /// Campo de tipo Image utilizado para almacenar o configurar globo.
    /// </summary>
    public Image globo;

    /// <summary>
    /// Tiempo o duración asociado a flash duration.
    /// </summary>
    public float flashDuration = 1f;
    /// <summary>
    /// Tiempo o duración asociado a time between flashes.
    /// </summary>
    public float timeBetweenFlashes = 1f;

    /// <summary>
    /// Valor numérico que limita o define max fails.
    /// </summary>
    public int maxFails = 3;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar circle flashes.
    /// </summary>
    public int circleFlashes = 2;

    /// <summary>
    /// Color utilizado para representar fail color.
    /// </summary>
    public Color failColor = Color.red;
    /// <summary>
    /// Color utilizado para representar success color.
    /// </summary>
    public Color successColor = Color.green;

    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar notes per round.
    /// </summary>
    public int notesPerRound = 3;

    /// <summary>
    /// Fuente de audio utilizada para reproducir sonidos del componente.
    /// </summary>
    private AudioSource audioSource;

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
    /// Campo de tipo int utilizado para almacenar o configurar level.
    /// </summary>
    private int level = 0;
    /// <summary>
    /// Valor numérico que limita o define fail count.
    /// </summary>
    private int failCount = 0;

    /// <summary>
    /// Valor numérico que limita o define max stored games.
    /// </summary>
    private const int MaxStoredGames = 5;

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

        StartCoroutine(SetActiveObjects());

    }

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    void Start()
    {
        isPlayerTurn = false;
        canPress = false;
       
    }

    /// <summary>
    /// Ejecuta la lógica asociada a on start button pressed dentro de SimonGameManager.
    /// </summary>
    public void OnStartButtonPressed()
    {
        if (gameStarted) return;

        gameStarted = true;

        if (startButton != null)
        {
            contadorContainer.SetActive(true);
            startButton.gameObject.SetActive(false);
        }

        // Calcular ronda inicial
        level = CalculateStartingRound();

        Debug.Log("Ronda inicial calculada: " + level);

        // Generar patrón previo
        GenerateInitialPattern();

        StartCoroutine(StartNewRound());
    }

    /// <summary>
    /// Ejecuta la lógica asociada a on back button pressed dentro de SimonGameManager.
    /// </summary>
    public void OnBackButtonPressed()
    {
        // Si hay panel de confirmación, lo mostramos y esperamos a Sí/No.
        // Si no, conservamos el comportamiento previo (salida directa).
        if (exitConfirmPanel != null)
        {
            exitConfirmPanel.SetActive(true);
            return;
        }

        ConfirmExit();
    }

    /// <summary>Botón "Sí" del panel de confirmación de salida.</summary>
    public void ConfirmExit()
    {
        if (exitConfirmPanel != null) exitConfirmPanel.SetActive(false);
        StopAllCoroutines();

        // Resolver ChangeScene si no está asignado: lo buscamos por tag.
        if (changeScene == null)
        {
            var go = GameObject.FindGameObjectWithTag("SceneChanger");
            if (go != null) changeScene = go.GetComponent<ChangeScene>();
        }

        if (changeScene != null)
        {
            changeScene.Cambiar_A_Escena(exitScene);
        }
        else
        {
            // Sin ChangeScene disponible, conservamos el comportamiento anterior
            // (ocultar el canvas) para no dejar al jugador colgado.
            Debug.LogWarning("[SimonGameManager] No hay ChangeScene asignado ni encontrado por tag; se oculta el canvas como fallback.");
            StartCoroutine(wait());
        }
    }

    /// <summary>Botón "No" del panel de confirmación de salida.</summary>
    public void CancelExit()
    {
        if (exitConfirmPanel != null) exitConfirmPanel.SetActive(false);
    }
    /// <summary>
    /// Establece o actualiza active objects dentro del sistema.
    /// </summary>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    IEnumerator SetActiveObjects()
    {
       

        yield return new WaitForSeconds(1.263f);

        //if (startButton != null && backButton != null)
        //{
        //    startButton.gameObject.SetActive(true);
        //    backButton.gameObject.SetActive(true);
        //}
        //if (globo != null)
        //{
        //    globo.gameObject.SetActive(true);
        //}

        //for (int i = 0; i < circles.Length; i++)
        //{
        //    if (circles[i] != null)
        //    {
        //        circles[i].gameObject.SetActive(true);
        //    }
        //}
        canvas.gameObject.SetActive(true);

    }
    /// <summary>
    /// Ejecuta la lógica asociada a wait dentro de SimonGameManager.
    /// </summary>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    IEnumerator wait() 
    {
        yield return new WaitForSeconds(0.47f);
        canvas.gameObject.SetActive(false);
    }
    /// <summary>
    /// Ejecuta la lógica asociada a start new round dentro de SimonGameManager.
    /// </summary>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    IEnumerator StartNewRound()
    {
        isPlayerTurn = false;
        canPress = false;

        playerInput.Clear();

        yield return new WaitForSeconds(1f);

        // Añadir nuevas notas
        for (int i = 0; i < notesPerRound; i++)
        {
            int newIndex = Random.Range(0, circles.Length);
            pattern.Add(newIndex);
        }

        level++;

        contadorText.enabled = true;
        contadorText.text = $"Round {level}";

        // Incrementar dificultad
        flashDuration = Mathf.Max(0.05f, flashDuration - 0.08f);

        timeBetweenFlashes = Mathf.Max(0.05f, timeBetweenFlashes - 0.06f);

        // Mostrar patrón
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

            ShowLetter(index);

            yield return StartCoroutine(circles[index].Flash(flashDuration));

            yield return new WaitForSeconds(timeBetweenFlashes);
        }

        // Devolver los círculos a su color normal para indicar que es el turno del jugador.
        foreach (var c in circles) c.SetIdleDimmed(false);

        canPress = true;
    }

    /// <summary>
    /// Ejecuta la lógica asociada a on circle pressed dentro de SimonGameManager.
    /// </summary>
    /// <param name="index">Parámetro index empleado por el método.</param>
    public void OnCirclePressed(int index)
    {
        if (!isPlayerTurn || !canPress) return;

        canPress = false;

        StartCoroutine(HandlePlayerPress(index));
    }

    /// <summary>
    /// Ejecuta la lógica asociada a handle player press dentro de SimonGameManager.
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

        if (playerInput[currentStep] != pattern[currentStep])
        {
            failCount++;

            Debug.Log($"Fallo {failCount}/{maxFails}");

            if (failCount >= maxFails)
            {
                EndGameFail();
                yield break;
            }

            yield return StartCoroutine(HandleFail());

            yield break;
        }

        if (playerInput.Count == pattern.Count)
        {
            Debug.Log($"Ronda completada: {level}");

            yield return StartCoroutine(HandleSuccess());

            StartCoroutine(StartNewRound());

            yield break;
        }

        yield return new WaitForSeconds(0.2f);

        canPress = true;
    }

    /// <summary>
    /// Ejecuta la lógica asociada a handle success dentro de SimonGameManager.
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
    /// Ejecuta la lógica asociada a handle fail dentro de SimonGameManager.
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

        // Repetir patrón
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(PlayPattern());

        isPlayerTurn = true;
        canPress = true;
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
    /// Reproduce o inicia sound.
    /// </summary>
    /// <param name="circleIndex">Parámetro circle index empleado por el método.</param>
    private void PlaySound(int circleIndex)
    {
        if (audioSource != null &&
            circleIndex < circleSounds.Length &&
            circleSounds[circleIndex] != null)
        {
            audioSource.PlayOneShot(circleSounds[circleIndex]);
        }
    }


    /// <summary>
    /// Ejecuta la lógica asociada a end game fail dentro de SimonGameManager.
    /// </summary>
    private void EndGameFail()
    {
        StartCoroutine(FailRoutine());
    }

    /// <summary>
    /// Ejecuta la lógica asociada a fail routine dentro de SimonGameManager.
    /// </summary>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    IEnumerator FailRoutine()
    {
        Debug.Log("FIN MINIJUEGO ENTRENAMIENTO");

        int energiaGanada = level - 1;

        // Guardar score
        SaveGameResult(energiaGanada);

        // Mostrar energía
        LetterDisplayUI.Instance.ShowEnergy(energiaGanada);

        yield return new WaitForSeconds(2f);
    }


    //Guardar partidas
    /// <summary>
    /// Guarda el estado actual para que pueda recuperarse posteriormente.
    /// </summary>
    /// <param name="roundReached">Parámetro round reached empleado por el método.</param>
    private void SaveGameResult(int roundReached)
    {
        List<int> scores = LoadRecentGames();

        scores.Add(roundReached);

        // Mantener solo últimas 5
        while (scores.Count > MaxStoredGames)
        {
            scores.RemoveAt(0);
        }

        string saveData = string.Join(",", scores);

        PlayerPrefs.SetString("RecentSimonGames", saveData);

        PlayerPrefs.Save();

        Debug.Log("Partidas guardadas: " + saveData);
    }

    /// <summary>
    /// Carga el estado previamente guardado y restaura los datos del sistema.
    /// </summary>
    /// <returns>Instancia o valor de tipo List<int> resultante de la operación.</returns>
    private List<int> LoadRecentGames()
    {
        List<int> scores = new List<int>();

        string saveData = PlayerPrefs.GetString("RecentSimonGames", "");

        if (string.IsNullOrEmpty(saveData))
            return scores;

        string[] split = saveData.Split(',');

        foreach (string value in split)
        {
            if (int.TryParse(value, out int result))
            {
                scores.Add(result);
            }
        }

        return scores;
    }

    /// <summary>
    /// Calcula starting round a partir de los datos disponibles.
    /// </summary>
    /// <returns>Valor numérico calculado o consultado por el método.</returns>
    private int CalculateStartingRound()
    {
        List<int> scores = LoadRecentGames();

        if (scores.Count == 0)
            return 0;

        float total = 0;

        foreach (int score in scores)
        {
            total += score;
        }

        float average = total / scores.Count;

        // Reducir 20%
        float reduced = average * 0.8f;

        return Mathf.FloorToInt(reduced);
    }

    /// <summary>
    /// Genera initial pattern utilizando los parámetros configurados.
    /// </summary>
    private void GenerateInitialPattern()
    {
        pattern.Clear();

        for (int round = 0; round < level; round++)
        {
            for (int i = 0; i < notesPerRound; i++)
            {
                int newIndex = Random.Range(0, circles.Length);

                pattern.Add(newIndex);
            }
        }
    }

    /// <summary>
    /// Ejecuta la lógica asociada a can player press dentro de SimonGameManager.
    /// </summary>
    /// <returns>true si la operación se ha completado correctamente; false en caso contrario.</returns>
    public bool CanPlayerPress()
    {
        return isPlayerTurn && canPress;
    }
}
