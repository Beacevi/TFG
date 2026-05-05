using UnityEngine;

[CreateAssetMenu(fileName = "Bird", menuName = "Scriptable Objects/Bird")]
public class Bird : ScriptableObject
{
    public bool obtenido = false;

    [Header("Visual")]
    public GameObject birdPrefab;
    public string birdName;

    [Header("Gameplay")]
    public int rondasTotales;      // Cuántas rondas dura el minijuego
    public int notasPorTurno;      // Cuántas notas se añaden cada ronda (1, 2 o 4)

    public int maxFallos = 3;      // Cuántos fallos antes de perder
}
