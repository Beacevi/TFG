using UnityEngine;

public class CustomMenuSprites : MonoBehaviour
{
    [Header("Animators")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Animator _balloonAnimator;

    [Header("Custom Parts")]
    [SerializeField] private GameObject _TopPart;
    [SerializeField] private GameObject _MiddlePart;
    [SerializeField] private GameObject _BottomPart;
    [SerializeField] private GameObject _SupportPart;

    [Header("Custom UI")]
    [SerializeField] private GameObject _TopCustom;
    [SerializeField] private GameObject _MiddleCustom;
    [SerializeField] private GameObject _BottomCustom;
    [SerializeField] private GameObject _SupportCustom;
    [SerializeField] private GameObject _CheckButtonCustom;

    [Header("Sprites")]
    public Sprite[] topSprites;
    public Sprite[] middleSprites;
    public Sprite[] bottomSprites;
    public Sprite[] supportSprites;

    private int currentTopIndex = 0;
    private int currentMiddleIndex = 0;
    private int currentBottomIndex = 0;
    private int currentSupportIndex = 0;

    // Guardar el estado "original" para saber si hay cambios
    private int _actualTopIndex;
    private int _actualMiddleIndex;
    private int _actualBottomIndex;
    private int _actualSupportIndex;

    void Start()
    {
        _CheckButtonCustom.SetActive(false);

        // Inicializa índices según el sprite actual
        currentTopIndex = System.Array.IndexOf(topSprites, _TopPart.GetComponent<SpriteRenderer>().sprite);
        currentMiddleIndex = System.Array.IndexOf(middleSprites, _MiddlePart.GetComponent<SpriteRenderer>().sprite);
        currentBottomIndex = System.Array.IndexOf(bottomSprites, _BottomPart.GetComponent<SpriteRenderer>().sprite);
        currentSupportIndex = System.Array.IndexOf(supportSprites, _SupportPart.GetComponent<SpriteRenderer>().sprite);

        if (currentTopIndex < 0) currentTopIndex = 0;
        if (currentMiddleIndex < 0) currentMiddleIndex = 0;
        if (currentBottomIndex < 0) currentBottomIndex = 0;
        if (currentSupportIndex < 0) currentSupportIndex = 0;

        // Guardar el estado original
        _actualTopIndex = currentTopIndex;
        _actualMiddleIndex = currentMiddleIndex;
        _actualBottomIndex = currentBottomIndex;
        _actualSupportIndex = currentSupportIndex;
    }

    public void ChangePart(string part, bool isLeft)
    {
        switch (part)
        {
            case "Top":
                currentTopIndex = GetNextIndex(currentTopIndex, topSprites.Length, isLeft);
                _TopPart.GetComponent<SpriteRenderer>().sprite = topSprites[currentTopIndex];
                UpdateSelectedSquare(_TopCustom, currentTopIndex);
                break;

            case "Middle":
                currentMiddleIndex = GetNextIndex(currentMiddleIndex, middleSprites.Length, isLeft);
                _MiddlePart.GetComponent<SpriteRenderer>().sprite = middleSprites[currentMiddleIndex];
                UpdateSelectedSquare(_MiddleCustom, currentMiddleIndex);
                break;

            case "Bottom":
                currentBottomIndex = GetNextIndex(currentBottomIndex, bottomSprites.Length, isLeft);
                _BottomPart.GetComponent<SpriteRenderer>().sprite = bottomSprites[currentBottomIndex];
                UpdateSelectedSquare(_BottomCustom, currentBottomIndex);
                break;

            case "Support":
                currentSupportIndex = GetNextIndex(currentSupportIndex, supportSprites.Length, isLeft);
                _SupportPart.GetComponent<SpriteRenderer>().sprite = supportSprites[currentSupportIndex];
                UpdateSelectedSquare(_SupportCustom, currentSupportIndex);
                break;
        }

        // Activar o desactivar botón de guardar cambios
        UpdateCheckButton();
    }

    private int GetNextIndex(int current, int length, bool isLeft)
    {
        if (isLeft)
        {
            current--;
            if (current < 0) current = length - 1;
        }
        else
        {
            current++;
            if (current >= length) current = 0;
        }
        return current;
    }

    private void UpdateSelectedSquare(GameObject parent, int index)
    {
        int i = 0;
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(i == index);
            i++;
        }
    }

    private void UpdateCheckButton()
    {
        if (currentTopIndex != _actualTopIndex ||
            currentMiddleIndex != _actualMiddleIndex ||
            currentBottomIndex != _actualBottomIndex ||
            currentSupportIndex != _actualSupportIndex)
        {
            _CheckButtonCustom.SetActive(true);
        }
        else
        {
            _CheckButtonCustom.SetActive(false);
        }
    }

    // Llamar este método cuando se “guarden” los cambios para actualizar el estado original
    public void CheckSprites()
    {
        _actualTopIndex = currentTopIndex;
        _actualMiddleIndex = currentMiddleIndex;
        _actualBottomIndex = currentBottomIndex;
        _actualSupportIndex = currentSupportIndex;

        _CheckButtonCustom.SetActive(false);
    }
}
