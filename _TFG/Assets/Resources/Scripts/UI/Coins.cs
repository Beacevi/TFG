/**
 * @file Coins.cs
 * @brief Actualiza la representación visual de monedas en la interfaz de usuario.
 * @author Hortensia Studio - Beatriz Ceballos Vidal
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Actualiza la representación visual de monedas en la interfaz de usuario.
/// </summary>
public class Coins : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar actual coins.
    /// </summary>
    [Header("CoinsNumber")]
    public int _actualCoins = 0;

    /// <summary>
    /// Campo de tipo TextMeshProUGUI utilizado para almacenar o configurar text actual coins.
    /// </summary>
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _textActualCoins; //> Se actualiza el texto de Coins de la UI

    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar max coins.
    /// </summary>
    private const int _maxCoins = 1000;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar min coins.
    /// </summary>
    private const int _minCoins = 0;

    /// <summary>
    /// Referencia al gestor encargado de game manager.
    /// </summary>
    [SerializeField] private GameManager _gameManager;

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    void Start()
    {
        if(_gameManager != null)
        {
            _actualCoins = _gameManager.GetMoney();
        }

        ActualiceCoinsUI();
    }

    // Update is called once per frame
    /// <summary>
    /// Actualiza la lógica del componente en cada fotograma.
    /// </summary>
    void Update()
    {
        
    }

    /// <summary>
    /// Ejecuta la lógica asociada a actualice coins ui dentro de Coins.
    /// </summary>
    private void ActualiceCoinsUI()
    {
        _textActualCoins.text  = _actualCoins.ToString();
    }
}
