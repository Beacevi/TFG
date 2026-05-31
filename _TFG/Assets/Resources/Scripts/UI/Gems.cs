/**
 * @file Gems.cs
 * @brief Actualiza la representación visual de gemas en la interfaz de usuario.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Actualiza la representación visual de gemas en la interfaz de usuario.
/// </summary>
public class Gems : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar actual gems.
    /// </summary>
    public  int              _actualGems = 0;
    /// <summary>
    /// Campo de tipo TextMeshProUGUI utilizado para almacenar o configurar text actual gems.
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textActualGems; //> Se actualiza el texto de Gems de la UI

    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar max gems.
    /// </summary>
    private const int       _maxGems = 2;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar min gems.
    /// </summary>
    private const int       _minGems = 0;


    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    void Start()
    {
        _actualGems = GameManager.Instance.GetGems();
        ActualiceGemsUI();
    }

    /// <summary>
    /// Ejecuta la lógica asociada a actualice gems ui dentro de Gems.
    /// </summary>
    public void ActualiceGemsUI()
    {
        _textActualGems.text = GameManager.Instance.GetGems().ToString();
        GameManager.Instance.SaveGame();
    }
}
