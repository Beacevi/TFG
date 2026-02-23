using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StickerPlacer : MonoBehaviour
{
    public List<StickerData> stickers;

    public GameObject stickerButtonPrefab;
    public Transform gridParent;

    public RectTransform blackboard;
    public GameObject placedStickerPrefab;

    private StickerData selectedSticker;
    private List<StickerButtonUI> buttonUIs = new List<StickerButtonUI>();

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        foreach (StickerData data in stickers)
        {
            GameObject obj = Instantiate(stickerButtonPrefab, gridParent);
            StickerButtonUI ui = obj.GetComponent<StickerButtonUI>();
            ui.Setup(data, this);
            buttonUIs.Add(ui);

        }
    }

    public void SelectSticker(StickerData data)
    {
        selectedSticker = data;

        // Disable all buttons
        foreach (var btn in buttonUIs)
            btn.button.interactable = false;
    }

    /*void Update()
    {
        if (selectedSticker == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 localPoint;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                blackboard,
                Input.mousePosition,
                null,
                out localPoint))
            {
                PlaceSticker(localPoint);
            }
        }
    }*/

    void Update()
    {
        if (selectedSticker == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            // First check: is the click inside the Blackboard rectangle?
            if (!RectTransformUtility.RectangleContainsScreenPoint(
                blackboard,
                Input.mousePosition))
            {
                return; // Click was outside → ignore
            }

            Vector2 localPoint;

            // Convert screen point to local position inside Blackboard
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                blackboard,
                Input.mousePosition,
                null,
                out localPoint);

            PlaceSticker(localPoint);
        }
    }

    void PlaceSticker(Vector2 position)
    {
        GameObject obj = Instantiate(placedStickerPrefab, blackboard);
        obj.GetComponent<Image>().sprite = selectedSticker.sprite;
        obj.GetComponent<RectTransform>().anchoredPosition = position;

        selectedSticker.UseOne();

        selectedSticker = null;

        // Re-enable buttons
        foreach (var btn in buttonUIs)
            btn.UpdateState();
    }
}