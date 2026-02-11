using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyBallon : MonoBehaviour
{
    private static DontDestroyBallon _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        ApplyBallonVisibility(SceneManager.GetActiveScene().name);
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyBallonVisibility(scene.name);
    }
    private void ApplyBallonVisibility(string sceneName)
    {
        bool isSimonSays = sceneName == "SimonSays";
        bool isTerrein = sceneName == "ProceduralTerrain";
        if (isSimonSays)
        {
            this.gameObject?.SetActive(!isSimonSays);
        }
        else
        {
            this.gameObject?.SetActive(!isTerrein);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
