/**
 * @file KPIsManager.cs
 * @brief Registra y consulta indicadores de progreso o comportamiento del jugador utilizados como métricas internas del proyecto.
 * @author Hortensia Studio - Alejandro Romero Burgada
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;

/// <summary>
/// Registra y consulta indicadores de progreso o comportamiento del jugador utilizados como métricas internas del proyecto.
/// </summary>
public class KPIsManager : MonoBehaviour
{
    /// <summary>
    /// Instancia singleton utilizada para acceder globalmente al controlador.
    /// </summary>
    public static KPIsManager Instance;

    /// <summary>
    /// Tiempo o duración asociado a sesion timer.
    /// </summary>
    public float SesionTimer; //Lo que lleva jugado en UNA sesión
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar tiempo juego.
    /// </summary>
    public float tiempoJuego; //Lo que lleva jugado durante TODAS las sesiones
    /// <summary>
    /// Valor numérico que limita o define minijuego timer.
    /// </summary>
    public float MinijuegoTimer;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar contador sesiones.
    /// </summary>
    public int contadorSesiones = 0;
    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar fecha actual.
    /// </summary>
    public string fechaActual;

    /// <summary>
    /// Tiempo o duración asociado a sesion start time.
    /// </summary>
    float sesionStartTime;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /// <summary>
    /// Inicializa referencias internas antes de que comience la ejecución normal del componente.
    /// </summary>
    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Nueva sesión
        contadorSesiones++;
        fechaActual = System.DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");


        sesionStartTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    /// <summary>
    /// Actualiza la lógica del componente en cada fotograma.
    /// </summary>
    void Update()
    {
        // Tiempo real de esta sesión
        SesionTimer = Time.realtimeSinceStartup - sesionStartTime;

        // Tiempo TOTAL jugado (todas las sesiones)
        tiempoJuego += Time.unscaledDeltaTime;

    }

}
