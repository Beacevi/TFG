using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class OnClickCollection : MonoBehaviour
{
    [SerializeField] private CollectionButton _canvasManagerCollection;
    [SerializeField] private BirdButton       _canvasManagerBird;

    private Image _myButtonImage;
    private TMP_Text _myButtonText;
    public void OnButtonClickCollection()
    {
        string buttonTag = gameObject.tag;

        _canvasManagerCollection.UpdateTextBasedOnTag(buttonTag);
    }
    public void OnButtonClickBird()
    {
        string buttonTag = gameObject.tag;

        _myButtonImage = gameObject.GetComponent<Image>();
        _myButtonText  = gameObject.GetComponentInParent<TMP_Text>();

        _canvasManagerBird.UpdateTextBasedOnTag(buttonTag, _myButtonImage, _myButtonText);
    }

    public void OnButtonClickTakeBirdOff()
    {
        string buttonTag = gameObject.tag;
        _canvasManagerBird.ClearOneTag(buttonTag);
    }
    
}
