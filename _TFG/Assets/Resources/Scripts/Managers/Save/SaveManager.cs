/**
 * @file SaveManager.cs
 * @brief Gestiona el almacenamiento y recuperación de datos persistentes del proyecto.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using Firebase;
using Firebase.Firestore;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// Gestiona el almacenamiento y recuperación de datos persistentes del proyecto.
/// </summary>
public class SaveManager : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo FirebaseFirestore utilizado para almacenar o configurar db.
    /// </summary>
    static FirebaseFirestore db;
    /// <summary>
    /// Campo de tipo SaveManager utilizado para almacenar o configurar save instance.
    /// </summary>
    public static SaveManager saveInstance;
    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar player name.
    /// </summary>
    public string playerName;
    /// <summary>
    /// Referencia al jugador o a su objeto asociado en la escena.
    /// </summary>
    public Player player;

    /// <summary>
    /// Inicializa referencias internas antes de que comience la ejecución normal del componente.
    /// </summary>
    void Awake()
    {
        if (saveInstance == null)
        {
            saveInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    private async void Start()
    {
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            db = FirebaseFirestore.DefaultInstance;
            Debug.Log("Firebase inicializado correctamente");
        }
        else
        {
            Debug.LogError($"No se pudieron resolver las dependencias de Firebase: {dependencyStatus}");
        }
    }

    /// <summary>
    /// Guarda el estado actual para que pueda recuperarse posteriormente.
    /// </summary>
    public void Save()
    {
        // Obtener el Player del nivel 1
        Player playerLevel1 = PlayerManager.playerInstance.levelTable[1];

        // Guardar en Firestore con un nombre de documento
        SavePlayer(playerName, playerLevel1);
    }

    /// <summary>
    /// Carga el estado previamente guardado y restaura los datos del sistema.
    /// </summary>
    public async void Load()
    {
        // Antes de cargar
        Player playerAntes = PlayerManager.playerInstance.levelTable[1];
        Debug.Log($"Antes de cargar: Nivel={playerAntes.level}, SC={playerAntes.SC}");

        // Cargar desde Firebase
        Player loadedPlayer = await LoadPlayer(playerName);

        if (loadedPlayer != null)
        {
            Debug.Log($"Datos cargados: Nivel={loadedPlayer.level}, SC={loadedPlayer.SC}");

            // Actualizar levelTable[1] con los datos cargados
            PlayerManager.playerInstance.levelTable[1] = loadedPlayer;
        }
        else
        {
            Debug.LogWarning(" No se encontraron datos para este jugador.");
        }

        // Después de cargar
        Player playerDespues = PlayerManager.playerInstance.levelTable[1];
        Debug.Log($"Después de cargar: Nivel={playerDespues.level}, SC={playerDespues.SC}");
    }


    /// <summary>
    /// Guarda el estado actual para que pueda recuperarse posteriormente.
    /// </summary>
    /// <param name="playerName">Referencia al jugador afectado por la operación.</param>
    /// <param name="player">Referencia al jugador afectado por la operación.</param>
    public void SavePlayer(string playerName, Player player)
    {
        Player playerData = new Player(player);

        db.Collection("Jugadores").Document(playerName).SetAsync(playerData).ContinueWith(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log($"Partida guardada correctamente como '{playerName}' en colección 'Jugadores'");
            }
            else
            {
                Debug.LogError($"Error al guardar la partida: {task.Exception}");
            }
        });
    }

    /// <summary>
    /// Carga el estado previamente guardado y restaura los datos del sistema.
    /// </summary>
    /// <param name="playerName">Referencia al jugador afectado por la operación.</param>
    /// <returns>Instancia o valor de tipo Task<Player> resultante de la operación.</returns>
    public async Task<Player> LoadPlayer(string playerName)
    {
        try
        {
            DocumentReference docRef = db.Collection("Jugadores").Document(playerName);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Player player = snapshot.ConvertTo<Player>();
                Debug.Log($"Datos del jugador '{playerName}' cargados correctamente.");
                return player;
            }
            else
            {
                Debug.LogWarning($"No se encontró ningún documento para el jugador '{playerName}'.");
                return null;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al cargar los datos del jugador: {e}");
            return null;
        }
    }


}
