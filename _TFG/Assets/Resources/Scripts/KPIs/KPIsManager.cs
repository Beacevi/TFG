using UnityEngine;

public class KPIsManager : MonoBehaviour
{
    public static KPIsManager Instance;

    public float SesionTimer; //Lo que lleva jugado en UNA sesión
    public float tiempoJuego; //Lo que lleva jugado durante TODAS las sesiones
    public float MinijuegoTimer;
    public int contadorSesiones = 0;
    public string fechaActual;

    float sesionStartTime;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
    void Update()
    {
        // Tiempo real de esta sesión
        SesionTimer = Time.realtimeSinceStartup - sesionStartTime;

        // Tiempo TOTAL jugado (todas las sesiones)
        tiempoJuego += Time.unscaledDeltaTime;

    }

}
