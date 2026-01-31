using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class CSVReader : MonoBehaviour
{
    //URL 
    [SerializeField] private string url;

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
            string csvData = www.downloadHandler.text;
            ParseCSV(csvData);
        }
    }

    void ParseCSV(string data)
    {
        // Dividir el CSV por líneas
        string[] lines = data.Split('\n');

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Dividir cada línea por comas o tabulación
            string[] columns = line.Split('\t');

            //  Imprimir x columna
            Debug.Log("Dato: " + columns[1]);
        }
        Debug.Log("CSV cargado con éxito");
    }
}
