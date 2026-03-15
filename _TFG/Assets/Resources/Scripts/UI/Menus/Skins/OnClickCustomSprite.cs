using UnityEngine;

public class OnClickCustomSprite : MonoBehaviour
{
    [SerializeField] private CustomMenuSprites _canvasCustomSprites;

    // Cambiar el sprite hacia la izquierda
    public void OnButtonClickLeftChangeCustomSprite()
    {
        string buttonTag = gameObject.tag;
        _canvasCustomSprites.ChangePart(buttonTag, true);
    }

    // Cambiar el sprite hacia la derecha
    public void OnButtonClickRightChangeCustomSprite()
    {
        string buttonTag = gameObject.tag;
        _canvasCustomSprites.ChangePart(buttonTag, false);
    }
}
