/**
 * @file GoogleSheetsForUnity.cs
 * @brief Obtiene información publicada desde Google Sheets y la adapta a estructuras de datos consumibles por Unity.
 * @author Hortensia Studio - David Díaz Espinosa de los Monteros
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using Google.Apis.Services;
using System;
using Google.Apis.Sheets.v4.Data;
using System.IO;
using System.Threading.Tasks;

/// <summary>
/// Obtiene información publicada desde Google Sheets y la adapta a estructuras de datos consumibles por Unity.
/// </summary>
public class GoogleSheetsForUnity : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar spread sheet id.
    /// </summary>
    [Header("GoogleSheets Information")]
    [SerializeField] private string spreadSheetID;
    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar sheet id.
    /// </summary>
    [SerializeField] private string sheetID;

    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar get data in range.
    /// </summary>
    [Header("Data from GoogleSheets")]
    [SerializeField] private string getDataInRange;

    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar service account email.
    /// </summary>
    private string serviceAccountEmail = "";
    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar certificate name.
    /// </summary>
    private string certificateName = "";
    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar certificate path.
    /// </summary>
    private string certificatePath;

    /// <summary>
    /// Campo de tipo SheetsService utilizado para almacenar o configurar google sheets service.
    /// </summary>
    private static SheetsService googleSheetsService;
    /// <summary>
    /// Representa una fila de datos procedente de una hoja externa.
    /// </summary>
    [Serializable]
    public class Row
    {
        public List<string> cellData = new List<string>();
    }
    /// <summary>
    /// Contenedor serializable de filas utilizado para deserializar respuestas externas.
    /// </summary>
    [Serializable]
    public class RowList
    {
        public List<Row> rows = new List<Row>();
    }

    /// <summary>
    /// Campo de tipo RowList utilizado para almacenar o configurar data from google sheets.
    /// </summary>
    public RowList DataFromGoogleSheets = new RowList();

    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar write data in range.
    /// </summary>
    [Header("Write Data From Unity")]
    [SerializeField] private string writeDataInRange;

    /// <summary>
    /// Campo de tipo RowList utilizado para almacenar o configurar write data from unity.
    /// </summary>
    public RowList WriteDataFromUnity = new RowList();

    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar delete data in range.
    /// </summary>
    [Header("Delete Data In GoogleSheets")]
    [SerializeField] private string deleteDataInRange;


    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    void Start()
    {
        //Remove comment to use on Android

/*        string tempPath = Path.Combine(Application.streamingAssetsPath, certificateName);
        WWW reader = new WWW(tempPath);
        while (!reader.isDone) { }
        certificatePath = Application.persistentDataPath + "/db";
        File.WriteAllBytes(certificatePath, reader.bytes);*/


        certificatePath = Application.dataPath + "/StreamingAssets/" + certificateName;  //Comment to use on Android

        var certificate = new X509Certificate2(certificatePath, "notasecret", X509KeyStorageFlags.Exportable);

        ServiceAccountCredential credential = new ServiceAccountCredential(
            new ServiceAccountCredential.Initializer(serviceAccountEmail)
            {
                Scopes = new[] { SheetsService.Scope.Spreadsheets }
            }.FromCertificate(certificate));

        googleSheetsService = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "GoogleSheets API for Unity"
        });

        //Use async methods to increase Android performance.
    }

    /// <summary>
    /// Ejecuta la lógica asociada a read data dentro de RowList.
    /// </summary>
    public void ReadData()
    {
        string range = sheetID + "!" + getDataInRange;

        var request = googleSheetsService.Spreadsheets.Values.Get(spreadSheetID, range);
        var reponse = request.Execute();
        var values = reponse.Values;
        if (values != null && values.Count > 0)
        {
            foreach (var row in values)
            {
                Row newRow = new Row();
                DataFromGoogleSheets.rows.Add(newRow);
                foreach (var value in row)
                {
                    newRow.cellData.Add(value.ToString());
                    Debug.Log(value.ToString());
                }

            }
        }
    }

    /// <summary>
    /// Ejecuta la lógica asociada a read data asyn dentro de RowList.
    /// </summary>
    public async void ReadDataAsyn()
    {
        var task = await Task.Run(() =>
        {
            string range = sheetID + "!" + getDataInRange;

            var request = googleSheetsService.Spreadsheets.Values.Get(spreadSheetID, range);
            var reponse = request.Execute();
            var values = reponse.Values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    Row newRow = new Row();
                    DataFromGoogleSheets.rows.Add(newRow);
                    foreach (var value in row)
                    {
                        newRow.cellData.Add(value.ToString());
                    }

                }
            }
            return 0;
        });
    }

    /// <summary>
    /// Ejecuta la lógica asociada a write data dentro de RowList.
    /// </summary>
    public void WriteData()
    {
        string range = sheetID + "!" + writeDataInRange;
        var valueRange = new ValueRange();
        var cellData = new List<object>();
        var arrows = new List<IList<object>>();
        foreach (var row in WriteDataFromUnity.rows)
        {
            cellData = new List<object>();
            foreach (var data in row.cellData)
            {
                cellData.Add(data);
            }

            arrows.Add(cellData);
        }

        valueRange.Values = arrows;

        var request = googleSheetsService.Spreadsheets.Values.Append(valueRange, spreadSheetID, range);
        request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
        var reponse = request.Execute();
    }

    /// <summary>
    /// Ejecuta la lógica asociada a write data asyn dentro de RowList.
    /// </summary>
    public async void WriteDataAsyn()
    {
        var task = await Task.Run(() =>
        {
            string range = sheetID + "!" + writeDataInRange;
            var valueRange = new ValueRange();
            var cellData = new List<object>();
            var arrows = new List<IList<object>>();
            foreach (var row in WriteDataFromUnity.rows)
            {
                cellData = new List<object>();
                foreach (var data in row.cellData)
                {
                    cellData.Add(data);
                }

                arrows.Add(cellData);
            }

            valueRange.Values = arrows;

            var request = googleSheetsService.Spreadsheets.Values.Append(valueRange, spreadSheetID, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var reponse = request.Execute();
            return 0;
        });
    }

    /// <summary>
    /// Ejecuta la lógica asociada a delete data dentro de RowList.
    /// </summary>
    public void DeleteData()
    {
        var range = sheetID + "!" + deleteDataInRange;

        var deleteData = googleSheetsService.Spreadsheets.Values.Clear(new ClearValuesRequest(), spreadSheetID, range);
        deleteData.Execute();
    }

    /// <summary>
    /// Ejecuta la lógica asociada a delete data asyn dentro de RowList.
    /// </summary>
    public async void DeleteDataAsyn()
    {
        var task = await Task.Run(() =>
        {
            var range = sheetID + "!" + deleteDataInRange;

            var deleteData = googleSheetsService.Spreadsheets.Values.Clear(new ClearValuesRequest(), spreadSheetID, range);
            deleteData.Execute();
            return 0;
        });
    }
    
    
    /// <summary>
    /// Actualiza data para reflejar el estado actual del sistema.
    /// </summary>
    public void UpdateData()
    {
        string range = sheetID + "!" + writeDataInRange;
        var valueRange = new ValueRange();
        var cellData = new List<object>();
        var arrows = new List<IList<object>>();
        foreach (var row in WriteDataFromUnity.rows)
        {
            cellData = new List<object>();
            foreach (var data in row.cellData)
            {
                cellData.Add(data);
            }

            arrows.Add(cellData);
        }

        valueRange.Values = arrows;

        var updateRequest = googleSheetsService.Spreadsheets.Values.Update(valueRange, spreadSheetID, range);
        updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
        var appendReponse = updateRequest.Execute();
    }

    /// <summary>
    /// Actualiza data asyn para reflejar el estado actual del sistema.
    /// </summary>
    public async void UpdateDataAsyn()
    {
        var task = await Task.Run(() =>
        {
            string range = sheetID + "!" + writeDataInRange;
            var valueRange = new ValueRange();
            var cellData = new List<object>();
            var arrows = new List<IList<object>>();
            foreach (var row in WriteDataFromUnity.rows)
            {
                cellData = new List<object>();
                foreach (var data in row.cellData)
                {
                    cellData.Add(data);
                }

                arrows.Add(cellData);
            }

            valueRange.Values = arrows;

            var updateRequest = googleSheetsService.Spreadsheets.Values.Update(valueRange, spreadSheetID, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var appendReponse = updateRequest.Execute();
            return 0;
        });
    }


}
