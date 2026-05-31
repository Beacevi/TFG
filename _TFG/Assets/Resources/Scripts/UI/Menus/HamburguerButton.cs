/**
 * @file HamburguerButton.cs
 * @brief Gestiona el botón de menú hamburguesa y el despliegue de opciones de interfaz.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Gestiona el botón de menú hamburguesa y el despliegue de opciones de interfaz.
/// </summary>
public class HamburguerButton : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo Animator utilizado para almacenar o configurar animator.
    /// </summary>
    [SerializeField] private Animator _animator;

    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar opened icon.
    /// </summary>
    [Header("Icons")]
    [SerializeField] private GameObject _openedIcon;
    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar closed icon.
    /// </summary>
    [SerializeField] private GameObject _closedIcon;

    /// <summary>
    /// Botón de interfaz asociado a button hamburguer.
    /// </summary>
    [Header("Plus")]
    [SerializeField] private Button _buttonHamburguer;
    /// <summary>
    /// Botón de interfaz asociado a button functions.
    /// </summary>
    private ButtonFunctions _buttonFunctions;

    /// <summary>
    /// Indica si is open está activo o habilitado.
    /// </summary>
    private bool isOpen = false;


    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    private void Start()
    {
        _buttonFunctions = GetComponent<ButtonFunctions>();

        if(_openedIcon != null && _closedIcon != null)
        {
            _openedIcon.SetActive(true);
            _closedIcon.SetActive(false);
        }

        
    }
    /// <summary>
    /// Ejecuta la lógica asociada a hamburguer menu dentro de HamburguerButton.
    /// </summary>
    public void HamburguerMenu()
    {
        if (isOpen)
        {
            StartCoroutine(_buttonFunctions.InteractibleButton(_buttonHamburguer, _animator));
            _animator.SetTrigger("CloseTrigger");

            _openedIcon.SetActive(true);
            _closedIcon.SetActive(false);
            isOpen = false;
        }
        else
        {
            StartCoroutine(_buttonFunctions.InteractibleButton(_buttonHamburguer, _animator));
            _animator.SetTrigger("OpenTrigger");

            _closedIcon.SetActive(true);
            _openedIcon.SetActive(false);
            isOpen = true;
        }
    }
}
