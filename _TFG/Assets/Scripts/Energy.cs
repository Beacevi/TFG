
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Energy : MonoBehaviour
{
    public const float _time = 10f; //> Segundos que tienen que pasar para que se suba uno de enrgía
    public int _actualEnergy; //> Energia del jugador
    private const int _incrementPerTime = 1; //> Cantidad de Energía que se suma cada vez de cada cierto tiempo(_time)
    private const int _maxEnergy = 4; //> Energía maxima que puede tener el jugador
    private const int _minEnergy = 0; //> Energía minima que puede tener el jugador 

    public TextMeshProUGUI _textActualEnergy; //> Se actualiza el texto de Energy de la UI
    public TextMeshProUGUI _textTimerEnergy; //> Timer del tiempo que queda para que se actualice la siguiente energia

    private bool _isCoroutineRunning = false; //> Booleano para saber si hay una corrutina en marcha
    private float _countdownTimer; //> Variable que funciona como timer 

    void Start()
    {
        _actualEnergy = 0; //> Puesto a modificaciones, por ello es publica.
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

    /// <summary>
    /// Corrutina para que cada cierto tiempo se sume el incremento de energia a la energia del jugador.
    /// LLama a las funciones que actualizan valores en la UI.
    /// </summary>
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

    /// <summary>
    /// Actualiza en la UI  la energía que posee el usuario.
    /// </summary>
    private void ActualiceEnergyUI()
    {
        _textActualEnergy.text = _actualEnergy.ToString();
    }

    /// <summary>
    /// Funcion para actualizar el Timer. Solo se actualiza el timer si la corrutina está funcionando, y si el tiempo que queda es mas de cero.
    /// </summary>
    private void ActuliceTimerUI()
    {
        if (_isCoroutineRunning && _countdownTimer > 0)
        {
            int minutos = Mathf.FloorToInt(_countdownTimer / 60);
            int segundos = Mathf.FloorToInt(_countdownTimer % 60);
            _textTimerEnergy.text = $"{minutos:D2}:{segundos:D2}";
        }
        else
        {
            _textTimerEnergy.text = "00:00";
        }
    }
}