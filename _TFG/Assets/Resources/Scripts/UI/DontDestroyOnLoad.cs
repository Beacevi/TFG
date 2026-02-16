using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoad : MonoBehaviour
{
    private static DontDestroyOnLoad _instance;

    [SerializeField] private GameObject _OptionMenu;
    [SerializeField] private GameObject _HamburguerMenu;
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
        ApplyMenuVisibility(SceneManager.GetActiveScene().name);
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyMenuVisibility(scene.name);
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (scene.name == "SimonSays")
        {
            _OptionMenu?.SetActive(true);
            _HamburguerMenu?.SetActive(true);
        }
    }
    private void ApplyMenuVisibility(string sceneName)
    {
        bool isSimonSays = sceneName == "SimonSays";
        bool isTerrain   = sceneName == "ProceduralTerrain";
        _OptionMenu?.SetActive(!isSimonSays);
        _HamburguerMenu?.SetActive(!isTerrain);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
}
