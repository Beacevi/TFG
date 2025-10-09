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
            case "Gorrion":
                _TitleText.text = "Gorrion";
                _SubtitleText.text = "11/10/2026";
                _InfoText.text = "Puta cerda de mierda, lesbiana, fea, guarra y ortera";
                _ImageCollection.sprite = Resources.Load<Sprite>("UI/Buttons/Birds/Bird");
                break;
            case "Avestruz":
                _TitleText.text = "Avestruz";
                _SubtitleText.text = "04/08/2013";
                _InfoText.text = "Te voy a dar lo tuyyo nena";
                _ImageCollection.sprite = Resources.Load<Sprite>("Glovo");
                break;
            case "Untagged":
                _TitleText.text = "Title";
                _SubtitleText.text = "Subtitle";
                _InfoText.text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor";
                break; 
            default:
                _TitleText.text = "Title";
                break;
        }
    }

}
