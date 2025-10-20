using UnityEngine;
using UnityEngine.UI;

public class LevelIndicator : MonoBehaviour
{
    public SpriteRenderer[] dots;

    public void SetLevelSuccess(int level)
    {
        if (level < dots.Length)
            dots[level].color = Color.green;
    }

    public void SetLevelFail(int level)
    {
        if (level < dots.Length)
            dots[level].color = Color.red;
    }

    public void ResetIndicators()
    {
        foreach (var dot in dots)
            dot.color = Color.gray;
    }
}
