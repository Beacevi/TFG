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

        if(_CustomPanel != null && _CheckButtonCustom != null)
        {
            _CustomPanel.SetActive(false);
            _CheckButtonCustom.SetActive(false);
        }

        _actualColorTop = _TopPart.GetComponent<SpriteRenderer>().color;
        _actualColorMiddle = _MiddlePart.GetComponent<SpriteRenderer>().color;
        _actualColorBottom = _BottomPart.GetComponent<SpriteRenderer>().color;
        _actualColorSupport = _SupportPart.GetComponent<SpriteRenderer>().color;

        customChange = new List<Color32>(baseColors);


    }
    public void OpenCustomMenu(UnityEngine.UI.Button button)
    {
        _CustomPanel.SetActive(true);
        _balloonAnimator.SetTrigger("EditingTrigger");

        _animator.SetTrigger("OpenTrigger");
        StartCoroutine(_buttonFunctions.InteractibleButton(button, _animator));

        _buttonFunctions.OpenMenu();
    }

    public void CloseCustomMenu(UnityEngine.UI.Button button)
    {

        if(!check)
        {
            _TopPart.GetComponent<SpriteRenderer>().color = _actualColorTop;
            _MiddlePart.GetComponent<SpriteRenderer>().color = _actualColorMiddle;
            _BottomPart.GetComponent<SpriteRenderer>().color = _actualColorBottom;
            _SupportPart.GetComponent<SpriteRenderer>().color = _actualColorSupport;

            CkeckColors(); //>Momentary, when there r real panels I will do it
        }

        _balloonAnimator.SetTrigger("NotEditingTrigger");

        _animator.SetTrigger("CloseTrigger");
        StartCoroutine(_buttonFunctions.CloseInteractibleButton(button, _animator, _CustomPanel));

        _buttonFunctions.CloseMenu();
    }


    // Update is called once per frame
    private List<Color32> baseColors = new List<Color32>()
    {
        new Color32(230, 199, 255, 255),
        new Color32(255, 217, 199, 255),
        new Color32(199, 255, 228, 255),
        new Color32(199, 213, 255, 255),
        new Color32(99, 89, 124, 255)
    };

    public void SetUnlockedColors(List<Color32> colors)
    {
        customChange = new List<Color32>(baseColors);

        foreach (var c in colors)
        {
            if (!customChange.Contains(c))
                customChange.Add(c);
        }
    }
    public List<Color32> GetUnlockedColors()
    {
        List<Color32> result = new List<Color32>();

        for (int i = baseColors.Count; i < customChange.Count; i++)
        {
            result.Add(customChange[i]);
        }

        return result;
    }
    public List<Color32> customChange { get; private set; }
    public List<string> GetColors()
    {
        List<string> result = new List<string>();

        foreach (Color32 c in customChange)
        {
            string hex = ColorUtility.ToHtmlStringRGBA(c);
            result.Add(hex);
        }

        return result;
    }
    public void SetColors(List<string> colors)
    {
        customChange.Clear();

        foreach (string hex in colors)
        {
            if (ColorUtility.TryParseHtmlString("#" + hex, out Color c))
            {
                customChange.Add(c);
            }
        }
    }
    public void AddColor(Color32 color)
    {
        if (!customChange.Contains(color))
            customChange.Add(color);
    }
    private int GetColorIndex(Color32 color)
    {
        for (int i = 0; i < customChange.Count; i++)
        {
            Color32 c = customChange[i];

            if (c.r == color.r &&
                c.g == color.g &&
                c.b == color.b &&
                c.a == color.a)
            {
                return i;
            }
        }

        return -1;
    }
    private Color32 ChangeLeftColorPanel(GameObject panel)
    {
        SpriteRenderer renderer = panel.GetComponent<SpriteRenderer>();
        if (renderer == null) return Color.white;

        int currentIndex = GetColorIndex((Color32)renderer.color);

        if (currentIndex == -1)
            return customChange[0];

        int nextIndex = currentIndex - 1;

        if (nextIndex < 0)
            nextIndex = customChange.Count - 1;

        return customChange[nextIndex];
    }
    private Color32 ChangeRightColorPanel(GameObject panel)
    {
        SpriteRenderer renderer = panel.GetComponent<SpriteRenderer>();
        if (renderer == null) return Color.white;

        int currentIndex = GetColorIndex((Color32)renderer.color);

        if (currentIndex == -1)
            return customChange[0];

        int nextIndex = currentIndex + 1;

        if (nextIndex >= customChange.Count)
            nextIndex = 0;

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
