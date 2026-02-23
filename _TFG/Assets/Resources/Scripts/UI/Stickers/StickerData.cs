using UnityEngine;

[CreateAssetMenu(fileName = "NewSticker", menuName = "Stickers/Sticker")]
public class StickerData : ScriptableObject
{
    public string stickerName;
    public Sprite sprite;

    public int amount = 0;

    public bool discovered = false;

    public bool CanUse()
    {
        return discovered && amount > 0;
    }

    public void AddAmount(int value)
    {
        amount += value;

        if (amount > 0)
            discovered = true;
    }

    public void UseOne()
    {
        if (amount > 0)
            amount--;
    }
}