using UnityEngine;

public class OnClickCollection : MonoBehaviour
{
    [SerializeField] private CollectionButton _canvasManagerCollection;
    [SerializeField] private BirdButton       _canvasManagerBird;

    public void OnButtonClickCollection()
    {
        string buttonTag = gameObject.tag;

        _canvasManagerCollection.UpdateTextBasedOnTag(buttonTag);
    }
    public void OnButtonClickBird()
    {
        string buttonTag = gameObject.tag;
        //Sprite image = gameObject.Sprite;

        _canvasManagerBird.UpdateTextBasedOnTag(buttonTag);
    }
}
