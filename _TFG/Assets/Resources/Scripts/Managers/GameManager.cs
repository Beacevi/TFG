using UnityEngine;
using System.IO;
using GUPS.AntiCheat.Protected;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Variables")]
    [SerializeField] private ProtectedInt32 coins        = 200;
    [SerializeField] private ProtectedInt32 gems         =  20;
    [SerializeField] private ProtectedInt32 energy       =   6;
    [SerializeField] private ProtectedInt32 currentLevel =   1;
    [SerializeField] private ProtectedInt32 balloonLevel =   1;
    public bool cloudsClosing = false; //Si es false, es que las nubes no cubren la escena, si es true, las nubes cubren la escena

    private string savePath;
    public CSVReader reader;

    [Header("UI")]
    [SerializeField] private TMP_Text coins_ui;
    [SerializeField] private TMP_Text gems_ui;
    //[SerializeField] private TMP_Text energy_ui;


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
        coins_ui.text = coins.ToString();
        gems_ui.text  =  gems.ToString();
    }
    public void SaveGame()
    {
        SaveDataManager data = new SaveDataManager();
        data.coins           = coins;
        data.gems            = gems;
        data.energy          = energy;
        data.currentLevel    = currentLevel;
        data.balloonLevel    = balloonLevel;

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

                coins = data.coins;
                gems = data.gems;
                energy = data.energy;
                currentLevel = data.currentLevel;
                balloonLevel = data.balloonLevel;
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

    private void OnApplicationQuit() //< QuitGamePause (No pdn estar las dos funciones a la vez)
    {
        SaveGame();   //< Para que cndo se salga del juego se guarde todo
        //ResetSave(); //< Para resetear todo
    }



    public bool AddMoney(int amount)
    {
        if (coins + amount < 0)
        {
            Debug.LogWarning("Not enough coins!");
            return false;
        }
        coins += amount;
        coins_ui.text = coins.ToString();

        SaveGame();

        return true;
    }
    public int GetMoney()
    {
        return coins; 
    }

    public void AddGems(int amount)
    {
        gems += amount;
        gems_ui.text = gems.ToString();

        SaveGame();
    }
    public int GetGems()
    {
        return gems;
    }

    public void AddEnergy(int amount)
    {
        energy += amount;

        SaveGame();
    }
    public void SetEnergy(int amount)
    {
        energy = amount;

        SaveGame();
    }

    public int GetEnergy()
    {
        return energy;
    }

    public int GetCurretLevel()
    {
        return currentLevel;
    }   

    public void SetANewCurrentLevel()
    {
        currentLevel += 1;


        SaveGame();
    }

    public int GetBalloonLevel()
    {
        return balloonLevel;
    }

    public void SetANewBalloonLevel()
    {
        balloonLevel += 1;

        SaveGame();
    }
}
