/**
 * @file CSVReader.cs
 * @brief Descarga y procesa datos externos en formato CSV para convertirlos en estructuras utilizables por el juego.
 * @author Hortensia Studio - David Díaz Espinosa de los Monteros
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Modelo de datos serializable que representa la configuración de progresión de un nivel del globo.
/// </summary>
[System.Serializable]
public class GlobeLevel
{
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar level.
    /// </summary>
    public int level;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar upgrade cost.
    /// </summary>
    public int upgradeCost;
    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar biome.
    /// </summary>
    public string biome;
    /// <summary>
    /// Valor numérico que limita o define max e.
    /// </summary>
    public int maxE;
    /// <summary>
    /// Valor numérico que limita o define max character level.
    /// </summary>
    public int maxCharacterLevel;
}

/// <summary>
/// Descarga y procesa datos externos en formato CSV para convertirlos en estructuras utilizables por el juego.
/// </summary>
public class CSVReader : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar url.
    /// </summary>
    [SerializeField] private string url;

    /// <summary>
    /// Colección de levels utilizada por este componente.
    /// </summary>
    private List<GlobeLevel> levels = new List<GlobeLevel>();

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    void Start()
    {
        StartCoroutine(DownloadCSV());
    }

    /// <summary>
    /// Ejecuta la lógica asociada a download csv dentro de CSVReader.
    /// </summary>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    IEnumerator DownloadCSV()
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error descargando CSV: " + www.error);
        }
        else
        {
            ParseCSV(www.downloadHandler.text);
        }
    }

    /// <summary>
    /// Procesa csv y lo convierte en datos internos utilizables.
    /// </summary>
    /// <param name="data">Datos de entrada que se van a procesar.</param>
    void ParseCSV(string data)
    {
        string[] lines = data.Split('\n');

        for (int i = 1; i < lines.Length; i++) // saltar header
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string cleanLine = lines[i].Trim();

            // detectar separador
            char separator = cleanLine.Contains("\t") ? '\t' : ',';

            string[] columns = cleanLine.Split(separator);

            if (columns.Length < 5)
            {
                Debug.LogWarning("L�nea inv�lida: " + cleanLine);
                continue;
            }

            GlobeLevel levelData = new GlobeLevel();

            int.TryParse(CleanNumber(columns[0]), out levelData.level);
            int.TryParse(CleanNumber(columns[1]), out levelData.upgradeCost);

            levelData.biome = columns[2].Trim();

            int.TryParse(CleanNumber(columns[3]), out levelData.maxE);
            int.TryParse(CleanNumber(columns[4]), out levelData.maxCharacterLevel);

            levels.Add(levelData);
        }

        Debug.Log("CSV cargado correctamente. Niveles: " + levels.Count);
    }

    /// <summary>
    /// Limpia o normaliza number antes de utilizarlo.
    /// </summary>
    /// <param name="value">Parámetro value empleado por el método.</param>
    /// <returns>Texto resultante de la consulta o procesamiento.</returns>
    string CleanNumber(string value)
    {
        return value.Replace(".", "").Replace("\r", "").Trim();
    }

    /// <summary>
    /// Obtiene level a partir del estado actual del sistema.
    /// </summary>
    /// <param name="level">Nivel que se utilizará como referencia.</param>
    /// <returns>Instancia o valor de tipo GlobeLevel resultante de la operación.</returns>
    public GlobeLevel GetLevel(int level)
    {
        if (level - 1 < 0 || level - 1 >= levels.Count) return null;
        return levels[level - 1];
    }

    /// <summary>
    /// Obtiene upgrade cost a partir del estado actual del sistema.
    /// </summary>
    /// <param name="level">Nivel que se utilizará como referencia.</param>
    /// <returns>Valor numérico calculado o consultado por el método.</returns>
    public int GetUpgradeCost(int level)
    {
        return GetLevel(level)?.upgradeCost ?? 0;
    }

    /// <summary>
    /// Obtiene biome a partir del estado actual del sistema.
    /// </summary>
    /// <param name="level">Nivel que se utilizará como referencia.</param>
    /// <returns>Texto resultante de la consulta o procesamiento.</returns>
    public string GetBiome(int level)
    {
        return GetLevel(level)?.biome;
    }

    /// <summary>
    /// Obtiene all upgrade costs a partir del estado actual del sistema.
    /// </summary>
    /// <returns>Instancia o valor de tipo int[] resultante de la operación.</returns>
    public int[] GetAllUpgradeCosts()
    {
        int[] result = new int[levels.Count];

        for (int i = 0; i < levels.Count; i++)
            result[i] = levels[i].upgradeCost;

        return result;
    }

    /// <summary>
    /// Obtiene all biomes a partir del estado actual del sistema.
    /// </summary>
    /// <returns>Instancia o valor de tipo string[] resultante de la operación.</returns>
    public string[] GetAllBiomes()
    {
        string[] result = new string[levels.Count];

        for (int i = 0; i < levels.Count; i++)
            result[i] = levels[i].biome;

        return result;
    }
}
/* EJEMPLO:
/// <summary>
/// Controlador persistente principal que mantiene el estado global del jugador, la economía, el guardado y la interfaz.
/// </summary>
public class GameManager : MonoBehaviour
{
    public CSVReader reader;

    void Start()
    {
        int cost = reader.GetUpgradeCost(3);
        Debug.Log("Coste nivel 3: " + cost);

        string biome = reader.GetBiome(3);
        Debug.Log("Bioma nivel 3: " + biome);

        int[] allCosts = reader.GetAllUpgradeCosts();
    }
}
 */ 
