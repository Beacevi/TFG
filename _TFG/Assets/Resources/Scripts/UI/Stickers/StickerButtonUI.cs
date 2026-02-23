using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StickerButtonUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI amountText;
    public Button button;

    private StickerData stickerData;
    private StickerPlacer placer;

    public void Setup(StickerData data, StickerPlacer stickerPlacer)
    {
        stickerData = data;
        placer = stickerPlacer;

        icon.sprite = data.sprite;
        amountText.text = data.amount.ToString();

        button.onClick.AddListener(OnClicked);

        UpdateState();

        if (icon == null) Debug.LogError("Icon not assigned!");
        if (amountText == null) Debug.LogError("AmountText not assigned!");
        if (button == null) Debug.LogError("Button not assigned!");
    }

    void OnClicked()
    {
        if (!stickerData.CanUse())
            return;

        placer.SelectSticker(stickerData);
    }

    public void UpdateState()
    {
        amountText.text = stickerData.amount.ToString();
        button.interactable = stickerData.CanUse();
    }
}