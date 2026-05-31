/**
 * @file Energy.cs
 * @brief Actualiza la representación visual de energía o stamina en la interfaz de usuario.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Actualiza la energia cada vez que el jugador se queda sin enegia, a traves de un timer
/// </summary>
public class Energy : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar actual energy.
    /// </summary>
    public int         _actualEnergy;               //> Energia del jugador
    /// <summary>
    /// Campo de tipo Slider utilizado para almacenar o configurar slider.
    /// </summary>
    [SerializeField] private Slider slider;


    /// <summary>
    /// Tiempo o duración asociado a countdown timer.
    /// </summary>
    [Header("Timer")]
    [SerializeField] private float       _countdownTimer;             //> Variable que funciona como timer 
    /// <summary>
    /// Tiempo o duración asociado a increment per time.
    /// </summary>
    [SerializeField] private const int   _incrementPerTime   = 1;     //> Cantidad de Energ�a que se suma cada vez de cada cierto tiempo(_time)
    /// <summary>
    /// Tiempo o duración asociado a time.
    /// </summary>
    [SerializeField] private const float _time = 10f;   //> Segundos que tienen que pasar para que se suba uno de enrg�a

    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar max energy.
    /// </summary>
    [Header("Range of Energy")]
    [SerializeField] private const int   _maxEnergy          = 200;     //> Energ�a maxima que puede tener el jugador
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar min energy.
    /// </summary>
    [SerializeField] private const int   _minEnergy          = 0;     //> Energ�a minima que puede tener el jugador 

    /// <summary>
    /// Indica si is coroutine running está activo o habilitado.
    /// </summary>
    private bool        _isCoroutineRunning = false; //> Booleano para saber si hay una corrutina en marcha

    /// <summary>
    /// Campo de tipo TextMeshProUGUI utilizado para almacenar o configurar text actual energy.
    /// </summary>
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _textActualEnergy;        //> Se actualiza el texto de Energy de la UI
    //[SerializeField] private TextMeshProUGUI _textTimerEnergy;         //> Timer del tiempo que queda para que se actualice la siguiente energia



    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    void Start()
    {
        _actualEnergy = GameManager.Instance.GetEnergy();  //> Puesto a modificaciones, por ello es publica.

        slider.maxValue = _maxEnergy;
        slider.minValue = _minEnergy;

        slider.value = _actualEnergy;

        ActualiceEnergyUI();
        ActuliceTimerUI();
        StartCoroutine(EnergyIncreaseTime());
    }

    /// <summary>
    /// Actualiza la lógica del componente en cada fotograma.
    /// </summary>
    void Update()
    {
        if (_actualEnergy < _maxEnergy && !_isCoroutineRunning)
        {
            StartCoroutine(EnergyIncreaseTime());
        }
    }

    /// Corrutina para que cada cierto tiempo se sume el incremento de energia a la energia del jugador.
    /// LLama a las funciones que actualizan valores en la UI.
    private IEnumerator EnergyIncreaseTime()
    {
        _isCoroutineRunning = true;

        while (_actualEnergy < _maxEnergy)
        {
            _countdownTimer = _time;

            while (_countdownTimer > 0)
            {
                _countdownTimer -= Time.deltaTime;

                ActuliceTimerUI();

                yield return null;
            }

            _actualEnergy += _incrementPerTime;

            ActualiceEnergyUI();
        }

        _isCoroutineRunning = false;
    }

    /// Actualiza en la UI  la energ�a que posee el usuario.
    private void ActualiceEnergyUI()
    {
        GameManager.Instance.SetEnergy(_actualEnergy);
        GameManager.Instance.SaveGame();

        _textActualEnergy.text = _actualEnergy.ToString();
        slider.value = _actualEnergy;
    }

    /// Funcion para actualizar el Timer. Solo se actualiza el timer si la corrutina est� funcionando, y si el tiempo que queda es mas de cero.
    private void ActuliceTimerUI()
    {
        if (_isCoroutineRunning && _countdownTimer > 0)
        {
            int minutos  = Mathf.FloorToInt(_countdownTimer / 60);
            int segundos = Mathf.FloorToInt(_countdownTimer % 60);

            //_textTimerEnergy.text = $"{minutos:D2}:{segundos:D2}";
        }
        else
        {
            //_textTimerEnergy.text = "";
        }
    }
}
