using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class CSVReader : MonoBehaviour
{
    //URL 
    [SerializeField] private string url;
    int[] niveles;
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
        string[] lines = data.Split('\n');

        //porque no vamos a usar la cabecera
        niveles = new int[lines.Length - 1];

        int indice = 0;

        // empezamos en 1 para saltar la cabecera
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];

            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] columns = line.Split('\t');

            //niveles[indice] = int.Parse(columns[1]);

            Debug.Log("Exp Siguiente Nivel: " + columns[1]);
            //Debug.Log("Max Nivel Pj: " + columns[columns.Length - 1]);

            indice++;
        }

        Debug.Log("CSV cargado con éxito");
    }
}
