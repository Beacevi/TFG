/**
 * @file GameManagerLogic.cs
 * @brief Agrupa lógica de apoyo relacionada con las reglas de progreso y economía del juego.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */


using UnityEngine;
using System.IO;
using GUPS.AntiCheat.Protected;
using TMPro;
 
/// <summary>
/// Agrupa lógica de apoyo relacionada con las reglas de progreso y economía del juego.
/// </summary>
public class GameManagerLogic : MonoBehaviour
{
    /// <summary>
    /// Instancia singleton utilizada para acceder globalmente al controlador.
    /// </summary>
    public static GameManagerLogic Instance;

    /// <summary>
    /// Cantidad de monedas almacenadas o mostradas por el sistema.
    /// </summary>
    [Header("Variables")]
    [SerializeField] private ProtectedInt32 coins = 200;
    /// <summary>
    /// Cantidad de gemas almacenadas o mostradas por el sistema.
    /// </summary>
    [SerializeField] private ProtectedInt32 gems = 20;
    /// <summary>
    /// Valor de energía o stamina disponible para el jugador.
    /// </summary>
    [SerializeField] private ProtectedInt32 energy = 6;
    /// <summary>
    /// Nivel actual de progreso del jugador.
    /// </summary>
    [SerializeField] private ProtectedInt32 currentLevel = 1;
    /// <summary>
    /// Nivel actual del globo aerostático.
    /// </summary>
    [SerializeField] private ProtectedInt32 balloonLevel = 1;
    /// <summary>
    /// Indica si clouds closing está activo o habilitado.
    /// </summary>
    public bool cloudsClosing = false;

    // ?? Validación: rangos máximos
    /// <summary>
    /// Valor numérico que limita o define max coins.
    /// </summary>
    private const int MAX_COINS = 999999;
    /// <summary>
    /// Valor numérico que limita o define max gems.
    /// </summary>
    private const int MAX_GEMS = 99999;
    /// <summary>
    /// Valor numérico que limita o define max energy.
    /// </summary>
    private const int MAX_ENERGY = 500;
    /// <summary>
    /// Valor numérico que limita o define max change per op.
    /// </summary>
    private const int MAX_CHANGE_PER_OP = 1000;

    // ?? Validación: control temporal
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar last coins change.
    /// </summary>
    private float lastCoinsChange;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar last gems change.
    /// </summary>
    private float lastGemsChange;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar last energy change.
    /// </summary>
    private float lastEnergyChange;
    /// <summary>
    /// Valor numérico que limita o define min time between changes.
    /// </summary>
    private const float MIN_TIME_BETWEEN_CHANGES = 0.1f;

    // ?? Validación: origen del cambio
    /// <summary>
    /// Indica si operation authorized está activo o habilitado.
    /// </summary>
    private bool operationAuthorized = false;

    // ?? Validación: redundancia
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar coins backup.
    /// </summary>
    private int coinsBackup;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar gems backup.
    /// </summary>
    private int gemsBackup;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar energy backup.
    /// </summary>
    private int energyBackup;

    // ?? Control de inicialización
    // Evita falsos positivos durante el frame inicial en que la UI
    // todavía no ha sido asignada o los backups aún no están listos.
    /// <summary>
    /// Indica si is ready está activo o habilitado.
    /// </summary>
    private bool isReady = false;

    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar save path.
    /// </summary>
    private string savePath;
    /// <summary>
    /// Campo de tipo CSVReader utilizado para almacenar o configurar reader.
    /// </summary>
    public CSVReader reader;

    /// <summary>
    /// Referencia de interfaz utilizada para mostrar o actualizar coins ui.
    /// </summary>
    [Header("UI")]
    [SerializeField] private TMP_Text coins_ui;
    /// <summary>
    /// Referencia de interfaz utilizada para mostrar o actualizar gems ui.
    /// </summary>
    [SerializeField] private TMP_Text gems_ui;

    // ────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Inicializa referencias internas antes de que comience la ejecución normal del componente.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Application.persistentDataPath + "/save.json";
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    public void Start()
    {
        // Inicializar backups con los valores cargados
        coinsBackup = coins;
        gemsBackup = gems;
        energyBackup = energy;

        UpdateUI();

        // Marcamos el sistema como listo UN frame después de Start,
        // para que la UI tenga tiempo de renderizarse antes de empezar
        // a comprobar coherencia. Sin esto se producen falsos positivos.
        Invoke(nameof(SetReady), 0.1f);
    }

    /// <summary>
    /// Establece o actualiza ready dentro del sistema.
    /// </summary>
    private void SetReady()
    {
        isReady = true;
    }

    // ────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Actualiza ui para reflejar el estado actual del sistema.
    /// </summary>
    private void UpdateUI()
    {
        coins_ui.text = coins.ToString();
        gems_ui.text = gems.ToString();
    }

    /// <summary>
    /// Actualiza la lógica del componente en cada fotograma.
    /// </summary>
    private void Update()
    {
        // No comprobar nada hasta que el sistema esté completamente inicializado
        if (!isReady) return;

        CheckUICoherence();
        CheckStateIntegrity();
    }

    /// <summary>
    /// Ejecuta la lógica asociada a check ui coherence dentro de GameManagerLogic.
    /// </summary>
    private void CheckUICoherence()
    {
        // Si la UI no está asignada todavía, no comprobamos nada
        if (coins_ui == null || gems_ui == null) return;

        if (coins_ui.text != coins.ToString() ||
            gems_ui.text != gems.ToString())
        {
            Debug.LogWarning("Incoherencia detectada entre UI y estado interno.");
            HandleCheatDetected("Coherencia UI");
        }
    }

    /// <summary>
    /// Ejecuta la lógica asociada a check state integrity dentro de GameManagerLogic.
    /// </summary>
    private void CheckStateIntegrity()
    {
        if ((int)coins != coinsBackup ||
            (int)gems != gemsBackup ||
            (int)energy != energyBackup)
        {
            if (!operationAuthorized)
            {
                Debug.LogWarning("Modificación no autorizada detectada.");
                HandleCheatDetected("Integridad de estado");
            }
        }
        operationAuthorized = false;
    }

    // ────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Ejecuta la lógica asociada a handle cheat detected dentro de GameManagerLogic.
    /// </summary>
    /// <param name="origen">Parámetro origen empleado por el método.</param>
    private void HandleCheatDetected(string origen)
    {
        Debug.LogWarning($"[AntiCheat] Trampa detectada. Origen: {origen}");
        ResetSave();
        UpdateUI();
    }

    // ────────────────────────────────────────────────────────────────────────
    #region AddMoney

    /// <summary>
    /// Añade money al estado gestionado por el componente.
    /// </summary>
    /// <param name="amount">Cantidad que se debe aplicar en la operación.</param>
    /// <returns>true si la operación se ha completado correctamente; false en caso contrario.</returns>
    public bool AddMoney(int amount)
    {
        operationAuthorized = true;

        if (Time.time - lastCoinsChange < MIN_TIME_BETWEEN_CHANGES)
        {
            Debug.LogWarning("Cambio de coins demasiado rápido.");
            HandleCheatDetected("Temporal coins");
            return false;
        }

        if (amount > MAX_CHANGE_PER_OP || amount < -MAX_CHANGE_PER_OP)
        {
            Debug.LogWarning("Cantidad sospechosa en coins: " + amount);
            HandleCheatDetected("Rango coins");
            return false;
        }

        if (coins + amount < 0)
        {
            Debug.LogWarning("Not enough coins!");
            operationAuthorized = false;
            return false;
        }

        if (coins + amount > MAX_COINS)
        {
            Debug.LogWarning("Coins excede el máximo.");
            operationAuthorized = false;
            return false;
        }

        coins += amount;
        coinsBackup = coins;
        lastCoinsChange = Time.time;

        UpdateUI();
        SaveGame();
        return true;
    }

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region AddGems

    /// <summary>
    /// Añade gems al estado gestionado por el componente.
    /// </summary>
    /// <param name="amount">Cantidad que se debe aplicar en la operación.</param>
    public void AddGems(int amount)
    {
        operationAuthorized = true;

        if (Time.time - lastGemsChange < MIN_TIME_BETWEEN_CHANGES)
        {
            Debug.LogWarning("Cambio de gems demasiado rápido.");
            HandleCheatDetected("Temporal gems");
            return;
        }

        if (amount > MAX_CHANGE_PER_OP || amount < -MAX_CHANGE_PER_OP)
        {
            Debug.LogWarning("Cantidad sospechosa en gems: " + amount);
            HandleCheatDetected("Rango gems");
            return;
        }

        if (gems + amount < 0)
        {
            Debug.LogWarning("Not enough gems!");
            operationAuthorized = false;
            return;
        }

        if (gems + amount > MAX_GEMS)
        {
            Debug.LogWarning("Gems excede el máximo.");
            operationAuthorized = false;
            return;
        }

        gems += amount;
        gemsBackup = gems;
        lastGemsChange = Time.time;

        UpdateUI();
        SaveGame();
    }

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region AddEnergy / SetEnergy

    /// <summary>
    /// Añade energy al estado gestionado por el componente.
    /// </summary>
    /// <param name="amount">Cantidad que se debe aplicar en la operación.</param>
    public void AddEnergy(int amount)
    {
        operationAuthorized = true;

        if (Time.time - lastEnergyChange < MIN_TIME_BETWEEN_CHANGES)
        {
            Debug.LogWarning("Cambio de energy demasiado rápido.");
            HandleCheatDetected("Temporal energy");
            return;
        }

        if (amount > MAX_CHANGE_PER_OP || amount < -MAX_CHANGE_PER_OP)
        {
            Debug.LogWarning("Cantidad sospechosa en energy: " + amount);
            HandleCheatDetected("Rango energy");
            return;
        }

        if (energy + amount < 0)
        {
            Debug.LogWarning("Not enough energy!");
            operationAuthorized = false;
            return;
        }

        energy = energy + amount > MAX_ENERGY ? MAX_ENERGY : energy + amount;

        energyBackup = energy;
        lastEnergyChange = Time.time;

        SaveGame();
    }

    /// <summary>
    /// Establece o actualiza energy dentro del sistema.
    /// </summary>
    /// <param name="amount">Cantidad que se debe aplicar en la operación.</param>
    public void SetEnergy(int amount)
    {
        operationAuthorized = true;

        if (amount < 0 || amount > MAX_ENERGY)
        {
            Debug.LogWarning("Valor de energy fuera de rango.");
            HandleCheatDetected("Rango SetEnergy");
            return;
        }

        energy = amount;
        energyBackup = amount;

        SaveGame();
    }

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region Getters

    /// <summary>
    /// Obtiene money a partir del estado actual del sistema.
    /// </summary>
    /// <returns>Valor numérico calculado o consultado por el método.</returns>
    public int GetMoney() => coins;
    /// <summary>
    /// Obtiene gems a partir del estado actual del sistema.
    /// </summary>
    /// <returns>Valor numérico calculado o consultado por el método.</returns>
    public int GetGems() => gems;
    /// <summary>
    /// Obtiene energy a partir del estado actual del sistema.
    /// </summary>
    /// <returns>Valor numérico calculado o consultado por el método.</returns>
    public int GetEnergy() => energy;
    /// <summary>
    /// Obtiene curret level a partir del estado actual del sistema.
    /// </summary>
    /// <returns>Valor numérico calculado o consultado por el método.</returns>
    public int GetCurretLevel() => currentLevel;
    /// <summary>
    /// Obtiene balloon level a partir del estado actual del sistema.
    /// </summary>
    /// <returns>Valor numérico calculado o consultado por el método.</returns>
    public int GetBalloonLevel() => balloonLevel;

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region Nivel

    /// <summary>
    /// Establece o actualiza a new current level dentro del sistema.
    /// </summary>
    public void SetANewCurrentLevel()
    {
        operationAuthorized = true;
        currentLevel += 1;
        SaveGame();
    }

    /// <summary>
    /// Establece o actualiza a new balloon level dentro del sistema.
    /// </summary>
    public void SetANewBalloonLevel()
    {
        operationAuthorized = true;
        balloonLevel += 1;
        SaveGame();
    }

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region Save / Load / Reset

    /// <summary>
    /// Guarda el estado actual para que pueda recuperarse posteriormente.
    /// </summary>
    public void SaveGame()
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
        Debug.Log("Game Saved: " + savePath);
    }

    /// <summary>
    /// Carga el estado previamente guardado y restaura los datos del sistema.
    /// </summary>
    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                SaveDataManager data = JsonUtility.FromJson<SaveDataManager>(json);

                if (data.coins > MAX_COINS || data.gems > MAX_GEMS || data.energy > MAX_ENERGY
                    || data.coins < 0 || data.gems < 0 || data.energy < 0)
                {
                    Debug.LogWarning("Save file con valores fuera de rango.");
                    ResetSave();
                    return;
                }

                coins = data.coins;
                gems = data.gems;
                energy = data.energy;
                currentLevel = data.currentLevel;
                balloonLevel = data.balloonLevel;

                coinsBackup = coins;
                gemsBackup = gems;
                energyBackup = energy;
            }
            catch
            {
                Debug.LogWarning("Save file corrupted.");
                ResetSave();
            }
        }
        else
        {
            Debug.Log("No save file found.");
        }
    }

    /// <summary>
    /// Restablece los valores del sistema a su configuración inicial o por defecto.
    /// </summary>
    public void ResetSave()
    {
        coins = 200;
        gems = 20;
        energy = 6;
        currentLevel = 1;
        balloonLevel = 1;
        coinsBackup = 200;
        gemsBackup = 20;
        energyBackup = 6;

        SaveGame();
        Debug.Log("Save reset to default values.");
    }

    /// <summary>
    /// Guarda o sincroniza datos cuando la aplicación se cierra.
    /// </summary>
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    #endregion
}
