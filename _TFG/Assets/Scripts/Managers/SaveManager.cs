using Firebase;
using Firebase.Firestore;
using System.Threading.Tasks;
using UnityEngine;
public class SaveManager : MonoBehaviour
{
    static FirebaseFirestore db;
    public static SaveManager saveInstance;
    public string playerName;
    public Player player;

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

    public void Save()
    {
        // Obtener el Player del nivel 1
        Player playerLevel1 = PlayerManager.playerInstance.levelTable[1];

        // Guardar en Firestore con un nombre de documento
        SavePlayer(playerName, playerLevel1);
    }

    public async void Load()
    {
        // Antes de cargar
        Player playerAntes = PlayerManager.playerInstance.levelTable[1];
        Debug.Log($"📌 Antes de cargar: Nivel={playerAntes.level}, SC={playerAntes.SC}");

        // Cargar desde Firebase
        Player loadedPlayer = await LoadPlayer(playerName);

        if (loadedPlayer != null)
        {
            Debug.Log($"✅ Datos cargados: Nivel={loadedPlayer.level}, SC={loadedPlayer.SC}");

            // Actualizar levelTable[1] con los datos cargados
            PlayerManager.playerInstance.levelTable[1] = loadedPlayer;
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontraron datos para este jugador.");
        }

        // Después de cargar
        Player playerDespues = PlayerManager.playerInstance.levelTable[1];
        Debug.Log($"📌 Después de cargar: Nivel={playerDespues.level}, SC={playerDespues.SC}");
    }


    public void SavePlayer(string playerName, Player player)
    {
        Player playerData = new Player(player);

        db.Collection("Jugadores").Document(playerName).SetAsync(playerData).ContinueWith(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log($"✅ Partida guardada correctamente como '{playerName}' en colección 'Jugadores'");
            }
            else
            {
                Debug.LogError($"❌ Error al guardar la partida: {task.Exception}");
            }
        });
    }

    public async Task<Player> LoadPlayer(string playerName)
    {
        try
        {
            DocumentReference docRef = db.Collection("Jugadores").Document(playerName);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Player player = snapshot.ConvertTo<Player>();
                Debug.Log($"✅ Datos del jugador '{playerName}' cargados correctamente.");
                return player;
            }
            else
            {
                Debug.LogWarning($"⚠️ No se encontró ningún documento para el jugador '{playerName}'.");
                return null;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error al cargar los datos del jugador: {e}");
            return null;
        }
    }


}
