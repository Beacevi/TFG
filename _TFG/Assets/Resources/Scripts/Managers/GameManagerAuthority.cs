using UnityEngine;
using TMPro;

/// <summary>
/// Sistema 3 – GameManager del cliente para la arquitectura de autoridad centralizada.
///
/// A diferencia de GameManager (Sistema 1) y GameManagerLogic (Sistema 2),
/// esta clase NO almacena ni modifica directamente las variables críticas.
/// Toda operación económica se delega a EconomyAuthority, que actúa como
/// servidor local y es la única fuente de verdad sobre el estado del juego.
///
/// El cliente solo puede:
///   1. Solicitar operaciones a la autoridad (RequestXxx)
///   2. Leer el estado actual a través de los getters de la autoridad
///   3. Actualizar la UI con los valores que la autoridad confirma
/// </summary>
public class GameManagerAuthority : MonoBehaviour
{
    public static GameManagerAuthority Instance;

    // ── Referencia a la autoridad ────────────────────────────────────────────
    // El cliente necesita la autoridad para funcionar; si no está presente, no opera.
    private EconomyAuthority authority;

    public bool cloudsClosing = false;
    public CSVReader reader;

    // ── UI ───────────────────────────────────────────────────────────────────
    [Header("UI")]
    [SerializeField] private TMP_Text coins_ui;
    [SerializeField] private TMP_Text gems_ui;

    // ────────────────────────────────────────────────────────────────────────
    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Obtener la referencia a la autoridad
        authority = EconomyAuthority.Instance;

        if (authority == null)
        {
            Debug.LogError("[GameManagerAuthority] EconomyAuthority no encontrada en la escena. " +
                           "Asegúrate de que EconomyAuthority está presente y se inicializa antes que este script.");
        }
    }

    private void Start()
    {
        RefreshUI();
    }

    private void OnApplicationQuit()
    {
        // El guardado lo gestiona la autoridad; aquí solo lo invocamos
        // para asegurar que el estado más reciente queda persistido.
        if (CheckAuthority())
            authority.SaveState();
    }

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region Operaciones económicas (el cliente delega en la autoridad)

    /// <summary>
    /// Solicita a la autoridad modificar las monedas.
    /// Devuelve true si la operación fue aceptada.
    /// </summary>
    public bool AddMoney(int amount)
    {
        if (!CheckAuthority()) return false;

        bool accepted = authority.RequestCoinsChange(amount, "GameManagerAuthority.AddMoney");

        if (accepted)
            RefreshUI();
        else
            Debug.LogWarning($"[GameManagerAuthority] AddMoney({amount}) rechazado por la autoridad.");

        return accepted;
    }

    /// <summary>
    /// Solicita a la autoridad modificar las gemas.
    /// </summary>
    public void AddGems(int amount)
    {
        if (!CheckAuthority()) return;

        bool accepted = authority.RequestGemsChange(amount, "GameManagerAuthority.AddGems");

        if (accepted)
            RefreshUI();
        else
            Debug.LogWarning($"[GameManagerAuthority] AddGems({amount}) rechazado por la autoridad.");
    }

    /// <summary>
    /// Solicita a la autoridad modificar la energía.
    /// </summary>
    public void AddEnergy(int amount)
    {
        if (!CheckAuthority()) return;

        bool accepted = authority.RequestEnergyChange(amount, "GameManagerAuthority.AddEnergy");

        if (!accepted)
            Debug.LogWarning($"[GameManagerAuthority] AddEnergy({amount}) rechazado por la autoridad.");
    }

    /// <summary>
    /// Solicita a la autoridad establecer la energía a un valor absoluto.
    /// </summary>
    public void SetEnergy(int amount)
    {
        if (!CheckAuthority()) return;

        bool accepted = authority.RequestSetEnergy(amount, "GameManagerAuthority.SetEnergy");

        if (!accepted)
            Debug.LogWarning($"[GameManagerAuthority] SetEnergy({amount}) rechazado por la autoridad.");
    }

    /// <summary>
    /// Solicita a la autoridad avanzar al siguiente nivel.
    /// </summary>
    public void SetANewCurrentLevel()
    {
        if (!CheckAuthority()) return;

        authority.RequestLevelUp("GameManagerAuthority.SetANewCurrentLevel");
    }

    /// <summary>
    /// Solicita a la autoridad avanzar el nivel del globo.
    /// </summary>
    public void SetANewBalloonLevel()
    {
        if (!CheckAuthority()) return;

        authority.RequestBalloonLevelUp("GameManagerAuthority.SetANewBalloonLevel");
    }

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region Getters (el cliente lee de la autoridad, no de sus propias variables)

    public int GetMoney() => CheckAuthority() ? authority.GetCoins() : 0;
    public int GetGems() => CheckAuthority() ? authority.GetGems() : 0;
    public int GetEnergy() => CheckAuthority() ? authority.GetEnergy() : 0;
    public int GetCurretLevel() => CheckAuthority() ? authority.GetCurrentLevel() : 1;
    public int GetBalloonLevel() => CheckAuthority() ? authority.GetBalloonLevel() : 1;

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region UI

    /// <summary>
    /// Actualiza la interfaz consultando los valores a la autoridad.
    /// El cliente nunca muestra un valor que no haya sido validado por la autoridad.
    /// </summary>
    private void RefreshUI()
    {
        if (!CheckAuthority()) return;

        if (coins_ui != null) coins_ui.text = authority.GetCoins().ToString();
        if (gems_ui != null) gems_ui.text = authority.GetGems().ToString();
    }

    #endregion

    // ────────────────────────────────────────────────────────────────────────
    #region Utilidades internas

    /// <summary>
    /// Comprueba que la autoridad está disponible antes de operar.
    /// </summary>
    private bool CheckAuthority()
    {
        if (authority != null) return true;

        // Segundo intento: buscar la autoridad en escena si aún no se había cargado
        authority = EconomyAuthority.Instance;

        if (authority == null)
        {
            Debug.LogError("[GameManagerAuthority] EconomyAuthority no disponible.");
            return false;
        }

        return true;
    }

    #endregion
}