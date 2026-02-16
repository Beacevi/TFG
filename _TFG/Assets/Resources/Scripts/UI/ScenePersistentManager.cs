using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePersistentManager : MonoBehaviour
{
    private static ScenePersistentManager _instance;

    [Header("UI")]
    [SerializeField] private GameObject optionMenu;
    [SerializeField] private GameObject hamburgerMenu;

    [Header("World Objects")]
    [SerializeField] private GameObject bird;
    [SerializeField] private GameObject balloon;

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
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        ApplyVisibility(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyVisibility(scene.name);
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (scene.name == "SimonSays")
        {
            RestoreAfterMiniGame();
        }
    }

    private void ApplyVisibility(string sceneName)
    {
        bool isSimonSays = sceneName == "SimonSays";
        bool isTerrain = sceneName == "ProceduralTerrain";

        //UI
        if (optionMenu) { optionMenu.SetActive(!isSimonSays); }
        if (hamburgerMenu) { hamburgerMenu.SetActive(!isTerrain); }

        //World Objects
        if (bird) bird.SetActive(!isSimonSays && !isTerrain);
        if (balloon) balloon.SetActive(!isSimonSays && !isTerrain);
    }

    private void RestoreAfterMiniGame()
    {
        if (optionMenu) { optionMenu.SetActive(true); }
        if (hamburgerMenu) { hamburgerMenu.SetActive(true); }
        if (bird) { bird.SetActive(true); }
        if (balloon) { balloon.SetActive(true); }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
}

