/**
 * @file TileClickDetector.cs
 * @brief Detecta la interacción del cursor o toque sobre tiles del mapa procedural.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

/// <summary>
/// Detecta la interacción del cursor o toque sobre tiles del mapa procedural.
/// </summary>
public class TileHoverDetector : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo Tilemap utilizado para almacenar o configurar main tilemap.
    /// </summary>
    [SerializeField] Tilemap mainTilemap;
    /// <summary>
    /// Campo de tipo Tilemap utilizado para almacenar o configurar highlight tilemap.
    /// </summary>
    [SerializeField] Tilemap highlightTilemap;
    /// <summary>
    /// Campo de tipo Tile utilizado para almacenar o configurar highlight tile.
    /// </summary>
    [SerializeField] Tile highlightTile;
    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar silhouette.
    /// </summary>
    public GameObject silhouette;
    /// <summary>
    /// Referencia visual o sprite asociado a normal sprite.
    /// </summary>
    public GameObject normalSprite;

    /// <summary>
    /// Campo de tipo Vector3Int utilizado para almacenar o configurar previous cell.
    /// </summary>
    private Vector3Int previousCell = new Vector3Int(int.MinValue, int.MinValue, 0);

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    private void Start()
    {
        Cursor.visible = false;
        if (!mainTilemap)
            mainTilemap = GameObject.FindGameObjectWithTag("MainTileMap").GetComponent<Tilemap>();
        if (!highlightTilemap)
            highlightTilemap = GameObject.FindGameObjectWithTag("HighlightTileMap").GetComponent<Tilemap>();
    }

    /// <summary>
    /// Actualiza la lógica del componente en cada fotograma.
    /// </summary>
    void Update()
    {
        if (!IsometricCamera.inputEnabled)
            return;

        if (SimonGameManagerPajaro.IsActive)
            return;

        if (Camera.main == null)
            return;

        if (Input.touchCount == 0 && !Input.GetMouseButton(0))
            return;

        Vector3 screenPos;

        if (Input.touchCount > 0)
            screenPos = Input.GetTouch(0).position;
        else
            screenPos = Input.mousePosition;

        if (float.IsNaN(screenPos.x) || float.IsNaN(screenPos.y))
            return;

        if (float.IsInfinity(screenPos.x) || float.IsInfinity(screenPos.y))
            return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(screenPos);
        mouseWorld.z = 0f;

        Vector3Int cellPos = mainTilemap.WorldToCell(mouseWorld);

        if (cellPos != previousCell)
        {
            highlightTilemap.SetTile(previousCell, null);

            if (mainTilemap.HasTile(cellPos))
            {
                highlightTilemap.SetTile(cellPos, highlightTile);

                Matrix4x4 m = mainTilemap.GetTransformMatrix(cellPos);
                highlightTilemap.SetTransformMatrix(cellPos, m);

                previousCell = cellPos;
            }
        }
    }



    /// <summary>
    /// Procesa la entrada de otro collider 2D en el área de detección.
    /// </summary>
    /// <param name="col">Parámetro col empleado por el método.</param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Tree"))
        {
            silhouette.SetActive(true);
            normalSprite.SetActive(false);
        }
    }

    /// <summary>
    /// Procesa la salida de otro collider 2D del área de detección.
    /// </summary>
    /// <param name="col">Parámetro col empleado por el método.</param>
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Tree"))
        {
            silhouette.SetActive(false);
            normalSprite.SetActive(true);
        }
    }

}
