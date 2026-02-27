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

    public Color failColor = Color.red; // Color del flash de fallo
    public int failFlashes = 2;         // Cuántas veces parpadea

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

        if (ScenePersistentManager.instance.interactedBird != null)
        {
            selectedBird = ScenePersistentManager.instance.interactedBird;
        }
        

        isPlayerTurn = false;
        canPress = false;

        SpriteRenderer sr = selectedBird.birdPrefab.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
             pajaro.sprite = sr.sprite;  // Aquí está el sprite que ves en la escena
            Debug.Log("Sprite actual: " + pajaro.name);
        }
        else
        {
            Debug.LogWarning("No hay SpriteRenderer en este objeto.");
        }


        if (startButton != null)
        {
            startButton.gameObject.SetActive(true);       
        }

    }

    IEnumerator StartNewRound()
    {
        isPlayerTurn = false;
        playerInput.Clear();  
        yield return new WaitForSeconds(1f);

        // Solo añadir nuevo paso si no hubo fallo
        if (!hasFailed)
        {
            int newIndex = Random.Range(0, circles.Length);
            pattern.Add(newIndex);
            level++;
            //Debug.Log($"Nivel {level}");
            contadorText.enabled = true;
            contadorText.text = $"Level {level}";

            flashDuration = Mathf.Max(0.05f, flashDuration - 0.08f); // Aumenta la dificultad reduciendo el tiempo de flash
            timeBetweenFlashes = Mathf.Max(0.05f, timeBetweenFlashes - 0.06f); // Aumenta la dificultad reduciendo el tiempo entre flashes
                
            //flashDuration = Mathf.Clamp(flashDuration, 0.05f, 1f);
            //timeBetweenFlashes = Mathf.Clamp(timeBetweenFlashes, 0.05f, 1f);
            
        }
        else
        {
            Debug.Log("Repitiendo patrón");
            hasFailed = false;
        }

        
        // Mostrar el patrón
        yield return StartCoroutine(PlayPattern());

        isPlayerTurn = true;
        canPress = true;
    }

    IEnumerator HandleFail()
    {
        isPlayerTurn = false;
        canPress = false;
        playerInput.Clear();

        for (int i = 0; i < failFlashes; i++)
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

        if (levelIndicator != null)
        {
            levelIndicator.SetLevelFail(level - 1);
        }

        // Repite el mismo patrón
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(PlayPattern());

        isPlayerTurn = true;
        canPress = true;
        hasFailed = false;
    }


    IEnumerator PlayPattern()
    {
        canPress = false;

        foreach (int index in pattern)
        {
            PlaySound(index);
            yield return StartCoroutine(circles[index].Flash(flashDuration));
            
            yield return new WaitForSeconds(timeBetweenFlashes);
        }

        canPress = true;
    }

    IEnumerator HandlePlayerPress(int index)
    {
        PlaySound(index);
        StartCoroutine(circles[index].Flash(0.2f));

        playerInput.Add(index);
        int currentStep = playerInput.Count - 1;

        if (playerInput[currentStep] != pattern[currentStep])
        {
            Debug.Log("Eres un maula");
            hasFailed = true;
            hasFailedCurrentLevel = true;
            StartCoroutine(HandleFail());
            yield break;
        }
      
        // Si completó correctamente la secuencia
        if (playerInput.Count == pattern.Count)
        {
            Debug.Log("El nivel que llevas es: " + level);
            Debug.Log("Las rondas necesarias son: " + selectedBird.rondasNecesarias);
            Debug.Log("Oleeeeeeeeeee");
            if (levelIndicator != null)
            {
                if (!hasFailedCurrentLevel)
                {
                    levelIndicator.SetLevelSuccess(level - 1);
                }
                else
                {
                    levelIndicator.SetLevelFail(level - 1);
                }
            }

            hasFailedCurrentLevel = false;

            

            if (level >= selectedBird.rondasNecesarias)//
            {
                CompleteMiniGame();
                yield break;
            }

            yield return new WaitForSeconds(0.3f);

            

            StartCoroutine(StartNewRound());
            yield break;
        }

        // Delay
        yield return new WaitForSeconds(0.2f);
        canPress = true;
    }

    public void OnCirclePressed(int index)
    {
        if (!isPlayerTurn || !canPress) return;

        canPress = false;
        StartCoroutine(HandlePlayerPress(index));     
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

        if (selectedBird != null)
        {
            selectedBird.obtenido = true;

            selectedBird = null;
            ScenePersistentManager.instance.interactedBird = null;

            Debug.Log("Pájaro marcado como conseguido");
        }
        

        // Cerrar escena aditiva
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("SimonSaysPajaro");
    }


    public bool CanPlayerPress()
    {
        return isPlayerTurn && canPress;
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


}



