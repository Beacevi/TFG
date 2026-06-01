/**
 * @file ShopMenu.cs
 * @brief Administra la tienda del juego, incluyendo compra, desbloqueo y visualización de elementos.
 * @author Hortensia Studio - Beatriz Ceballos Vidal
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Administra la tienda del juego, incluyendo compra, desbloqueo y visualización de elementos.
/// </summary>
public class ShopMenu : MonoBehaviour
{

    /// <summary>
    /// Campo de tipo Animator utilizado para almacenar o configurar animator.
    /// </summary>
    [Header("Animators")]
    [SerializeField] private Animator _animator;

    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar store menu.
    /// </summary>
    [Header("Panels")]
    [SerializeField] private GameObject _storeMenu;
    /// <summary>
    /// Panel de interfaz asociado a reset store panel.
    /// </summary>
    [SerializeField] private GameObject _resetStorePanel;

    /// <summary>
    /// Tiempo o duración asociado a next reset time.
    /// </summary>
    [Header("Timer")]
    private DateTime _nextResetTime;
    /// <summary>
    /// Color utilizado para representar normal color.
    /// </summary>
    private Color _normalColor;
    /// <summary>
    /// Color utilizado para representar last hour color.
    /// </summary>
    [SerializeField] private Color _lastHourColor = Color.red;

    //[Header("Articles In Store")]
    //[SerializeField] private GameObject _Article1;
    //[SerializeField] private GameObject _Article2;
    //[SerializeField] private GameObject _Article3;

    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar balloon.
    /// </summary>
    [Header("Plus")]
    [SerializeField] private GameObject _balloon;
    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar birds.
    /// </summary>
    [SerializeField] private GameObject _birds;

    /// <summary>
    /// Botón de interfaz asociado a button functions.
    /// </summary>
    [Header("Scripts")]
    [SerializeField] private ButtonFunctions _buttonFunctions;

    /// <summary>
    /// Indica si is reset message open está activo o habilitado.
    /// </summary>
    private bool _isResetMessageOpen = false;
    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    void Start()
    {
        if(_buttonFunctions != null && _normalColor != null && _nextResetTime != null)
        {
           _buttonFunctions = GetComponent<ButtonFunctions>();

            _nextResetTime = DateTime.Today.AddDays(1); 
        }


        if(_storeMenu != null)
        {
            _storeMenu.SetActive(false);
        }

    }

    /// <summary>
    /// Abre store menu dentro del flujo de interfaz.
    /// </summary>
    /// <param name="button">Parámetro button empleado por el método.</param>
    public void OpenStoreMenu(Button button)
    {
        _storeMenu.SetActive(true);
        
        _animator.SetTrigger("OpenTrigger");
        StartCoroutine(_buttonFunctions.InteractibleButton(button, _animator));

        _birds.SetActive(false);
        _balloon.SetActive(false);

        _buttonFunctions.OpenMenu();
    }
    /// <summary>
    /// Restablece los valores del sistema a su configuración inicial o por defecto.
    /// </summary>
    public void ResetStorePanel()
    {
        if (!_isResetMessageOpen)
        {
            _resetStorePanel.SetActive(true);
            _isResetMessageOpen = true;
        }
        else
        {
            _resetStorePanel.SetActive(false);
            _isResetMessageOpen = false;
        }   
    }
    /// <summary>
    /// Cierra animation dentro del flujo de interfaz.
    /// </summary>
    /// <param name="button">Parámetro button empleado por el método.</param>
    public void CloseAnimation(Button button)
    {
        
        _animator.SetTrigger("CloseTrigger");
        StartCoroutine(_buttonFunctions.CloseInteractibleButton(button, _animator, _storeMenu));

        Close();

    }
    /// <summary>
    /// Cierra elemento dentro del flujo de interfaz.
    /// </summary>
    private void Close()
    {
        //_resetStorePanel.SetActive(false);

        _birds.SetActive(true);
        _balloon.SetActive(true);
    }

    /// <summary>
    /// Restablece los valores del sistema a su configuración inicial o por defecto.
    /// </summary>
    public void ResetStore()
    {
        //Change articles here
    }

    //public void BuyArticle(int article)
    //{
    //    //Buy article logic here

    //    if (article == 1)
    //    {
    //        Buy(_Article1);
    //    }
    //    else if (article == 2)
    //    {
    //        Buy(_Article2);
    //    }
    //    else if (article == 3)
    //    {
    //        Buy(_Article3);
    //    }
    //}

    /// <summary>
    /// Ejecuta la lógica asociada a buy dentro de ShopMenu.
    /// </summary>
    /// <param name="_article">Parámetro article empleado por el método.</param>
    private void Buy(GameObject _article)
    {
        // Implement buy logic here
    }
}

