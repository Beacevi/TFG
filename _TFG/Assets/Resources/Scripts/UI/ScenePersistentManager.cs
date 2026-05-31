/**
 * @file ScenePersistentManager.cs
 * @brief Mantiene objetos o información persistente entre cambios de escena.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Mantiene objetos o información persistente entre cambios de escena.
/// </summary>
public class ScenePersistentManager : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo ScenePersistentManager utilizado para almacenar o configurar instance.
    /// </summary>
    public static ScenePersistentManager instance;

    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar option menu.
    /// </summary>
    [Header("UI")]
    [SerializeField] private GameObject optionMenu;
    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar hamburger menu.
    /// </summary>
    [SerializeField] private GameObject hamburgerMenu;

    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar bird.
    /// </summary>
    [Header("World Objects")]
    [SerializeField] private GameObject bird;
    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar balloon.
    /// </summary>
    [SerializeField] private GameObject balloon;
    /// <summary>
    /// Campo de tipo Bird utilizado para almacenar o configurar interacted bird.
    /// </summary>
    public Bird interactedBird;

    public List <Bird> collectedBirdsList;


    /// <summary>
    /// Inicializa referencias internas antes de que comience la ejecución normal del componente.
    /// </summary>
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

/// <summary>
/// Ejecuta la lógica asociada a on scene loaded dentro de ScenePersistentManager.
/// </summary>
/// <param name="scene">Nombre o referencia de la escena objetivo.</param>
/// <param name="mode">Parámetro mode empleado por el método.</param>
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

    /// <summary>
    /// Ejecuta la lógica asociada a on scene unloaded dentro de ScenePersistentManager.
    /// </summary>
    /// <param name="scene">Nombre o referencia de la escena objetivo.</param>
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

    /// <summary>
    /// Ejecuta la lógica asociada a apply visibility dentro de ScenePersistentManager.
    /// </summary>
    /// <param name="sceneName">Nombre o referencia de la escena objetivo.</param>
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

    /// <summary>
    /// Ejecuta la lógica asociada a restore after mini game dentro de ScenePersistentManager.
    /// </summary>
    private void RestoreAfterMiniGame()
    {
        if (optionMenu) { optionMenu.SetActive(true); }
        if (hamburgerMenu) { hamburgerMenu.SetActive(true); }
        if (bird) { bird.SetActive(true); }
        if (balloon) { balloon.SetActive(true); }
    }

    /// <summary>
    /// Libera referencias o recursos cuando el objeto va a destruirse.
    /// </summary>
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
}

