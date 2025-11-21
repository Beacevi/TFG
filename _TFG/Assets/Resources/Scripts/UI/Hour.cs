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
    [Header("Text")]
    [SerializeField] private TMP_Text _realClockText; //> Hora exacta del dia
    [SerializeField] private TMP_Text _weekdayText;   //> Dia de la semana

    [Header("Icons")]
    [SerializeField] private GameObject _sunnyIcon;
    [SerializeField] private GameObject _nightIcon;

    private int _lastMinute = -1;
    private int _lastHour   = -1;

    private string _lastWeekday = "";
    void Start()
    {
        UpdateClock();

        UpdateWeekday();

        UpdateIcon();
    }

    void Update()
    {
        DateTime now = DateTime.Now;                                                      //> Si el minuto es diferente al anterior guardado se actualiza el reloj

        if(now.Minute != _lastMinute)
        {
            UpdateClock();
        }
        if (now.Hour != _lastHour)
        {
            UpdateIcon();
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

        _lastMinute = _currentHour.Minute;
        _lastHour   = _currentHour.Hour;

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

    private void UpdateIcon()
    {

        int currentHour = DateTime.Now.Hour;

        if (currentHour >= 18 || currentHour < 6)
        {
            _sunnyIcon.SetActive(false);
            _nightIcon.SetActive(true);
        }
        else
        {
            _sunnyIcon.SetActive(true);
            _nightIcon.SetActive(false);
        }
    }
}
