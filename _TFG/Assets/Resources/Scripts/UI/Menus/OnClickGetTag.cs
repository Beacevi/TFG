using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class OnClickCollection : MonoBehaviour
{
    [SerializeField] private CollectionButton _canvasManagerCollection;
    [SerializeField] private BirdButton       _canvasManagerBird;
    [SerializeField] private CustomMenu       _canvasCustom;

    public void OnButtonClickCollection()
    {
        string buttonTag = gameObject.tag;

        _canvasManagerCollection.UpdateTextBasedOnTag(buttonTag);
    }

    public void OnButtonClickBird()
    {
        string buttonTag = gameObject.tag;

        Image _myButtonImage = gameObject.GetComponent<Image>();


        string _myButtonUpdate = "";


        string _myButtonTitle = gameObject.GetComponentInChildren<TMP_Text>().text;

        Slider _myButtonLevel = gameObject.GetComponentInChildren<Slider>();

        Image fatherPanel = gameObject.transform.parent.GetComponent<Image>();
        

        _canvasManagerBird.UpdateTextBasedOnTag(buttonTag, _myButtonImage, _myButtonTitle, _myButtonUpdate, fatherPanel);
    }

    public void OnButtonClickTakeBirdOff()
    {
        string buttonTag = gameObject.tag;
        _canvasManagerBird.ClearOneTag(buttonTag);
    }

    public void OnButtonClickLeftChangeCustom()
    {
        string buttonTag = gameObject.tag;

        _canvasCustom.ChangeColor(buttonTag, true);   
    }
    public void OnButtonClickRightChangeCustom()
    {
        string buttonTag = gameObject.tag;

        _canvasCustom.ChangeColor(buttonTag, false);
    }

}
