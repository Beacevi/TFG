using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TileHoverDetector : MonoBehaviour
{
    [SerializeField] Tilemap mainTilemap;
    [SerializeField] Tilemap highlightTilemap;
    [SerializeField] Tile highlightTile;

    private Vector3Int previousCell = new Vector3Int(int.MinValue, int.MinValue, 0);

    private void Start()
    {
        Cursor.visible = false;
        if (!mainTilemap)
            mainTilemap = GameObject.FindGameObjectWithTag("MainTileMap").GetComponent<Tilemap>();
        if (!highlightTilemap)
            highlightTilemap = GameObject.FindGameObjectWithTag("HighlightTileMap").GetComponent<Tilemap>();
    }

    void Update()
    {
        if (!IsometricCamera.inputEnabled)
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
}
