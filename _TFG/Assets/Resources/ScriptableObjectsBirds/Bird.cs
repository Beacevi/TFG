/**
 * @file Bird.cs
 * @brief Características de cada pájaro, incluyendo su prefab, nombre, comportamiento de movimiento y parámetros relacionados con el minijuego.
 * @author Hortensia Studio - David Díaz Espinosa de los Monteros
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */
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


    [Header("Idle Movement (Ambient)")]
    public BirdIdleMovementType idleMovementType;

    public float idleAmplitude = 0.5f;
    public float idleSpeed = 1f;

    public float idleShakeIntensity = 0.2f;

    public float idleCircleRadius = 1f;
    public float idleCircleSpeed = 1f;

    public float idleHoverOffset = 0.2f;

    public float spawnChance;
}
public enum BirdMovementType
{
    Random,
    Jump,
    Shake,
    Circle
}

public enum BirdIdleMovementType
{
    Float,
    Hover,
    Shake,
    Circle,
    Sway,
    FreeRoam
}


