using UnityEngine;

[CreateAssetMenu(fileName = "Bird", menuName = "Scriptable Objects/Bird")]
public class Bird : ScriptableObject
{
    public bool obtenido = false;

    [Header("Visual")]
    public GameObject birdPrefab;
    public string birdName;

    [Header("Gameplay")]
    public int rondasTotales;      // Cuantas rondas dura el minijuego
    public int notasPorTurno;      // Cuantas notas se añaden cada ronda (1, 2 o 4)

    public int maxFallos = 3;      // Cuantos fallos antes de perder

    [Header("Movement")]
    public BirdMovementType movementType;
    public float moveRange = 1f;
    public float moveDuration = 0.5f;
}
public enum BirdMovementType
{
    Random,
    Jump,
    Shake,
    Circle
}
