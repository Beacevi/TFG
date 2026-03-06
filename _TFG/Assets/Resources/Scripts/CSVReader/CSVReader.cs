using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class CSVReader : MonoBehaviour
{
    [SerializeField] private string url;

    [System.Serializable]
    public class BalloonLevel
    {
        public int level;
        public int upgradeCost;
        public string biome;
        public int maxE;
        public int maxCharacterLevel;
    }

    private List<BalloonLevel> levels = new List<BalloonLevel>();

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

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] columns = lines[i].Split('\t');

            BalloonLevel level = new BalloonLevel();

            level.level = int.Parse(columns[0]);
            level.upgradeCost = int.Parse(columns[1]);
            level.biome = columns[2];
            level.maxE = int.Parse(columns[3]);
            level.maxCharacterLevel = int.Parse(columns[4]);

            levels.Add(level);
        }

        Debug.Log("CSV cargado con éxito");
    }

    // GETTERS

    public BalloonLevel GetLevel(int level)
    {
        return levels[level - 1];
    }

    public int[] GetUpgradeCosts()
    {
        int[] result = new int[levels.Count];

        for (int i = 0; i < levels.Count; i++)
            result[i] = levels[i].upgradeCost;

        return result;
    }

    public string[] GetBiomes()
    {
        string[] result = new string[levels.Count];

        for (int i = 0; i < levels.Count; i++)
            result[i] = levels[i].biome;

        return result;
    }

    public int GetCostByLevel(int level)
    {
        return levels[level - 1].upgradeCost;
    }

    public string GetBiomeByLevel(int level)
    {
        return levels[level - 1].biome;
    }
}
/* HOLA BEA SI QUIERES LLAMARLO DESDE OTRO SCRIPT, HAZLO ASÍ:
 * public class GameManager : MonoBehaviour
{
    public CSVReader reader;

    void Start()
    {
        int cost = reader.GetCostByLevel(3);
        Debug.Log("Coste nivel 3: " + cost);

        string biome = reader.GetBiomeByLevel(3);
        Debug.Log("Bioma nivel 3: " + biome);
    }
}
SI NO TE GUSTA TE JODES, DIGOOO...SI NO TE GUSTA ME DICES QUÉ QUIERES CAMBIAR
 */ 