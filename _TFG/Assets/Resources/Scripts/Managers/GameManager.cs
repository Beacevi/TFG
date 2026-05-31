/**
 * @file GameManager.cs
 * @brief Controlador persistente principal que mantiene el estado global del jugador, la economía, el guardado y la interfaz.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using GUPS.AntiCheat.Protected;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controlador persistente principal que mantiene el estado global del jugador, la economía, el guardado y la interfaz.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Instancia singleton utilizada para acceder globalmente al controlador.
    /// </summary>
    public static GameManager Instance;

    /// <summary>
    /// Cantidad de monedas almacenadas o mostradas por el sistema.
    /// </summary>
    [Header("Variables")]
    [SerializeField] private ProtectedInt32 coins        = 200;
    /// <summary>
    /// Cantidad de gemas almacenadas o mostradas por el sistema.
    /// </summary>
    [SerializeField] private ProtectedInt32 gems         =  20;
    /// <summary>
    /// Valor de energía o stamina disponible para el jugador.
    /// </summary>
    [SerializeField] private ProtectedInt32 energy       =   6;
    /// <summary>
    /// Nivel actual de progreso del jugador.
    /// </summary>
    [SerializeField] private ProtectedInt32 currentLevel =   1;
    /// <summary>
    /// Nivel actual del globo aerostático.
    /// </summary>
    [SerializeField] private ProtectedInt32 balloonLevel =   1;
    /// <summary>
    /// Indica si clouds closing está activo o habilitado.
    /// </summary>
    public bool cloudsClosing = false; //Si es false, es que las nubes no cubren la escena, si es true, las nubes cubren la escena

    /// <summary>
    /// Indica si buff money active está activo o habilitado.
    /// </summary>
    public bool buffMoneyActive;
    /// <summary>
    /// Indica si buff energy active está activo o habilitado.
    /// </summary>
    public bool buffEnergyActive;
    /// <summary>
    /// Indica si buff both active está activo o habilitado.
    /// </summary>
    public bool buffBothActive;

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
    /// <summary>
    /// Referencia al gestor encargado de sound manager.
    /// </summary>
    [SerializeField] private GameObject soundManager;


    /// <summary>
    /// Campo de tipo Store utilizado para almacenar o configurar store.
    /// </summary>
    Store store;


    /// <summary>
    /// Campo de tipo CustomMenu utilizado para almacenar o configurar custom menu.
    /// </summary>
    CustomMenu customMenu;
    //[SerializeField] private TMP_Text energy_ui;


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
            store = FindObjectOfType<Store>();
            customMenu = FindObjectOfType<CustomMenu>();
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }

        
    }
    /// <summary>
    /// Suscribe eventos o activa el comportamiento del componente al habilitarse.
    /// </summary>
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Cancela suscripciones o limpia estado temporal al deshabilitarse.
    /// </summary>
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Ejecuta la lógica asociada a on scene loaded dentro de GameManager.
    /// </summary>
    /// <param name="scene">Nombre o referencia de la escena objetivo.</param>
    /// <param name="mode">Parámetro mode empleado por el método.</param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "UI")
        {
            coins_ui = GameObject.FindGameObjectWithTag("CoinsText").GetComponent<TMP_Text>();
            gems_ui = GameObject.FindGameObjectWithTag("GemsText").GetComponent<TMP_Text>();
            soundManager = GameObject.FindGameObjectWithTag("SoundManager");
            soundManager.GetComponent<Sounds>().src = GetComponent<AudioSource>();

            UpdateUI();
        }
    }

    /// <summary>
    /// Actualiza ui para reflejar el estado actual del sistema.
    /// </summary>
    void UpdateUI()
    {
        if (coins_ui != null)
            coins_ui.text = coins.ToString();

        if (gems_ui != null)
            gems_ui.text = gems.ToString();
    }

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    public void Start()
    {
        UpdateUI();
    }

    /// <summary>
    /// Guarda el estado actual para que pueda recuperarse posteriormente.
    /// </summary>
    public void SaveGame()
    {
        SaveDataManager data = new SaveDataManager();
        data.coins           = coins;
        data.gems            = gems;
        data.energy          = energy;
        data.currentLevel    = currentLevel;
        data.balloonLevel    = balloonLevel;


        if (store != null)
        {
            data.shopItems = store.GetShopItems();
            Debug.LogWarning($"Saving shop items: {data.shopItems.Count}");
        }

        if (customMenu != null)
        {
            data.unlockedColors = customMenu.GetUnlockedColors();
        }

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

                coins = data.coins;
                gems = data.gems;
                energy = data.energy;
                currentLevel = data.currentLevel;
                balloonLevel = data.balloonLevel;

                if (store != null && data.shopItems != null)
                {
                    Debug.LogWarning($"Loading shop items: {data.shopItems.Count}");
                    store.SetShopItems(data.shopItems);
                }
                if (customMenu != null && data.unlockedColors != null)
                {
                    customMenu.SetUnlockedColors(data.unlockedColors);
                }
            }
            catch
            {
                Debug.LogWarning("Save file corrupted.");
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
        coins        = 200;
        gems         =  20;
        energy       =   6;
        currentLevel =   1;
        balloonLevel =   1;

        SaveGame();
        Debug.Log("Save reset to default values.");
    }

    /// <summary>
    /// Guarda o sincroniza datos cuando la aplicación se cierra.
    /// </summary>
    private void OnApplicationQuit() //< QuitGamePause (No pdn estar las dos funciones a la vez)
    {
        SaveGame();   //< Para que cndo se salga del juego se guarde todo
        //ResetSave(); //< Para resetear todo
    }



    /// <summary>
    /// Añade money al estado gestionado por el componente.
    /// </summary>
    /// <param name="amount">Cantidad que se debe aplicar en la operación.</param>
    public void AddMoney(int amount)
    {
        if(buffBothActive|| buffMoneyActive)
        {
            amount = amount *2; //TODO: Cambiar a que sea por el valor real
        }
        coins += amount;
        coins_ui.text = coins.ToString();
        SaveGame();
    }

    /// <summary>
    /// Comprueba y descuenta money si existen recursos suficientes.
    /// </summary>
    /// <param name="amount">Cantidad que se debe aplicar en la operación.</param>
    /// <returns>true si la operación se ha completado correctamente; false en caso contrario.</returns>
    public bool SpendMoney(int amount)
    {
        int currentCoins = coins;

        Debug.LogWarning($"Amount ! {amount} {currentCoins}");
        if (currentCoins < amount)
        {
            Debug.LogWarning("Not enough coins!");
            return false;
        }

        coins = currentCoins - amount;
        coins_ui.text = coins.ToString();

        Debug.LogWarning($"BUY ! {amount} {coins}");

        SaveGame();

        return true;
    }

    /// <summary>
    /// Obtiene money a partir del estado actual del sistema.
    /// </summary>
    /// <returns>Valor numérico calculado o consultado por el método.</returns>
    public int GetMoney()
    {
        return coins; 
    }

    /// <summary>
    /// Añade gems al estado gestionado por el componente.
    /// </summary>
    /// <param name="amount">Cantidad que se debe aplicar en la operación.</param>
    public void AddGems(int amount)
    {
        gems += amount;
        gems_ui.text = gems.ToString();

        SaveGame();
    }

    /// <summary>
    /// Comprueba y descuenta gems si existen recursos suficientes.
    /// </summary>
    /// <param name="amount">Cantidad que se debe aplicar en la operación.</param>
    /// <returns>true si la operación se ha completado correctamente; false en caso contrario.</returns>
    public bool SpendGems(int amount)
    {
        if (gems < amount)
        {
            Debug.LogWarning($"Not enough coins! {gems}");
            return false;
        }

        gems -= amount;
        gems_ui.text = gems.ToString();
        SaveGame();

        return true;
    }

    /// <summary>
    /// Obtiene gems a partir del estado actual del sistema.
    /// </summary>
    /// <returns>Valor numérico calculado o consultado por el método.</returns>
    public int GetGems()
    {
        return gems;
    }

    /// <summary>
    /// Añade energy al estado gestionado por el componente.
    /// </summary>
    /// <param name="amount">Cantidad que se debe aplicar en la operación.</param>
    public void AddEnergy(int amount)
    {

        if(buffBothActive || buffEnergyActive)
        {
            amount = amount *2; //TODO: Añadir que sea el valor del buffo
        }

        energy += amount;

        SaveGame();
    }
    /// <summary>
    /// Establece o actualiza energy dentro del sistema.
    /// </summary>
    /// <param name="amount">Cantidad que se debe aplicar en la operación.</param>
    public void SetEnergy(int amount)
    {
        energy = amount;

        SaveGame();
    }

    /// <summary>
    /// Obtiene energy a partir del estado actual del sistema.
    /// </summary>
    /// <returns>Valor numérico calculado o consultado por el método.</returns>
    public int GetEnergy()
    {
        return energy;
    }

    /// <summary>
    /// Obtiene curret level a partir del estado actual del sistema.
    /// </summary>
    /// <returns>Valor numérico calculado o consultado por el método.</returns>
    public int GetCurretLevel()
    {
        return currentLevel;
    }   

    /// <summary>
    /// Establece o actualiza a new current level dentro del sistema.
    /// </summary>
    public void SetANewCurrentLevel()
    {
        currentLevel += 1;


        SaveGame();
    }

    /// <summary>
    /// Obtiene balloon level a partir del estado actual del sistema.
    /// </summary>
    /// <returns>Valor numérico calculado o consultado por el método.</returns>
    public int GetBalloonLevel()
    {
        return balloonLevel;
    }

    /// <summary>
    /// Establece o actualiza a new balloon level dentro del sistema.
    /// </summary>
    public void SetANewBalloonLevel()
    {
        balloonLevel += 1;

        SaveGame();
    }
}
