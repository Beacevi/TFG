
using UnityEngine;
using System.IO;
using GUPS.AntiCheat.Protected;
using TMPro;
 
public class GameManagerLogic : MonoBehaviour
{
    public static GameManagerLogic Instance;

    [Header("Variables")]
    [SerializeField] private ProtectedInt32 coins = 200;
    [SerializeField] private ProtectedInt32 gems = 20;
    [SerializeField] private ProtectedInt32 energy = 6;
    [SerializeField] private ProtectedInt32 currentLevel = 1;
    [SerializeField] private ProtectedInt32 balloonLevel = 1;
    public bool cloudsClosing = false;

    // ?? Validación: rangos máximos
    private const int MAX_COINS = 999999;
    private const int MAX_GEMS = 99999;
    private const int MAX_ENERGY = 500;
    private const int MAX_CHANGE_PER_OP = 1000;

    // ?? Validación: control temporal
    private float lastCoinsChange;
    private float lastGemsChange;
    private float lastEnergyChange;
    private const float MIN_TIME_BETWEEN_CHANGES = 0.1f;

    // ?? Validación: origen del cambio
    private bool operationAuthorized = false;

    // ?? Validación: redundancia
    private int coinsBackup;
    private int gemsBackup;
    private int energyBackup;

    // ?? Control de inicialización
    // Evita falsos positivos durante el frame inicial en que la UI
    // todavía no ha sido asignada o los backups aún no están listos.
    private bool isReady = false;

    private string savePath;
    public CSVReader reader;

    [Header("UI")]
    [SerializeField] private TMP_Text coins_ui;
    [SerializeField] private TMP_Text gems_ui;

    // ────────────────────────────────────────────────────────────────────────

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

    private void SetReady()
    {
        isReady = true;
    }

    // ────────────────────────────────────────────────────────────────────────

    private void UpdateUI()
    {
        coins_ui.text = coins.ToString();
        gems_ui.text = gems.ToString();
    }

    private void Update()
    {
        // No comprobar nada hasta que el sistema esté completamente inicializado
        if (!isReady) return;

        CheckUICoherence();
        CheckStateIntegrity();
    }

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

    private void HandleCheatDetected(string origen)
    {
        Debug.LogWarning($"[AntiCheat] Trampa detectada. Origen: {origen}");
        ResetSave();
        UpdateUI();
    }

    // ────────────────────────────────────────────────────────────────────────
    #region AddMoney

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

    public int GetMoney() => coins;
    public int GetGems() => gems;
    public int GetEnergy() => energy;
    public int GetCurretLevel() => currentLevel;
    public int GetBalloonLevel() => balloonLevel;

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region Nivel

    public void SetANewCurrentLevel()
    {
        operationAuthorized = true;
        currentLevel += 1;
        SaveGame();
    }

    public void SetANewBalloonLevel()
    {
        operationAuthorized = true;
        balloonLevel += 1;
        SaveGame();
    }

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region Save / Load / Reset

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

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    #endregion
}