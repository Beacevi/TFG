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
    private bool hasFailed = false;
    private int level = 0;

    public Color failColor = Color.red; // Color del flash de fallo
    public int failFlashes = 2;         // Cuántas veces parpadea
    private bool canPress = false;

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

        // Solo añadir nuevo paso si no hubo fallo
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

        // Mostrar el patrón
        yield return StartCoroutine(PlayPattern());

        isPlayerTurn = true;
    }

    IEnumerator HandleFail()
    {
        isPlayerTurn = false;
        playerInput.Clear();

        for (int i = 0; i < failFlashes; i++)
        {
            foreach (var circle in circles)
                circle.SetColorInstant(failColor);
            yield return new WaitForSeconds(0.2f);

            foreach (var circle in circles)
                circle.RestoreOriginalColor();
            yield return new WaitForSeconds(0.2f);
        }

        //Repite el mismo patrón
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(PlayPattern());

        isPlayerTurn = true;
        hasFailed = false;
    }

    IEnumerator PlayPattern()
    {
        canPress = false;

        foreach (int index in pattern)
        {
            yield return StartCoroutine(circles[index].Flash(flashDuration));
            yield return new WaitForSeconds(timeBetweenFlashes);
        }

        canPress = true;
    }

    IEnumerator HandlePlayerPress(int index)
    {
        playerInput.Add(index);
        int currentStep = playerInput.Count - 1;

        if (playerInput[currentStep] != pattern[currentStep])
        {
            Debug.Log("ERES UN MAULA");
            hasFailed = true;
            StartCoroutine(HandleFail());
            yield break;
        }

        // Si completó correctamente la secuencia
        if (playerInput.Count == pattern.Count)
        {
            Debug.Log("OLEEEEEEE");
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

    // Método para verificar si el jugador puede presionar
    public bool CanPlayerPress()
    {
        return isPlayerTurn && canPress;
    }


}



