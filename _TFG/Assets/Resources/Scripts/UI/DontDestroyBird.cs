using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyBird : MonoBehaviour
{
    private static DontDestroyBird _instance;

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
        ApplyBirdVisibility(SceneManager.GetActiveScene().name);
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyBirdVisibility(scene.name);
    }
    private void ApplyBirdVisibility(string sceneName)
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
