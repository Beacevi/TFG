using UnityEngine;

public class CustomMenu : MonoBehaviour
{
    [Header("Animators")]
    [SerializeField] private Animator _animator; 
    [SerializeField] private Animator _balloonAnimator; //>Animator del glovo

    [Header("Custom")]
    [SerializeField] private GameObject _TopPart;
    [SerializeField] private GameObject _MiddlePart;
    [SerializeField] private GameObject _BottomPart;
    [SerializeField] private GameObject _SupportPart;
    [SerializeField] private GameObject _TopCustom;
    [SerializeField] private GameObject _MiddleCustom;
    [SerializeField] private GameObject _BottomCustom;
    [SerializeField] private GameObject _SupportCustom;
    [SerializeField] private GameObject _CheckButtonCustom;

    [Header("Scripts")]
    private ButtonFunctions _buttonFunctions;

    [Header("Panels")]
    [SerializeField] private GameObject _CustomPanel;
    void Start()
    {
        _buttonFunctions = GetComponent<ButtonFunctions>();

        _CustomPanel.SetActive(false);
        _CheckButtonCustom.SetActive(false);
    }
    public void OpenCustomMenu()
    {
        _CustomPanel.SetActive(true);
        _balloonAnimator.SetTrigger("EditingTrigger");

        _buttonFunctions.OpenMenu();
    }

    public void CloseCustomMenu()
    {
        _CustomPanel.SetActive(false);

        _balloonAnimator.SetTrigger("NotEditingTrigger");

        _buttonFunctions.CloseMenu();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
