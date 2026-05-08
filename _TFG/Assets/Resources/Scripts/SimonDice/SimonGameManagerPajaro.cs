using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Rendering;

public class SimonGameManagerPajaro : MonoBehaviour
{
    public static SimonGameManagerPajaro Instance { get; private set; }

    public CircleButtonPajaro[] circles;
    public LevelIndicator levelIndicator;
    public AudioClip[] circleSounds;
    public Sprite[] letterSprites;
    private AudioSource audioSource;
    public TextMeshProUGUI contadorText;

    public float flashDuration = 1f;
    public float timeBetweenFlashes = 1f;

    private List<int> pattern = new List<int>();
    private List<int> playerInput = new List<int>();

    private bool isPlayerTurn = false;
    private bool hasFailed = false;
    private bool canPress = false;
    private bool hasFailedCurrentLevel = false;
    private bool gameStarted = false;

    public Button startButton;

    private int level = 0;
    private int failCount = 0;
    public int maxFails = 3;

    public Color failColor = Color.red; // Color flash fallo
    public Color successColor = Color.green; // Color flash acierto
    public int circleFlashes = 2;         // Cuántas veces parpadea

    public Image pajaro;
    private Bird selectedBird;
    

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
        // Obtener pájaro seleccionado
        /* COMENTADO HASTA QUE HAYA PAJARO
         if (ScenePersistentManager.instance.interactedBird != null)
         {
             selectedBird = ScenePersistentManager.instance.interactedBird;
         }
         */

        isPlayerTurn = false;
        canPress = false;

       /* SpriteRenderer sr = selectedBird.birdPrefab.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            pajaro.sprite = sr.sprite;  // Aquí está el sprite que ves en la escena
            Debug.Log("Sprite actual: " + pajaro.name);
        }
        else
        {
            Debug.LogWarning("No hay SpriteRenderer en este objeto.");
        }
       */

        if (startButton != null)
        {
            startButton.gameObject.SetActive(true);       
        }

    }

    public void OnStartButtonPressed()
    {
        if (gameStarted) return; // Evita doble click

        gameStarted = true;

        if (startButton != null)
        {
            startButton.gameObject.SetActive(false);
        }

        StartCoroutine(StartNewRound());
    }

    IEnumerator StartNewRound()
    {
        isPlayerTurn = false;
        canPress = false; //
        playerInput.Clear();  

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 4 /*i < selectedBird.notasPorTurno*/; i++)
        {
            int newIndex = Random.Range(0, circles.Length);
            pattern.Add(newIndex);
        }

        level++;

        //Debug.Log($"Nivel {level}");
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

    IEnumerator PlayPattern()
    {
        canPress = false;

        foreach (int index in pattern)
        {
            PlaySound(index);
            ShowLetter(index); // QUITAR ESTO PARA PONERSELO AL PAJARO
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

    private void ShowLetter(int index)
    {
        if (index < letterSprites.Length)
        {
            LetterDisplayUI.Instance.ShowLetter(letterSprites[index]);
        }
    }

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
            Debug.Log($"Fallo {failCount}/{3/*selectedBird.maxFallos*/}");

            if (failCount >= 3/*selectedBird.maxFallos*/)
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

            if (level >= 2/*selectedBird.rondasTotales*/)
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

        //if (levelIndicator != null)
        //{
        //    levelIndicator.SetLevelFail(level - 1);
        //}

        //failCount++;

        //if (failCount >= maxFails)
        //{
        //    EndGameFail();
        //    yield break;
        //}

        // Repite el mismo patrón
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(PlayPattern());

        isPlayerTurn = true;
        canPress = true;
        //hasFailed = false;
    }

 
    private void PlaySound(int circleIndex)
    {
        if (audioSource != null && circleIndex < circleSounds.Length && circleSounds[circleIndex] != null)
        {
            audioSource.PlayOneShot(circleSounds[circleIndex]);
        }
    }

    private void CompleteMiniGame()
    {
        Debug.Log("MINIJUEGO COMPLETADO");
        /* COMENTADO HASTA QUE HAYA PAJARO
        if (selectedBird != null)
        {
            selectedBird.obtenido = true;

            selectedBird = null;
            ScenePersistentManager.instance.interactedBird = null;

            Debug.Log("Pájaro marcado como conseguido");
        }*/

        //int energiaGanada = level;
        //Debug.Log($"Energía ganada: {energiaGanada}");

        // Cerrar escena aditiva
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("SimonSaysPajaro");//Aqui se cierra la escena
    }

    private void EndGameFail()
    {
        Debug.Log("MINIJUEGO FALLADO");

        //int energiaGanada = level;
        //Debug.Log($"Energía ganada: {energiaGanada}");

        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("SimonSaysPajaro");
    }

    public bool CanPlayerPress()
    {
        return isPlayerTurn && canPress;
    }
}