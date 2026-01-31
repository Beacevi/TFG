using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoad : MonoBehaviour
{
    private static DontDestroyOnLoad _instance;

    [SerializeField] private GameObject _OptionMenu;
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
        ApplyMenuVisibility(SceneManager.GetActiveScene().name);
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyMenuVisibility(scene.name);
    }
    private void ApplyMenuVisibility(string sceneName)
    {
        bool isSimonSays = sceneName == "SimonSays";
        _OptionMenu?.SetActive(!isSimonSays);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
