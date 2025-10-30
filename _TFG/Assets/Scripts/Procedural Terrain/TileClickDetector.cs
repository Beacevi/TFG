using UnityEngine;
using UnityEngine.Tilemaps;

public class TileHoverDetector : MonoBehaviour
{
    public Tilemap mainTilemap;
    public Tilemap highlightTilemap;
    public Tile highlightTile;

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
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
