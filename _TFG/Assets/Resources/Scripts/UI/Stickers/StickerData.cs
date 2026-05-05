using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSticker", menuName = "Stickers/Sticker")]
public class StickerData : ScriptableObject
{
    public string stickerName;

    public Sprite sprite;
    public Sprite unknownSprite;

    public int amount = 0;
    public bool discovered = false;
}