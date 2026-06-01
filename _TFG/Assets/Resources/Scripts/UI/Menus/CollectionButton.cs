/**
 * @file CollectionButton.cs
 * @brief Gestiona el botón de acceso a la colección de aves u objetos.
 * @author Hortensia Studio - Beatriz Ceballos Vidal
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Gestiona el botón de acceso a la colección de aves u objetos.
/// </summary>
public class CollectionButton : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo Animator utilizado para almacenar o configurar animator.
    /// </summary>
    [Header("Animators")]
    [SerializeField] private Animator _animator;

    /// <summary>
    /// Botón de interfaz asociado a button functions.
    /// </summary>
    [Header("Scripts")]
    private ButtonFunctions  _buttonFunctions;

    /// <summary>
    /// Panel de interfaz asociado a collection panel.
    /// </summary>
    [Header("Panels")]
    [SerializeField] private GameObject _CollectionPanel;
    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar all collection.
    /// </summary>
    [SerializeField] private GameObject _AllCollection;
    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar info collection.
    /// </summary>
    [SerializeField] private GameObject _InfoCollection;
    /// <summary>
    /// Botón de interfaz asociado a button close collection.
    /// </summary>
    [SerializeField] private GameObject _ButtonCloseCollection; //>Boton que cierra el panel de collection

    /// <summary>
    /// Referencia de interfaz utilizada para mostrar o actualizar title text.
    /// </summary>
    [Header("InfoCollectionsObjects")]
    [SerializeField] private TMP_Text _TitleText;
    /// <summary>
    /// Referencia de interfaz utilizada para mostrar o actualizar subtitle text.
    /// </summary>
    [SerializeField] private TMP_Text _SubtitleText;
    /// <summary>
    /// Referencia de interfaz utilizada para mostrar o actualizar info text.
    /// </summary>
    [SerializeField] private TMP_Text _InfoText;
    /// <summary>
    /// Campo de tipo Image utilizado para almacenar o configurar image collection.
    /// </summary>
    [SerializeField] private Image _ImageCollection;

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    private void Start()
    {
        _buttonFunctions = GetComponent<ButtonFunctions>();

        if(_CollectionPanel != null && _InfoCollection != null)
        {
         _CollectionPanel.SetActive(false);
         _InfoCollection.SetActive(false);
        }
    }

    /// <summary>
    /// Abre collection menu dentro del flujo de interfaz.
    /// </summary>
    /// <param name="button">Parámetro button empleado por el método.</param>
    public void OpenCollectionMenu(Button button)
    {
        _CollectionPanel.SetActive(true);

        _animator.SetTrigger("OpenTrigger");
        StartCoroutine(_buttonFunctions.InteractibleButton(button, _animator));

        StickerManager.Instance.CargarStickers();

        _buttonFunctions.OpenMenu();
    }

    /// <summary>
    /// Cierra collection menu dentro del flujo de interfaz.
    /// </summary>
    /// <param name="button">Parámetro button empleado por el método.</param>
    public void CloseCollectionMenu(Button button)
    {
        _animator.SetTrigger("CloseTrigger");
        StartCoroutine(_buttonFunctions.CloseInteractibleButton(button, _animator, _CollectionPanel));

        Close();
    }
    /// <summary>
    /// Cierra elemento dentro del flujo de interfaz.
    /// </summary>
    private void Close()
    {

        _buttonFunctions.CloseMenu();
    }

    /// <summary>
    /// Abre info dentro del flujo de interfaz.
    /// </summary>
    public void OpenInfo() ///Con un sistema de tags se cambia el texto de la carta
    {
        _AllCollection.SetActive(false);
        _ButtonCloseCollection.SetActive(false);

        _InfoCollection.SetActive(true);
    }
    /// <summary>
    /// Cierra info dentro del flujo de interfaz.
    /// </summary>
    public void CloseInfo()
    {
        _AllCollection.SetActive(true);
        _ButtonCloseCollection.SetActive(true);

        _InfoCollection.SetActive(false);
    }
    /// <summary>
    /// Actualiza text based on tag para reflejar el estado actual del sistema.
    /// </summary>
    /// <param name="buttonTag">Parámetro button tag empleado por el método.</param>
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
