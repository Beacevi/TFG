using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CollectionButton : MonoBehaviour
{
    [Header("Scripts")]
    private ButtonFunctions  _buttonFunctions;

    [Header("Panels")]
    [SerializeField] private GameObject _CollectionPanel;
    [SerializeField] private GameObject _AllCollection;
    [SerializeField] private GameObject _InfoCollection;
    [SerializeField] private GameObject _ButtonCloseCollection; //>Boton que cierra el panel de collection

    [Header("InfoCollectionsObjects")]
    [SerializeField] private TMP_Text _TitleText;
    [SerializeField] private TMP_Text _SubtitleText;
    [SerializeField] private TMP_Text _InfoText;
    [SerializeField] private Image _ImageCollection;

    private void Start()
    {
        _buttonFunctions = GetComponent<ButtonFunctions>();

        _CollectionPanel.SetActive(false);
        _InfoCollection.SetActive(false);
    }

    public void OpenCollectionMenu()
    {
        _CollectionPanel.SetActive(true);

        _buttonFunctions.OpenMenu();
    }

    public void CloseCollectionMenu()
    {
        _CollectionPanel.SetActive(false);

        _buttonFunctions.CloseMenu();
    }

    public void OpenInfo() ///Con un sistema de tags se cambia el texto de la carta
    {
        _AllCollection.SetActive(false);
        _ButtonCloseCollection.SetActive(false);

        _InfoCollection.SetActive(true);
    }
    public void CloseInfo()
    {
        _AllCollection.SetActive(true);
        _ButtonCloseCollection.SetActive(true);

        _InfoCollection.SetActive(false);
    }
    public void UpdateTextBasedOnTag(string buttonTag)
    {
        switch (buttonTag)
        {
            case "Untagged":
                _TitleText.text = "ShortKing";
                _SubtitleText.text = "Dobby";
                _InfoText.text = "jhvjjkuiugc";
                break; 
            default:
                _TitleText.text = "HarryxDraco";
                _SubtitleText.text = "Sex";
                _InfoText.text = "Draco mira fijamente a Harry y acerca lentamente sus labios a los suyos, Harry no se lo puede creer, y tampoco se puede detener";
                break;
        }
    }

}
