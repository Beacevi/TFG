using UnityEngine;

public class CustomMenuSprites : MonoBehaviour
{
    [Header("Animators")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Animator _balloonAnimator;

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
    [SerializeField] private GameObject _CustomPanel;

    [Header("Sprites")]
    public Sprite[] topSprites;
    public Sprite[] middleSprites;
    public Sprite[] bottomSprites;
    public Sprite[] supportSprites;

    private int currentTopIndex;
    private int currentMiddleIndex;
    private int currentBottomIndex;
    private int currentSupportIndex;

    private int _actualTopIndex;
    private int _actualMiddleIndex;
    private int _actualBottomIndex;
    private int _actualSupportIndex;

    private bool check = false;

    void Start()
    {
        _CustomPanel.SetActive(false);
        _CheckButtonCustom.SetActive(false);

        currentTopIndex   = GetInitialIndex(_TopPart, topSprites);
        currentMiddleIndex = GetInitialIndex(_MiddlePart, middleSprites);
        currentBottomIndex = GetInitialIndex(_BottomPart, bottomSprites);
        currentSupportIndex = GetInitialIndex(_SupportPart, supportSprites);

        _actualTopIndex = currentTopIndex;
        _actualMiddleIndex = currentMiddleIndex;
        _actualBottomIndex = currentBottomIndex;
        _actualSupportIndex = currentSupportIndex;
    }

    int GetInitialIndex(GameObject part, Sprite[] array)
    {
        int index = System.Array.IndexOf(array, part.GetComponent<SpriteRenderer>().sprite);
        return index < 0 ? 0 : index;
    }

    // ========= MENU =========

    public void OpenCustomMenu()
    {
        _CustomPanel.SetActive(true);
        _balloonAnimator.SetTrigger("EditingTrigger");
        _animator.SetTrigger("OpenTrigger");
    }

    public void CloseCustomMenu()
    {
        if (!check)
        {
            RestoreOriginalSprites();
            CheckSprites(); // igual que primer script
        }

        _balloonAnimator.SetTrigger("NotEditingTrigger");
        _animator.SetTrigger("CloseTrigger");
    }

    void RestoreOriginalSprites()
    {
        currentTopIndex = _actualTopIndex;
        currentMiddleIndex = _actualMiddleIndex;
        currentBottomIndex = _actualBottomIndex;
        currentSupportIndex = _actualSupportIndex;

        _TopPart.GetComponent<SpriteRenderer>().sprite = topSprites[currentTopIndex];
        _MiddlePart.GetComponent<SpriteRenderer>().sprite = middleSprites[currentMiddleIndex];
        _BottomPart.GetComponent<SpriteRenderer>().sprite = bottomSprites[currentBottomIndex];
        _SupportPart.GetComponent<SpriteRenderer>().sprite = supportSprites[currentSupportIndex];
    }

    // ========= CHANGE =========

    public void ChangePart(string part, bool isLeft)
    {
        _CheckButtonCustom.SetActive(true);

        switch (part)
        {
            case "Top":
                currentTopIndex = NextIndex(currentTopIndex, topSprites.Length, isLeft);
                _TopPart.GetComponent<SpriteRenderer>().sprite = topSprites[currentTopIndex];

                if (currentTopIndex == _actualTopIndex)
                    ActivateSelectedSquare(_TopCustom);
                else
                    DesactivateSelectedSquare(_TopCustom);
                break;

            case "Middle":
                currentMiddleIndex = NextIndex(currentMiddleIndex, middleSprites.Length, isLeft);
                _MiddlePart.GetComponent<SpriteRenderer>().sprite = middleSprites[currentMiddleIndex];

                if (currentMiddleIndex == _actualMiddleIndex)
                    ActivateSelectedSquare(_MiddleCustom);
                else
                    DesactivateSelectedSquare(_MiddleCustom);
                break;

            case "Bottom":
                currentBottomIndex = NextIndex(currentBottomIndex, bottomSprites.Length, isLeft);
                _BottomPart.GetComponent<SpriteRenderer>().sprite = bottomSprites[currentBottomIndex];

                if (currentBottomIndex == _actualBottomIndex)
                    ActivateSelectedSquare(_BottomCustom);
                else
                    DesactivateSelectedSquare(_BottomCustom);
                break;

            case "Support":
                currentSupportIndex = NextIndex(currentSupportIndex, supportSprites.Length, isLeft);
                _SupportPart.GetComponent<SpriteRenderer>().sprite = supportSprites[currentSupportIndex];

                if (currentSupportIndex == _actualSupportIndex)
                    ActivateSelectedSquare(_SupportCustom);
                else
                    DesactivateSelectedSquare(_SupportCustom);
                break;
        }

        NothingToCheck();
    }

    int NextIndex(int current, int length, bool left)
    {
        current += left ? -1 : 1;

        if (current < 0) current = length - 1;
        if (current >= length) current = 0;

        return current;
    }

    // ========= CHECK SYSTEM =========

    void NothingToCheck()
    {
        if (currentTopIndex == _actualTopIndex &&
            currentMiddleIndex == _actualMiddleIndex &&
            currentBottomIndex == _actualBottomIndex &&
            currentSupportIndex == _actualSupportIndex)
        {
            _CheckButtonCustom.SetActive(false);
        }
    }

    public void CheckSprites()
    {
        _actualTopIndex = currentTopIndex;
        ActivateSelectedSquare(_TopCustom);

        _actualMiddleIndex = currentMiddleIndex;
        ActivateSelectedSquare(_MiddleCustom);

        _actualBottomIndex = currentBottomIndex;
        ActivateSelectedSquare(_BottomCustom);

        _actualSupportIndex = currentSupportIndex;
        ActivateSelectedSquare(_SupportCustom);

        _CheckButtonCustom.SetActive(false);
        check = true;
    }

    // ========= UI =========

    void DesactivateSelectedSquare(GameObject parent)
    {
        foreach (Transform child in parent.transform)
            child.gameObject.SetActive(false);
    }

    void ActivateSelectedSquare(GameObject parent)
    {
        foreach (Transform child in parent.transform)
            child.gameObject.SetActive(true);
    }
}