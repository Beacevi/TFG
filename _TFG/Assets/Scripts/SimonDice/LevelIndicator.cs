using UnityEngine;
using UnityEngine.UI;

public class LevelIndicator : MonoBehaviour
{
    public SpriteRenderer[] dots;

    public void SetLevelSuccess(int level)
    {
        Color sucessColor = new Color32(103, 80, 164, 255);
        if (level < dots.Length)
            dots[level].color = sucessColor;
    }

    public void SetLevelFail(int level)
    {
        Color failColor = new Color32(179, 38, 30, 255);
        if (level < dots.Length)
            dots[level].color = failColor;
    }

    public void ResetIndicators()
    {
        Color resetColor = new Color32(232, 221, 255, 255);
        foreach (var dot in dots)
            dot.color = resetColor;
    }
}
