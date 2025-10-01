using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Globalization;
using Unity.VisualScripting;

/// <summary>
/// Codigo que actualiza en la UI la hora y el dia de la semana.
/// </summary>

public class Hour : MonoBehaviour
{
    [SerializeField] private TMP_Text _realClockText; //> Hora exacta del dia
    [SerializeField] private TMP_Text _weekdayText;   //> Dia de la semana

    private int    _lastMinute  = -1;
    private string _lastWeekday = "";

    void Start()
    {
        UpdateClock();

        UpdateWeekday(); 
    }

    void Update()
    {
        DateTime now = DateTime.Now;                                                      //> Si el minuto es diferente al anterior guardado se actualiza el reloj

        if(now.Minute != _lastMinute)
        {
            UpdateClock();
        }

        string currentWeekday = DateTime.Now.ToString("dddd", new CultureInfo("en-US")); //> Actualiza un string que es mas optimo que actualizar la UI todo el tiempo
        if (currentWeekday != _lastWeekday)
        {
            UpdateWeekday();
        }

    }

    
    /// Actualiza el reloj de la UI
    private void UpdateClock()
    {
        DateTime _currentHour = DateTime.Now;
        _lastMinute           = _currentHour.Minute;

        string _formatedCloakText = _currentHour.ToString("hh:mm tt");                  //>Muestra PM y AM en mayusculas
        _formatedCloakText        = _formatedCloakText.Replace("AM", "a.m").Replace("PM", "p.m");

        _realClockText.text = _formatedCloakText;
    }

    /// Actualiza el texto del dia de la semana de la UI, solo cuando se cambia de dia
    private void UpdateWeekday()
    {
        _lastWeekday      = DateTime.Now.ToString("dddd", new CultureInfo("en-US"));
        _weekdayText.text = _lastWeekday;
    }
}
