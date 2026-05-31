/**
 * @file CustomMenuSprites.cs
 * @brief Almacena referencias visuales utilizadas por el menú de personalización.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;

/// <summary>
/// Almacena referencias visuales utilizadas por el menú de personalización.
/// </summary>
public class CustomMenuSprites : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo Animator utilizado para almacenar o configurar animator.
    /// </summary>
    [Header("Animators")]
    [SerializeField] private Animator _animator;
    /// <summary>
    /// Campo de tipo Animator utilizado para almacenar o configurar balloon animator.
    /// </summary>
    [SerializeField] private Animator _balloonAnimator;

    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar top part.
    /// </summary>
    [Header("Custom")]
    [SerializeField] private GameObject _TopPart;
    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar middle part.
    /// </summary>
    [SerializeField] private GameObject _MiddlePart;
    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar bottom part.
    /// </summary>
    [SerializeField] private GameObject _BottomPart;
    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar support part.
    /// </summary>
    [SerializeField] private GameObject _SupportPart;

    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar top custom.
    /// </summary>
    [SerializeField] private GameObject _TopCustom;
    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar middle custom.
    /// </summary>
    [SerializeField] private GameObject _MiddleCustom;
    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar bottom custom.
    /// </summary>
    [SerializeField] private GameObject _BottomCustom;
    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar support custom.
    /// </summary>
    [SerializeField] private GameObject _SupportCustom;

    /// <summary>
    /// Botón de interfaz asociado a check button custom.
    /// </summary>
    [SerializeField] private GameObject _CheckButtonCustom;
    /// <summary>
    /// Panel de interfaz asociado a custom panel.
    /// </summary>
    [SerializeField] private GameObject _CustomPanel;

    /// <summary>
    /// Colección de top sprites utilizada por este componente.
    /// </summary>
    [Header("Sprites")]
    public Sprite[] topSprites;
    /// <summary>
    /// Colección de middle sprites utilizada por este componente.
    /// </summary>
    public Sprite[] middleSprites;
    /// <summary>
    /// Colección de bottom sprites utilizada por este componente.
    /// </summary>
    public Sprite[] bottomSprites;
    /// <summary>
    /// Colección de support sprites utilizada por este componente.
    /// </summary>
    public Sprite[] supportSprites;

    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar current top index.
    /// </summary>
    private int currentTopIndex;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar current middle index.
    /// </summary>
    private int currentMiddleIndex;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar current bottom index.
    /// </summary>
    private int currentBottomIndex;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar current support index.
    /// </summary>
    private int currentSupportIndex;

    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar actual top index.
    /// </summary>
    private int _actualTopIndex;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar actual middle index.
    /// </summary>
    private int _actualMiddleIndex;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar actual bottom index.
    /// </summary>
    private int _actualBottomIndex;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar actual support index.
    /// </summary>
    private int _actualSupportIndex;

    /// <summary>
    /// Indica si check está activo o habilitado.
    /// </summary>
    private bool check = false;

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
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

    /// <summary>
    /// Obtiene initial index a partir del estado actual del sistema.
    /// </summary>
    /// <param name="part">Parámetro part empleado por el método.</param>
    /// <param name="array">Parámetro array empleado por el método.</param>
    /// <returns>Valor numérico calculado o consultado por el método.</returns>
    int GetInitialIndex(GameObject part, Sprite[] array)
    {
        int index = System.Array.IndexOf(array, part.GetComponent<SpriteRenderer>().sprite);
        return index < 0 ? 0 : index;
    }

    // ========= MENU =========

    /// <summary>
    /// Abre custom menu dentro del flujo de interfaz.
    /// </summary>
    public void OpenCustomMenu()
    {
        _CustomPanel.SetActive(true);
        _balloonAnimator.SetTrigger("EditingTrigger");
        _animator.SetTrigger("OpenTrigger");
    }

    /// <summary>
    /// Cierra custom menu dentro del flujo de interfaz.
    /// </summary>
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

    /// <summary>
    /// Ejecuta la lógica asociada a restore original sprites dentro de CustomMenuSprites.
    /// </summary>
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

    /// <summary>
    /// Cambia change part según la interacción o parámetro recibido.
    /// </summary>
    /// <param name="part">Parámetro part empleado por el método.</param>
    /// <param name="isLeft">Parámetro is left empleado por el método.</param>
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

    /// <summary>
    /// Ejecuta la lógica asociada a next index dentro de CustomMenuSprites.
    /// </summary>
    /// <param name="current">Parámetro current empleado por el método.</param>
    /// <param name="length">Parámetro length empleado por el método.</param>
    /// <param name="left">Parámetro left empleado por el método.</param>
    /// <returns>Valor numérico calculado o consultado por el método.</returns>
    int NextIndex(int current, int length, bool left)
    {
        current += left ? -1 : 1;

        if (current < 0) current = length - 1;
        if (current >= length) current = 0;

        return current;
    }

    // ========= CHECK SYSTEM =========

    /// <summary>
    /// Ejecuta la lógica asociada a nothing to check dentro de CustomMenuSprites.
    /// </summary>
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

    /// <summary>
    /// Ejecuta la lógica asociada a check sprites dentro de CustomMenuSprites.
    /// </summary>
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

    /// <summary>
    /// Ejecuta la lógica asociada a desactivate selected square dentro de CustomMenuSprites.
    /// </summary>
    /// <param name="parent">Parámetro parent empleado por el método.</param>
    void DesactivateSelectedSquare(GameObject parent)
    {
        foreach (Transform child in parent.transform)
            child.gameObject.SetActive(false);
    }

    /// <summary>
    /// Ejecuta la lógica asociada a activate selected square dentro de CustomMenuSprites.
    /// </summary>
    /// <param name="parent">Parámetro parent empleado por el método.</param>
    void ActivateSelectedSquare(GameObject parent)
    {
        foreach (Transform child in parent.transform)
            child.gameObject.SetActive(true);
    }
}
