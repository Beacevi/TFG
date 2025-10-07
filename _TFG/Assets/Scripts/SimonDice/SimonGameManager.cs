using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimonGameManager : MonoBehaviour
{
    public static SimonGameManager Instance { get; private set; }

    public CircleButton[] circles;
    public float flashDuration = 0.5f;
    public float timeBetweenFlashes = 0.3f;

    private List<int> pattern = new List<int>();
    private List<int> playerInput = new List<int>();
    private bool isPlayerTurn = false;

    private int level = 0;
    private bool hasFailed = false; 

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(StartNewRound());
    }

    IEnumerator StartNewRound()
    {
        isPlayerTurn = false;
        playerInput.Clear();

        yield return new WaitForSeconds(1f);

        // Si no hay fallo, se añade un nuevo paso al patrón
        if (!hasFailed)
        {
            int newIndex = Random.Range(0, circles.Length);
            pattern.Add(newIndex);
            level++;
            Debug.Log($"Nivel {level}");
        }
        else
        {
            Debug.Log("Repitiendo patrón");
            hasFailed = false;
        }

        // Mostrar patrón completo
        foreach (int index in pattern)
        {
            yield return StartCoroutine(circles[index].Flash(flashDuration));
            yield return new WaitForSeconds(timeBetweenFlashes);
        }

        isPlayerTurn = true;
    }
    IEnumerator RestartSamePattern()
    {
        isPlayerTurn = false;
        playerInput.Clear();

        // 
        yield return new WaitForSeconds(1f);

        // Repite misma secuencia
        foreach (int index in pattern)
        {
            yield return StartCoroutine(circles[index].Flash(flashDuration));
            yield return new WaitForSeconds(timeBetweenFlashes);
        }

        isPlayerTurn = true;
        hasFailed = false;
    }
    public void OnCirclePressed(int index)
    {
        if (!isPlayerTurn) return;

        playerInput.Add(index);
        int currentStep = playerInput.Count - 1;

        // Verificar jugada actual
        if (playerInput[currentStep] != pattern[currentStep])
        {
            Debug.Log("ERES UN MAULA");
            hasFailed = true;
            StartCoroutine(RestartSamePattern());
            return;
        }

        // Si completó correctamente la secuencia
        if (playerInput.Count == pattern.Count)
        {
            Debug.Log("OLEEEEE");
            StartCoroutine(StartNewRound());
        }
    }
}


    
