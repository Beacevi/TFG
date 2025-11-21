using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int coins = 200;
    public int gems = 20;
    public int energy = 6;
    public int currentLevel = 1;
    public int balloonLevel = 1;

    private string savePath;

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

    public void SaveGame()
    {
        SaveDataManager data = new SaveDataManager();
        data.coins = coins;
        data.gems = gems;
        data.energy = energy;
        data.currentLevel = currentLevel;
        data.balloonLevel = balloonLevel;

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
        coins = 200;
        gems = 20;
        energy = 6;
        currentLevel = 1;
        balloonLevel = 1;

        SaveGame();
        Debug.Log("Save reset to default values.");
    }

    private void OnApplicationQuit() //< QuitGamePause (No pdn estar las dos funciones a la vez)
    {
        SaveGame();   //< Para que cndo se salga del juego se guarde todo
        //ResetSave(); //< Para resetear todo
    }
}
