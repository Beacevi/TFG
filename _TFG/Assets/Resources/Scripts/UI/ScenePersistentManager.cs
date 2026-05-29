using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePersistentManager : MonoBehaviour
{
    public static ScenePersistentManager instance;

    [Header("UI")]
    [SerializeField] private GameObject optionMenu;
    [SerializeField] private GameObject hamburgerMenu;

    [Header("World Objects")]
    [SerializeField] private GameObject bird;
    [SerializeField] private GameObject balloon;
    public Bird interactedBird;

    public List <Bird> collectedBirdsList;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        ApplyVisibility(SceneManager.GetActiveScene().name);
        collectedBirdsList = new List<Bird>();
    }

private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    ApplyVisibility(scene.name);

    if (scene.name.Equals("UI"))
    {
        if (collectedBirdsList.Count > 0)
        {
            Debug.Log("Habia pajaros en la lista de domesticados");
            foreach (Bird b in collectedBirdsList)
            {
                Debug.Log($"Pájaro recogido: {b.birdName}");
            }
        }
        else
        {
            Debug.Log("NOOOOOO habia pajaros en la lista de domesticados");
        }
    }
}

    private void OnSceneUnloaded(Scene scene)
    {
        if (scene.name == "SimonSays")
        {
            RestoreAfterMiniGame();
        }

        if (scene.name == "SimonSaysPajaro")
        {
           //TileAStar tileAStar = GameObject.FindGameObjectWithTag("Player").GetComponent<TileAStar>();
           //tileAStar.RemoveBirdAtLastNode();
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

