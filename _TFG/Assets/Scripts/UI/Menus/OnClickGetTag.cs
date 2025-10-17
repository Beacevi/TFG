using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

        Image _myButtonImage = gameObject.GetComponent<Image>();

        Transform grandparentTransform = gameObject.transform.parent.parent;
        string _myButtonTitle = grandparentTransform.GetComponent<TMP_Text>().text;

        Transform greatgrandparentTransform = gameObject.transform.parent.parent.parent;
        string _myButtonUpdate = greatgrandparentTransform.GetComponent<TMP_Text>().text;

        Image fatherPanel = gameObject.transform.parent.GetComponent<Image>();
        Color selectedColor = new Color32(158, 137, 171, 255);

        if (_canvasManagerBird.BirdSelected.Count == 3)
        {
            fatherPanel.color = Color.white;
        }
        else
        {
            fatherPanel.color = selectedColor;
        }

        _canvasManagerBird.UpdateTextBasedOnTag(buttonTag, _myButtonImage, _myButtonTitle, _myButtonUpdate);
    }

    public void OnButtonClickTakeBirdOff()
    {
        string buttonTag = gameObject.tag;
        _canvasManagerBird.ClearOneTag(buttonTag);
    }
    
}
