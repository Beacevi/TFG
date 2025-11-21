using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

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

    private bool check = false;
    private Color _actualColorTop;
    private Color _actualColorMiddle;
    private Color _actualColorBottom;
    private Color _actualColorSupport;
    void Start()
    {
        _buttonFunctions = GetComponent<ButtonFunctions>();

        _CustomPanel.SetActive(false);
        _CheckButtonCustom.SetActive(false);

        _actualColorTop     = _TopPart.GetComponent<SpriteRenderer>().color;
        _actualColorMiddle  = _MiddlePart.GetComponent<SpriteRenderer>().color;
        _actualColorBottom  = _BottomPart.GetComponent<SpriteRenderer>().color;
        _actualColorSupport = _SupportPart.GetComponent<SpriteRenderer>().color;
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

        if(!check)
        {
            _TopPart.GetComponent<SpriteRenderer>().color = _actualColorTop;
            _MiddlePart.GetComponent<SpriteRenderer>().color = _actualColorMiddle;
            _BottomPart.GetComponent<SpriteRenderer>().color = _actualColorBottom;
            _SupportPart.GetComponent<SpriteRenderer>().color = _actualColorSupport;

            CkeckColors(); //>Momentary, when there r real panels I will do it
        }

        _balloonAnimator.SetTrigger("NotEditingTrigger");

        _buttonFunctions.CloseMenu();
    }
    // Update is called once per frame

    Dictionary<int, Color32> customChange = new Dictionary<int, Color32>
    {
        { 1, new Color32(230, 199, 255, 255) }, // #E6C7FF
        { 2, new Color32(255, 217, 199, 255) }, // #FFD9C7
        { 3, new Color32(199, 255, 228, 255) }, // #C7FFE4
        { 4, new Color32(199, 213, 255, 255) }, // #C7D5FF
        { 5, new Color32( 99,  89, 124, 255) }, // #63597C
    };
    private Color32 ChangeLeftColorPanel(GameObject panel)
    {
        SpriteRenderer renderer = panel.GetComponent<SpriteRenderer>();
        if (renderer == null) return renderer.color;

        int currentIndex = -1;
        foreach (var combo in customChange)
        {
            if (renderer.color.Equals(combo.Value))
            {
                currentIndex = combo.Key;
                break;
            }
        }

        if (currentIndex == -1) //In case it doesnt find the color that the balloon have
        {
            //renderer.color = customChange[1];
            return customChange[1];
        }

        int nextIndex = currentIndex - 1;
        if (nextIndex < 1)
            nextIndex = customChange.Count;

        //renderer.color = customChange[nextIndex]; 
        return customChange[nextIndex];
    }
    private Color32 ChangeRightColorPanel(GameObject panel)
    {
        SpriteRenderer renderer = panel.GetComponent<SpriteRenderer>();
        if (renderer == null) return renderer.color;

        int currentIndex = -1;
        foreach (var combo in customChange)
        {
            if (renderer.color.Equals(combo.Value))
            {
                currentIndex = combo.Key;
                break;
            }
        }

        if (currentIndex == -1) //In case it doesnt find the color that the balloon have
        {
            renderer.color = customChange[1];
            return customChange[customChange.Count];
        }

        int nextIndex = currentIndex + 1;
        if (nextIndex > customChange.Count)
            nextIndex = 1;

        return customChange[nextIndex];

    }
    public void ChangeColor(string part, bool isLeft)
    {
        _CheckButtonCustom.SetActive(true);

        switch (part)
        {
            case "Top":

                _TopPart.GetComponent<SpriteRenderer>().color = isLeft ? ChangeLeftColorPanel (_TopPart) : ChangeRightColorPanel(_TopPart);

                if (_actualColorTop == _TopPart.GetComponent<SpriteRenderer>().color)
                    ActivateSelectedSquare(_TopCustom);
                else
                    DesactivateSelectedSquare(_TopCustom);

                break;

            case "Middle":

                _MiddlePart.GetComponent<SpriteRenderer>().color = isLeft ? ChangeLeftColorPanel(_MiddlePart) : ChangeRightColorPanel(_MiddlePart);

                if (_actualColorMiddle == _MiddlePart.GetComponent<SpriteRenderer>().color)
                    ActivateSelectedSquare(_MiddleCustom);
                else
                    DesactivateSelectedSquare(_MiddleCustom);

                break;

            case "Bottom":

                _BottomPart.GetComponent<SpriteRenderer>().color = isLeft ? ChangeLeftColorPanel(_BottomPart) : ChangeRightColorPanel(_BottomPart);

                if (_actualColorBottom == _BottomPart.GetComponent<SpriteRenderer>().color)
                    ActivateSelectedSquare(_BottomCustom);
                else
                    DesactivateSelectedSquare(_BottomCustom);

                break;

            case "Support":

                _SupportPart.GetComponent<SpriteRenderer>().color = isLeft ? ChangeLeftColorPanel(_SupportPart) : ChangeRightColorPanel(_SupportPart);

                if (_actualColorSupport == _SupportPart.GetComponent<SpriteRenderer>().color)
                    ActivateSelectedSquare(_SupportCustom);
                else
                    DesactivateSelectedSquare(_SupportCustom);

                break;

            default:
                break;

        }

        NothingToCheck();
    }
    private void DesactivateSelectedSquare(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
    private void ActivateSelectedSquare(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(true);
        }
    }
    private void NothingToCheck()
    {
        if(_actualColorTop == _TopPart.GetComponent<SpriteRenderer>().color && _actualColorMiddle == _MiddlePart.GetComponent<SpriteRenderer>().color
            && _actualColorBottom ==  _BottomPart.GetComponent<SpriteRenderer>().color && _actualColorSupport == _SupportPart.GetComponent<SpriteRenderer>().color)
            _CheckButtonCustom.SetActive(false);
    }
    public void CkeckColors()
    {
        _actualColorTop     =  _TopPart.GetComponent<SpriteRenderer>().color;
        ActivateSelectedSquare(_TopCustom);

        _actualColorMiddle  = _MiddlePart.GetComponent<SpriteRenderer>().color;
        ActivateSelectedSquare(_MiddleCustom);

        _actualColorBottom  = _BottomPart.GetComponent<SpriteRenderer>().color;
        ActivateSelectedSquare(_BottomCustom);

        _actualColorSupport = _SupportPart.GetComponent<SpriteRenderer>().color;
        ActivateSelectedSquare(_SupportCustom);

        _CheckButtonCustom.SetActive(false);
    }
}
