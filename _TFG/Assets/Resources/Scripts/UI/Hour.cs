/**
 * @file Hour.cs
 * @brief Muestra o actualiza la hora dentro de la interfaz del juego.
 * @author Hortensia Studio - Beatriz Ceballos Vidal
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

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
    /// <summary>
    /// Referencia de interfaz utilizada para mostrar o actualizar real clock text.
    /// </summary>
    [Header("Text")]
    [SerializeField] private TMP_Text _realClockText; //> Hora exacta del dia
    /// <summary>
    /// Referencia de interfaz utilizada para mostrar o actualizar weekday text.
    /// </summary>
    [SerializeField] private TMP_Text _weekdayText;   //> Dia de la semana

    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar sunny icon.
    /// </summary>
    [Header("Icons")]
    [SerializeField] private GameObject _sunnyIcon;
    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar night icon.
    /// </summary>
    [SerializeField] private GameObject _nightIcon;

    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar background.
    /// </summary>
    [Header("Background")]
    [SerializeField] private GameObject _background;
    /// <summary>
    /// Campo de tipo Material utilizado para almacenar o configurar day.
    /// </summary>
    [SerializeField] private Material   _day;
    /// <summary>
    /// Campo de tipo Material utilizado para almacenar o configurar night.
    /// </summary>
    [SerializeField] private Material   _night;


    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar last minute.
    /// </summary>
    private int _lastMinute = -1;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar last hour.
    /// </summary>
    private int _lastHour   = -1;

    /// <summary>
    /// Campo de tipo string utilizado para almacenar o configurar last weekday.
    /// </summary>
    private string _lastWeekday = "";
    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    void Start()
    {
        UpdateClock();

        UpdateWeekday();

        UpdateIcon();
    }

    /// <summary>
    /// Actualiza la lógica del componente en cada fotograma.
    /// </summary>
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

    /// <summary>
    /// Actualiza icon para reflejar el estado actual del sistema.
    /// </summary>
    private void UpdateIcon()
    {
        if(_sunnyIcon != null && _background != null && _nightIcon != null)
        {
            int currentHour = DateTime.Now.Hour;

            if (currentHour >= 18 || currentHour < 6)
            {
                _sunnyIcon.SetActive(false);
                _background.GetComponent<Renderer>().material = _night;
                _nightIcon.SetActive(true);
            }
            else
            {
                _sunnyIcon.SetActive(true);
                _background.GetComponent<Renderer>().material = _day;
                _nightIcon.SetActive(false);
            }
        }

        
    }
}
