/**
 * @file CustomMenu.cs
 * @brief Controla el menú de personalización del globo y los elementos desbloqueables asociados.
 * @author Hortensia Studio - Beatriz Ceballos Vidal
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// Controla el menú de personalización del globo y los elementos desbloqueables asociados.
/// </summary>
public class CustomMenu : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo Animator utilizado para almacenar o configurar animator.
    /// </summary>
    [Header("Animators")]
    [SerializeField] private Animator _animator; 
    /// <summary>
    /// Campo de tipo Animator utilizado para almacenar o configurar balloon animator.
    /// </summary>
    [SerializeField] private Animator _balloonAnimator; //>Animator del glovo

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
    /// Botón de interfaz asociado a button functions.
    /// </summary>
    [Header("Scripts")]
    private ButtonFunctions _buttonFunctions;

    /// <summary>
    /// Panel de interfaz asociado a custom panel.
    /// </summary>
    [Header("Panels")]
    [SerializeField] private GameObject _CustomPanel;
    /// <summary>
    /// Panel de interfaz asociado a play panel.
    /// </summary>
    [SerializeField] private GameObject _PlayPanel;

    /// <summary>
    /// Indica si check está activo o habilitado.
    /// </summary>
    private bool check = false;
    /// <summary>
    /// Color utilizado para representar actual color top.
    /// </summary>
    private Color _actualColorTop;
    /// <summary>
    /// Color utilizado para representar actual color middle.
    /// </summary>
    private Color _actualColorMiddle;
    /// <summary>
    /// Color utilizado para representar actual color bottom.
    /// </summary>
    private Color _actualColorBottom;
    /// <summary>
    /// Color utilizado para representar actual color support.
    /// </summary>
    private Color _actualColorSupport;
    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
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
    /// <summary>
    /// Abre custom menu dentro del flujo de interfaz.
    /// </summary>
    /// <param name="button">Parámetro button empleado por el método.</param>
    public void OpenCustomMenu(UnityEngine.UI.Button button)
    {
        _PlayPanel.SetActive(false);
        _CustomPanel.SetActive(true);
        _balloonAnimator.SetTrigger("EditingTrigger");

        _animator.SetTrigger("OpenTrigger");
        StartCoroutine(_buttonFunctions.InteractibleButton(button, _animator));

        _buttonFunctions.OpenMenu();
    }

    /// <summary>
    /// Cierra custom menu dentro del flujo de interfaz.
    /// </summary>
    /// <param name="button">Parámetro button empleado por el método.</param>
    public void CloseCustomMenu(UnityEngine.UI.Button button)
    {
        _PlayPanel.SetActive(true);
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
    /// <summary>
    /// Colección de base colors utilizada por este componente.
    /// </summary>
    private List<Color32> baseColors = new List<Color32>()
    {
        new Color32(230, 199, 255, 255),
        new Color32(255, 217, 199, 255),
        new Color32(199, 255, 228, 255),
        new Color32(199, 213, 255, 255),
        new Color32(99, 89, 124, 255)
    };

    /// <summary>
    /// Establece o actualiza unlocked colors dentro del sistema.
    /// </summary>
    /// <param name="colors">Parámetro colors empleado por el método.</param>
    public void SetUnlockedColors(List<Color32> colors)
    {
        customChange = new List<Color32>(baseColors);

        foreach (var c in colors)
        {
            if (!customChange.Contains(c))
                customChange.Add(c);
        }
    }
    /// <summary>
    /// Obtiene unlocked colors a partir del estado actual del sistema.
    /// </summary>
    /// <returns>Instancia o valor de tipo List<Color32> resultante de la operación.</returns>
    public List<Color32> GetUnlockedColors()
    {
        List<Color32> result = new List<Color32>();

        for (int i = baseColors.Count; i < customChange.Count; i++)
        {
            result.Add(customChange[i]);
        }

        return result;
    }
    /// <summary>
    /// Propiedad que expone o modifica custom change.
    /// </summary>
    public List<Color32> customChange { get; private set; }
    /// <summary>
    /// Obtiene colors a partir del estado actual del sistema.
    /// </summary>
    /// <returns>Instancia o valor de tipo List<string> resultante de la operación.</returns>
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
    /// <summary>
    /// Establece o actualiza colors dentro del sistema.
    /// </summary>
    /// <param name="colors">Parámetro colors empleado por el método.</param>
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
    /// <summary>
    /// Añade color al estado gestionado por el componente.
    /// </summary>
    /// <param name="color">Parámetro color empleado por el método.</param>
    public void AddColor(Color32 color)
    {
        if (!customChange.Contains(color))
            customChange.Add(color);
    }
    /// <summary>
    /// Obtiene color index a partir del estado actual del sistema.
    /// </summary>
    /// <param name="color">Parámetro color empleado por el método.</param>
    /// <returns>Valor numérico calculado o consultado por el método.</returns>
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
    /// <summary>
    /// Cambia change left color panel según la interacción o parámetro recibido.
    /// </summary>
    /// <param name="currentColor">Parámetro current color empleado por el método.</param>
    /// <returns>Instancia o valor de tipo Color32 resultante de la operación.</returns>
    private Color32 ChangeLeftColorPanel(Color32 currentColor)
    {
        int currentIndex = GetColorIndex(currentColor);

        if (currentIndex == -1)
            return customChange[0];

        int nextIndex = currentIndex - 1;

        if (nextIndex < 0)
            nextIndex = customChange.Count - 1;

        return customChange[nextIndex];
    }

    /// <summary>
    /// Cambia change right color panel según la interacción o parámetro recibido.
    /// </summary>
    /// <param name="currentColor">Parámetro current color empleado por el método.</param>
    /// <returns>Instancia o valor de tipo Color32 resultante de la operación.</returns>
    private Color32 ChangeRightColorPanel(Color32 currentColor)
    {
        int currentIndex = GetColorIndex(currentColor);

        if (currentIndex == -1)
            return customChange[0];

        int nextIndex = currentIndex + 1;

        if (nextIndex >= customChange.Count)
            nextIndex = 0;

        return customChange[nextIndex];
    }
    /// <summary>
    /// Cambia change color según la interacción o parámetro recibido.
    /// </summary>
    /// <param name="part">Parámetro part empleado por el método.</param>
    /// <param name="isLeft">Parámetro is left empleado por el método.</param>
    /// <param name="targetImage">Parámetro target image empleado por el método.</param>
    public void ChangeColor(string part, bool isLeft, Image targetImage)
    {
        _CheckButtonCustom.SetActive(true);

        switch (part)
        {
            case "Top":
                ApplyColorChange(targetImage, _TopPart, isLeft, _actualColorTop, _TopCustom);
                break;

            case "Middle":
                ApplyColorChange(targetImage, _MiddlePart, isLeft, _actualColorMiddle, _MiddleCustom);
                break;

            case "Bottom":
                ApplyColorChange(targetImage, _BottomPart, isLeft, _actualColorBottom, _BottomCustom);
                break;

            case "Support":
                ApplyColorChange(targetImage, _SupportPart, isLeft, _actualColorSupport, _SupportCustom);
                break;
        }

        NothingToCheck();
    }

    /// <summary>
    /// Ejecuta la lógica asociada a apply color change dentro de CustomMenu.
    /// </summary>
    /// <param name="image">Parámetro image empleado por el método.</param>
    /// <param name="spritePart">Parámetro sprite part empleado por el método.</param>
    /// <param name="isLeft">Parámetro is left empleado por el método.</param>
    /// <param name="actualColor">Parámetro actual color empleado por el método.</param>
    /// <param name="customIndicator">Parámetro custom indicator empleado por el método.</param>
    private void ApplyColorChange(Image image, GameObject spritePart, bool isLeft, Color actualColor, GameObject customIndicator)
    {
        Color32 newColor = isLeft ? ChangeLeftColorPanel((Color32)image.color) : ChangeRightColorPanel((Color32)image.color);

        image.color = newColor;
        spritePart.GetComponent<SpriteRenderer>().color = newColor;

        if (actualColor == image.color)
            ActivateSelectedSquare(customIndicator);
        else
            DesactivateSelectedSquare(customIndicator);
    }
    /// <summary>
    /// Ejecuta la lógica asociada a desactivate selected square dentro de CustomMenu.
    /// </summary>
    /// <param name="parent">Parámetro parent empleado por el método.</param>
    private void DesactivateSelectedSquare(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Ejecuta la lógica asociada a activate selected square dentro de CustomMenu.
    /// </summary>
    /// <param name="parent">Parámetro parent empleado por el método.</param>
    private void ActivateSelectedSquare(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// Ejecuta la lógica asociada a nothing to check dentro de CustomMenu.
    /// </summary>
    private void NothingToCheck()
    {
        if(_actualColorTop == _TopPart.GetComponent<SpriteRenderer>().color && _actualColorMiddle == _MiddlePart.GetComponent<SpriteRenderer>().color
            && _actualColorBottom ==  _BottomPart.GetComponent<SpriteRenderer>().color && _actualColorSupport == _SupportPart.GetComponent<SpriteRenderer>().color)
            _CheckButtonCustom.SetActive(false);
    }
    /// <summary>
    /// Ejecuta la lógica asociada a ckeck colors dentro de CustomMenu.
    /// </summary>
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
