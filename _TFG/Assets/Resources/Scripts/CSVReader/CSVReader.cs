using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GlobeLevel
{
    public int level;
    public int upgradeCost;
    public string biome;
    public int maxE;
    public int maxCharacterLevel;
}

public class CSVReader : MonoBehaviour
{
    [SerializeField] private string url;

    private List<GlobeLevel> levels = new List<GlobeLevel>();

    void Start()
    {
        StartCoroutine(DownloadCSV());
    }

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
                Debug.LogWarning("Línea inválida: " + cleanLine);
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

    string CleanNumber(string value)
    {
        return value.Replace(".", "").Replace("\r", "").Trim();
    }

    public GlobeLevel GetLevel(int level)
    {
        if (level - 1 < 0 || level - 1 >= levels.Count) return null;
        return levels[level - 1];
    }

    public int GetUpgradeCost(int level)
    {
        return GetLevel(level)?.upgradeCost ?? 0;
    }

    public string GetBiome(int level)
    {
        return GetLevel(level)?.biome;
    }

    public int[] GetAllUpgradeCosts()
    {
        int[] result = new int[levels.Count];

        for (int i = 0; i < levels.Count; i++)
            result[i] = levels[i].upgradeCost;

        return result;
    }

    public string[] GetAllBiomes()
    {
        string[] result = new string[levels.Count];

        for (int i = 0; i < levels.Count; i++)
            result[i] = levels[i].biome;

        return result;
    }
}
/* EJEMPLO:
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