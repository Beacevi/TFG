using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveDataManager
{
    public int coins;
    public int gems;
    public int energy;
    public int currentLevel;
    public int balloonLevel;

    public List<ColorItem> shopItems;
    public List<Color32> unlockedColors;
}