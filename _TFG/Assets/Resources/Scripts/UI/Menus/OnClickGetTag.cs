/**
 * @file OnClickGetTag.cs
 * @brief Obtiene información del objeto pulsado para abrir o actualizar la vista de colección.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Obtiene información del objeto pulsado para abrir o actualizar la vista de colección.
/// </summary>
public class OnClickCollection : MonoBehaviour
{
    /// <summary>
    /// Referencia al gestor encargado de canvas manager collection.
    /// </summary>
    [SerializeField] private CollectionButton _canvasManagerCollection;
    /// <summary>
    /// Referencia al gestor encargado de canvas manager bird.
    /// </summary>
    [SerializeField] private BirdButton       _canvasManagerBird;
    /// <summary>
    /// Campo de tipo CustomMenu utilizado para almacenar o configurar canvas custom.
    /// </summary>
    [SerializeField] private CustomMenu       _canvasCustom;
    /// <summary>
    /// Campo de tipo Bird utilizado para almacenar o configurar bird data.
    /// </summary>
    [SerializeField] private Bird _birdData;
    /// <summary>
    /// Campo de tipo Image utilizado para almacenar o configurar target image.
    /// </summary>
    [SerializeField] Image _targetImage;

    /// <summary>
    /// Ejecuta la lógica asociada a on button click collection dentro de OnClickCollection.
    /// </summary>
    public void OnButtonClickCollection()
    {
        string buttonTag = gameObject.tag;

        _canvasManagerCollection.UpdateTextBasedOnTag(buttonTag);
    }

    /// <summary>
    /// Ejecuta la lógica asociada a on button click bird dentro de OnClickCollection.
    /// </summary>
    public void OnButtonClickBird()
    {
        string buttonTag = gameObject.tag;

        Image _myButtonImage = gameObject.GetComponent<Image>();


        string _myButtonUpdate = "";


        string _myButtonTitle = gameObject.GetComponentInChildren<TMP_Text>().text;

        Slider _myButtonLevel = gameObject.GetComponentInChildren<Slider>();

        Image fatherPanel = gameObject.transform.parent.GetComponent<Image>();
        
        Debug.Log("Tag del boton pulsado: " + buttonTag);

        _canvasManagerBird.UpdateTextBasedOnTag(_birdData, buttonTag, _myButtonImage, _myButtonTitle, _myButtonUpdate, fatherPanel);
    }

    /// <summary>
    /// Ejecuta la lógica asociada a on button click take bird off dentro de OnClickCollection.
    /// </summary>
    public void OnButtonClickTakeBirdOff()
    {
        string buttonTag = gameObject.tag;
        _canvasManagerBird.ClearOneTag(buttonTag);
    }

    /// <summary>
    /// Ejecuta la lógica asociada a on button click left change custom dentro de OnClickCollection.
    /// </summary>
    public void OnButtonClickLeftChangeCustom()
    {
        string buttonTag = gameObject.tag;
        _canvasCustom.ChangeColor(buttonTag, true, _targetImage);
    }

    /// <summary>
    /// Ejecuta la lógica asociada a on button click right change custom dentro de OnClickCollection.
    /// </summary>
    public void OnButtonClickRightChangeCustom()
    {
        string buttonTag = gameObject.tag;
        _canvasCustom.ChangeColor(buttonTag, false, _targetImage);
    }

}
