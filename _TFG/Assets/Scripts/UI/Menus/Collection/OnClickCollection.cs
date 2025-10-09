using UnityEngine;

public class OnClickCollection : MonoBehaviour
{
    public CollectionButton _canvasManager;  

    public void OnButtonClick()
    {
        string buttonTag = gameObject.tag;

        _canvasManager.UpdateTextBasedOnTag(buttonTag);
    }
}
