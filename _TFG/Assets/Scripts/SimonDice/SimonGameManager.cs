using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimonGameManager : MonoBehaviour
{
    //Singleton
    public static SimonGameManager Instance { get; private set; }

    [Header("Círculos del juego")]
    public CircleButton[] circles; // Asigna en el inspector

    [Header("Parámetros del juego")]
    public float flashDuration = 0.5f;
    public float timeBetweenFlashes = 0.3f;

    private List<int> pattern = new List<int>();
    private List<int> playerInput = new List<int>();
    private bool isPlayerTurn = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartCoroutine(StartNewRound());
    }

    IEnumerator StartNewRound()
    {
        isPlayerTurn = false;
        playerInput.Clear();

        // Añadir un nuevo número al patrón
        int newIndex = Random.Range(0, circles.Length);
        pattern.Add(newIndex);

        yield return new WaitForSeconds(1f);

        // Reproducir patrón
        foreach (int index in pattern)
        {
            yield return StartCoroutine(circles[index].Flash(flashDuration));
            yield return new WaitForSeconds(timeBetweenFlashes);
        }

        isPlayerTurn = true;
    }

    public void OnCirclePressed(int index)
    {
        if (!isPlayerTurn) return;

        playerInput.Add(index);
        int currentStep = playerInput.Count - 1;

        if (playerInput[currentStep] != pattern[currentStep])
        {
            Debug.Log("ERES UN MAULA");
            pattern.Clear();
            StartCoroutine(StartNewRound());
            return;
        }

        if (playerInput.Count == pattern.Count)
        {
            Debug.Log("OLEEEE");
            StartCoroutine(StartNewRound());
        }
    }
}
