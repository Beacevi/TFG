/**
 * @file ButtonFunctions.cs
 * @brief Agrupa funciones reutilizables llamadas desde botones de la interfaz.
 * @author Hortensia Studio - Beatriz Ceballos Vidal
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Agrupa funciones reutilizables llamadas desde botones de la interfaz.
/// </summary>
public class ButtonFunctions : MonoBehaviour
{
    /// <summary>
    /// Botón de interfaz asociado a hamburguer button script.
    /// </summary>
    [Header("Scripts")]
    private HamburguerButton _hamburguerButtonScript;
    /// <summary>
    /// Botón de interfaz asociado a play button script.
    /// </summary>
    private PlayButton _playButtonScript;

    /// <summary>
    /// Panel de interfaz asociado a option menu panel.
    /// </summary>
    [Header("Panels")]
    [SerializeField] private GameObject _OptionMenuPanel;

    /// <summary>
    /// Prefab o conjunto de prefabs utilizados para instanciar cloud prefab.
    /// </summary>
    [Header("Plus")]
    [SerializeField] private GameObject _cloudPrefab;
    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar my birds.
    /// </summary>
    [SerializeField] private GameObject _myBirds;

    /// <summary>
    /// Ejecuta la lógica asociada a interactible button dentro de ButtonFunctions.
    /// </summary>
    /// <param name="button">Parámetro button empleado por el método.</param>
    /// <param name="animator">Parámetro animator empleado por el método.</param>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    public IEnumerator InteractibleButton(Button button, Animator animator)
    {
        if (button)
        {
            button.interactable = false;
        }
            
        yield return null;
        float duration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duration);
        if (button)
        {
            button.interactable = true;
        }
            
    }
    /// <summary>
    /// Cierra interactible button dentro del flujo de interfaz.
    /// </summary>
    /// <param name="button">Parámetro button empleado por el método.</param>
    /// <param name="animator">Parámetro animator empleado por el método.</param>
    /// <param name="panel">Parámetro panel empleado por el método.</param>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    public IEnumerator CloseInteractibleButton(Button button, Animator animator, GameObject panel)
    {
        if (button)
            button.interactable = false;
        yield return null;
        float duration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duration);
        if (button)
            button.interactable = true;
        panel.SetActive(false);
    }
    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    private void Start()
    {
        _hamburguerButtonScript = GetComponent<HamburguerButton>();
        _playButtonScript = GetComponent<PlayButton>();

        _cloudPrefab.SetActive(true);
    }
    /// <summary>
    /// Abre bird menu dentro del flujo de interfaz.
    /// </summary>
    /// <param name="listAvailableBirds">Parámetro list available birds empleado por el método.</param>
    /// <param name="List<Bird>listScriptableObjectBirds">Parámetro list bird list scriptable object birds empleado por el método.</param>
    public void OpenBirdMenu(List<GameObject> listAvailableBirds, List<Bird>listScriptableObjectBirds)
    {
        _cloudPrefab.SetActive(false);

        //_hamburguerButtonScript.HamburguerMenu();

        if (_playButtonScript.isOpen)
        {
            _playButtonScript.PlayMenu();
        }

        for (int i = 0; i < listScriptableObjectBirds.Count; i++)
        {
            if (listScriptableObjectBirds[i].obtenido)
            {
                listAvailableBirds[i].GetComponent<Button>().interactable = true;

                // El SpriteRenderer está en el hijo "Normal", no en la raíz del prefab
                SpriteRenderer imageBird = listScriptableObjectBirds[i].birdPrefab.GetComponentInChildren<SpriteRenderer>();
                if (imageBird != null)
                    listAvailableBirds[i].GetComponent<Image>().sprite = imageBird.sprite;
                else
                    Debug.LogWarning($"No se encontró SpriteRenderer en el prefab de {listScriptableObjectBirds[i].birdName}");
            }

            if (!listScriptableObjectBirds[i].obtenido)
            {
                listAvailableBirds[i].GetComponent<Button>().interactable = false;
                // El sprite del botón queda con el valor por defecto asignado en el Inspector
            }

        }

        _OptionMenuPanel.SetActive(true);
    }

    /// <summary>
    /// Abre menu dentro del flujo de interfaz.
    /// </summary>
    public void OpenMenu()
    {
        _cloudPrefab.SetActive(false);

        _hamburguerButtonScript.HamburguerMenu();

        if (_playButtonScript.isOpen)
        {
            _playButtonScript.PlayMenu();
        }

        _OptionMenuPanel.SetActive(true);
    }

    /// <summary>
    /// Cierra menu dentro del flujo de interfaz.
    /// </summary>
    public void CloseMenu()
    {
        _cloudPrefab.SetActive(true);

        _OptionMenuPanel.SetActive(true);
    }

    /// <summary>
    /// Oculta birds en la interfaz o en la escena.
    /// </summary>
    /// <param name="isSeen">Parámetro is seen empleado por el método.</param>
    public void HideBirds(bool isSeen)
    {
        if(isSeen) _myBirds.SetActive(false);

        else _myBirds.SetActive(true);
    }
}
