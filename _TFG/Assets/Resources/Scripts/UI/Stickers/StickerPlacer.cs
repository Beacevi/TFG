using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StickerPlacer : MonoBehaviour
{
    public List<StickerData> stickers;

    public GameObject stickerButtonPrefab;
    public Transform gridParent;

    public RectTransform blackboard;
    public GameObject placedStickerPrefab;

    private StickerData selectedSticker;
    private List<StickerButtonUI> buttonUIs = new List<StickerButtonUI>();

    [Header("Delete Mode")]
    public bool deleteMode = false;

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
        /*if (EventSystem.current.IsPointerOverGameObject())
            return;*/
        
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

    /*void PlaceSticker(Vector2 position)
    {
        GameObject obj = Instantiate(placedStickerPrefab, blackboard);
        obj.GetComponent<Image>().sprite = selectedSticker.sprite;
        obj.GetComponent<RectTransform>().anchoredPosition = position;

        selectedSticker.UseOne();

        selectedSticker = null;

        // Re-enable buttons
        foreach (var btn in buttonUIs)
            btn.UpdateState();
    }*/

    void PlaceSticker(Vector2 position)
    {
        GameObject obj = Instantiate(placedStickerPrefab, blackboard);

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = position;

        PlacedStickerUI placed = obj.GetComponent<PlacedStickerUI>();
        placed.Setup(selectedSticker, this);

        selectedSticker.UseOne();

        selectedSticker = null;

        foreach (var btn in buttonUIs)
            btn.UpdateState();
    }

    public void ToggleDeleteMode()
    {
        deleteMode = !deleteMode;

        // Cancel placement if entering delete mode
        if (deleteMode)
            selectedSticker = null;

        Debug.Log("Delete Mode: " + deleteMode);
    }

    public bool IsDeleteMode()
    {
        return deleteMode;
    }

    public void DeleteSticker(PlacedStickerUI sticker)
    {
        StickerData data = sticker.GetData();

        // Return sticker to inventory
        data.AddAmount(1);

        Destroy(sticker.gameObject);

        foreach (var btn in buttonUIs)
            btn.UpdateState();
    }

    public void ExitDeleteMode()
    {
        deleteMode = false;
    }
}