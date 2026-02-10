using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CollectionButton : MonoBehaviour
{
    [Header("Animators")]
    [SerializeField] private Animator _animator;

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

    public void OpenCollectionMenu(Button button)
    {
        _CollectionPanel.SetActive(true);

        _animator.SetTrigger("OpenTrigger");
        StartCoroutine(_buttonFunctions.InteractibleButton(button, _animator));

        _buttonFunctions.OpenMenu();
    }

    public void CloseCollectionMenu(Button button)
    {
        _animator.SetTrigger("CloseTrigger");
        StartCoroutine(_buttonFunctions.CloseInteractibleButton(button, _animator, _CollectionPanel));

        Close();
    }
    private void Close()
    {

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
                _ImageCollection.sprite = Resources.Load<Sprite>("Images/Glovo");
                break; 
            default:
                _TitleText.text = "HarryxDraco";
                _SubtitleText.text = "Sex";
                _InfoText.text = "Draco mira fijamente a Harry y acerca lentamente sus labios a los suyos, Harry no se lo puede creer, y tampoco se puede detener";
                break;
        }
    }

}
