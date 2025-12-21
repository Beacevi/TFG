
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
    public int         _actualEnergy;               //> Energia del jugador
    [SerializeField] private Slider slider;


    [Header("Timer")]
    [SerializeField] private float       _countdownTimer;             //> Variable que funciona como timer 
    [SerializeField] private const int   _incrementPerTime   = 1;     //> Cantidad de Energía que se suma cada vez de cada cierto tiempo(_time)
    [SerializeField] private const float _time = 10f;   //> Segundos que tienen que pasar para que se suba uno de enrgía

    [Header("Range of Energy")]
    [SerializeField] private const int   _maxEnergy          = 200;     //> Energía maxima que puede tener el jugador
    [SerializeField] private const int   _minEnergy          = 0;     //> Energía minima que puede tener el jugador 

    private bool        _isCoroutineRunning = false; //> Booleano para saber si hay una corrutina en marcha

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _textActualEnergy;        //> Se actualiza el texto de Energy de la UI
    //[SerializeField] private TextMeshProUGUI _textTimerEnergy;         //> Timer del tiempo que queda para que se actualice la siguiente energia
    



    void Start()
    {
        _actualEnergy = GameManager.Instance.energy;  //> Puesto a modificaciones, por ello es publica.

        slider.maxValue = _maxEnergy;
        slider.minValue = _minEnergy;

        slider.value = _actualEnergy;

        ActualiceEnergyUI();
        ActuliceTimerUI();
        StartCoroutine(EnergyIncreaseTime());
    }

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

    /// Actualiza en la UI  la energía que posee el usuario.
    private void ActualiceEnergyUI()
    {
        GameManager.Instance.energy = _actualEnergy;
        GameManager.Instance.SaveGame();

        _textActualEnergy.text = _actualEnergy.ToString();
        slider.value = _actualEnergy;
    }

    /// Funcion para actualizar el Timer. Solo se actualiza el timer si la corrutina está funcionando, y si el tiempo que queda es mas de cero.
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