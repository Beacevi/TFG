using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class SimonGameManager : MonoBehaviour
{
    public static SimonGameManager Instance { get; private set; }

    public CircleButton[] circles;
    public AudioClip[] circleSounds;
    public Sprite[] letterSprites;

    public TextMeshProUGUI contadorText;
    public Button startButton;

    public float flashDuration = 1f;
    public float timeBetweenFlashes = 1f;

    public int maxFails = 3;
    public int circleFlashes = 2;

    public Color failColor = Color.red;
    public Color successColor = Color.green;

    public int notesPerRound = 3;

    private AudioSource audioSource;

    private List<int> pattern = new List<int>();
    private List<int> playerInput = new List<int>();

    private bool isPlayerTurn = false;
    private bool canPress = false;
    private bool gameStarted = false;

    private int level = 0;
    private int failCount = 0;

    private const int MaxStoredGames = 5;

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

    void Start()
    {
        isPlayerTurn = false;
        canPress = false;

        if (startButton != null)
        {
            startButton.gameObject.SetActive(true);
        }
    }

    public void OnStartButtonPressed()
    {
        if (gameStarted) return;

        gameStarted = true;

        if (startButton != null)
        {
            startButton.gameObject.SetActive(false);
        }

        // Calcular ronda inicial
        level = CalculateStartingRound();

        Debug.Log("Ronda inicial calculada: " + level);

        // Generar patrón previo
        GenerateInitialPattern();

        StartCoroutine(StartNewRound());
    }

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

    IEnumerator PlayPattern()
    {
        canPress = false;

        foreach (int index in pattern)
        {
            PlaySound(index);

            ShowLetter(index);

            yield return StartCoroutine(circles[index].Flash(flashDuration));

            yield return new WaitForSeconds(timeBetweenFlashes);
        }

        canPress = true;
    }

    public void OnCirclePressed(int index)
    {
        if (!isPlayerTurn || !canPress) return;

        canPress = false;

        StartCoroutine(HandlePlayerPress(index));
    }

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


    private void ShowLetter(int index)
    {
        if (index < letterSprites.Length)
        {
            LetterDisplayUI.Instance.ShowLetter(letterSprites[index]);
        }
    }

    private void PlaySound(int circleIndex)
    {
        if (audioSource != null &&
            circleIndex < circleSounds.Length &&
            circleSounds[circleIndex] != null)
        {
            audioSource.PlayOneShot(circleSounds[circleIndex]);
        }
    }


    private void EndGameFail()
    {
        StartCoroutine(FailRoutine());
    }

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

    public bool CanPlayerPress()
    {
        return isPlayerTurn && canPress;
    }
}