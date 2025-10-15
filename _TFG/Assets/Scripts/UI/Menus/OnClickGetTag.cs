using UnityEngine;
using UnityEngine.UI;
public class OnClickCollection : MonoBehaviour
{
    [SerializeField] private CollectionButton _canvasManagerCollection;
    [SerializeField] private BirdButton       _canvasManagerBird;

    private Image myButtonImage;

    public void OnButtonClickCollection()
    {
        string buttonTag = gameObject.tag;

        _canvasManagerCollection.UpdateTextBasedOnTag(buttonTag);
    }
    public void OnButtonClickBird()
    {
        string buttonTag = gameObject.tag;

        myButtonImage = gameObject.GetComponent<Image>();

        _canvasManagerBird.UpdateTextBasedOnTag(buttonTag, myButtonImage);
    }
}
