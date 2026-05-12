using UnityEngine;
using System.IO;
using GUPS.AntiCheat.Protected;

/// <summary>
/// Sistema 3 – Autoridad lógica centralizada.
///
/// Este módulo actúa como "servidor local": es la única entidad con acceso
/// directo a las variables críticas del juego (coins, gems, energy).
/// El cliente (GameManagerAuthority) nunca toca estas variables directamente;
/// únicamente envía solicitudes de operación que la autoridad valida o rechaza.
///
/// Cuando se detecta una operación inválida o sospechosa, la autoridad
/// resetea el estado del juego a sus valores por defecto, de forma equivalente
/// a la respuesta implementada en el Sistema 2 (validación lógica en cliente).
///
/// Inspirado en el principio de servidor-autoritativo: el cliente propone,
/// la autoridad decide.
/// </summary>
public class EconomyAuthority : MonoBehaviour
{
    public static EconomyAuthority Instance;

    // ── Variables críticas (solo accesibles desde este módulo) ───────────────
    // Usamos ProtectedInt32 para añadir la capa de cifrado en memoria del Sistema 1
    // por encima de la autoridad centralizada del Sistema 3.
    [SerializeField] private int coins = 200;
    [SerializeField] private int gems = 20;
    [SerializeField] private int energy = 6;
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int balloonLevel = 1;

    // ── Límites del sistema ──────────────────────────────────────────────────
    private const int MAX_COINS = 999999;
    private const int MAX_GEMS = 99999;
    private const int MAX_ENERGY = 500;
    private const int MAX_CHANGE_PER_OP = 1000;

    // ── Persistencia ─────────────────────────────────────────────────────────
    private string savePath;

    // ────────────────────────────────────────────────────────────────────────
    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Application.persistentDataPath + "/save_authority.json";
            LoadState();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        SaveState();
    }

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region Operaciones de economía (API pública para el cliente)

    /// <summary>
    /// El cliente solicita modificar las monedas.
    /// La autoridad valida la operación antes de aplicarla.
    /// Si detecta algo sospechoso, resetea el save.
    /// Devuelve true si la operación fue aceptada.
    /// </summary>
    public bool RequestCoinsChange(int amount, string requestOrigin = "Client")
    {
        // Validación de rango por operación
        if (!IsAmountInRange(amount))
        {
            Debug.LogWarning($"[Authority] Coins: cantidad sospechosa ({amount}) desde '{requestOrigin}'.");
            HandleCheatDetected("Rango coins");
            return false;
        }

        int projected = (int)coins + amount;

        // Saldo insuficiente: rechazo sin resetear (no es trampa, es lógica de juego)
        if (projected < 0)
        {
            Debug.LogWarning("[Authority] Coins: saldo insuficiente.");
            return false;
        }

        // Supera el máximo: cantidad sospechosa
        if (projected > MAX_COINS)
        {
            Debug.LogWarning($"[Authority] Coins: resultado ({projected}) excede el máximo.");
            HandleCheatDetected("Máximo coins");
            return false;
        }

        coins = projected;
        SaveState();
        Debug.Log($"[Authority] Coins → {(int)coins} (petición: '{requestOrigin}')");
        return true;
    }

    /// <summary>
    /// El cliente solicita modificar las gemas.
    /// </summary>
    public bool RequestGemsChange(int amount, string requestOrigin = "Client")
    {
        if (!IsAmountInRange(amount))
        {
            Debug.LogWarning($"[Authority] Gems: cantidad sospechosa ({amount}) desde '{requestOrigin}'.");
            HandleCheatDetected("Rango gems");
            return false;
        }

        int projected = (int)gems + amount;

        if (projected < 0)
        {
            Debug.LogWarning("[Authority] Gems: saldo insuficiente.");
            return false;
        }

        if (projected > MAX_GEMS)
        {
            Debug.LogWarning($"[Authority] Gems: resultado ({projected}) excede el máximo.");
            HandleCheatDetected("Máximo gems");
            return false;
        }

        gems = projected;
        SaveState();
        Debug.Log($"[Authority] Gems → {(int)gems} (petición: '{requestOrigin}')");
        return true;
    }

    /// <summary>
    /// El cliente solicita modificar la energía.
    /// </summary>
    public bool RequestEnergyChange(int amount, string requestOrigin = "Client")
    {
        if (!IsAmountInRange(amount))
        {
            Debug.LogWarning($"[Authority] Energy: cantidad sospechosa ({amount}) desde '{requestOrigin}'.");
            HandleCheatDetected("Rango energy");
            return false;
        }

        int projected = (int)energy + amount;

        if (projected < 0)
        {
            Debug.LogWarning("[Authority] Energy: saldo insuficiente.");
            return false;
        }

        // La energía se topa al máximo en lugar de rechazarse
        energy = Mathf.Min(projected, MAX_ENERGY);
        SaveState();
        Debug.Log($"[Authority] Energy → {(int)energy} (petición: '{requestOrigin}')");
        return true;
    }

    /// <summary>
    /// El cliente solicita establecer la energía a un valor absoluto.
    /// Solo se permite dentro del rango válido.
    /// </summary>
    public bool RequestSetEnergy(int value, string requestOrigin = "Client")
    {
        if (value < 0 || value > MAX_ENERGY)
        {
            Debug.LogWarning($"[Authority] SetEnergy: valor fuera de rango ({value}) desde '{requestOrigin}'.");
            HandleCheatDetected("Rango SetEnergy");
            return false;
        }

        energy = value;
        SaveState();
        return true;
    }

    /// <summary>
    /// El cliente solicita avanzar al siguiente nivel de juego.
    /// </summary>
    public bool RequestLevelUp(string requestOrigin = "Client")
    {
        currentLevel += 1;
        SaveState();
        Debug.Log($"[Authority] Level up → {(int)currentLevel} (petición: '{requestOrigin}')");
        return true;
    }

    /// <summary>
    /// El cliente solicita avanzar el nivel del globo.
    /// </summary>
    public bool RequestBalloonLevelUp(string requestOrigin = "Client")
    {
        balloonLevel += 1;
        SaveState();
        Debug.Log($"[Authority] Balloon level up → {(int)balloonLevel} (petición: '{requestOrigin}')");
        return true;
    }

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region Getters (solo lectura para el cliente)

    public int GetCoins() => coins;
    public int GetGems() => gems;
    public int GetEnergy() => energy;
    public int GetCurrentLevel() => currentLevel;
    public int GetBalloonLevel() => balloonLevel;

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region Detección y respuesta

    /// <summary>
    /// Respuesta ante trampa detectada: resetea el estado del juego
    /// a sus valores por defecto, igual que en el Sistema 2.
    /// </summary>
    private void HandleCheatDetected(string origen)
    {
        Debug.LogWarning($"[Authority] Trampa detectada. Origen: {origen}. Reseteando save.");
        ResetState();
    }

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region Validaciones internas

    private bool IsAmountInRange(int amount)
    {
        return amount >= -MAX_CHANGE_PER_OP && amount <= MAX_CHANGE_PER_OP;
    }

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region Persistencia

    public void SaveState()
    {
        SaveDataManager data = new SaveDataManager
        {
            coins = coins,
            gems = gems,
            energy = energy,
            currentLevel = currentLevel,
            balloonLevel = balloonLevel
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("[Authority] Estado guardado: " + savePath);
    }

    private void LoadState()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("[Authority] No se encontró archivo de guardado. Usando valores por defecto.");
            return;
        }

        try
        {
            string json = File.ReadAllText(savePath);
            SaveDataManager data = JsonUtility.FromJson<SaveDataManager>(json);

            // Validar rangos al cargar: si el archivo fue manipulado fuera del juego,
            // la autoridad lo detecta aquí y resetea el estado.
            if (data.coins > MAX_COINS || data.coins < 0 ||
                data.gems > MAX_GEMS || data.gems < 0 ||
                data.energy > MAX_ENERGY || data.energy < 0)
            {
                Debug.LogWarning("[Authority] Archivo de guardado con valores fuera de rango. Reseteando.");
                HandleCheatDetected("LoadState - valores fuera de rango");
                return;
            }

            coins = data.coins;
            gems = data.gems;
            energy = data.energy;
            currentLevel = data.currentLevel;
            balloonLevel = data.balloonLevel;

            Debug.Log("[Authority] Estado cargado correctamente.");
        }
        catch
        {
            Debug.LogWarning("[Authority] Archivo de guardado corrupto. Reseteando.");
            HandleCheatDetected("LoadState - archivo corrupto");
        }
    }

    public void ResetState()
    {
        coins = 200;
        gems = 20;
        energy = 6;
        currentLevel = 1;
        balloonLevel = 1;

        SaveState();
        Debug.Log("[Authority] Estado reseteado a valores por defecto.");
    }

    #endregion
}