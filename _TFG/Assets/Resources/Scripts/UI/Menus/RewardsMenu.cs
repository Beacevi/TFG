/**
 * @file RewardsMenu.cs
 * @brief Controla la presentación y reclamación de recompensas desde la interfaz.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla la presentación y reclamación de recompensas desde la interfaz.
/// </summary>
public class RewardsMenu : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo Animator utilizado para almacenar o configurar animator.
    /// </summary>
    [Header("Animators")]
    [SerializeField] private Animator _animator;

    /// <summary>
    /// Prefab o conjunto de prefabs utilizados para instanciar cloud prefab.
    /// </summary>
    [Header("Plus")]
    [SerializeField] private GameObject _cloudPrefab;

    /// <summary>
    /// Referencia de interfaz utilizada para mostrar o actualizar title text.
    /// </summary>
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _titleText;

    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar rewards menu.
    /// </summary>
    [Header("Panels")]
    [SerializeField] private GameObject _RewardsMenu;
    /// <summary>
    /// Panel de interfaz asociado a upgrades panel.
    /// </summary>
    [SerializeField] private GameObject _upgradesPanel;
    /// <summary>
    /// Panel de interfaz asociado a ship upgrades panel.
    /// </summary>
    [SerializeField] private GameObject _shipUpgradesPanel;
    /// <summary>
    /// Panel de interfaz asociado a character upgrades panel.
    /// </summary>
    [SerializeField] private GameObject _characterUpgradesPanel;
    /// <summary>
    /// Panel de interfaz asociado a daily stamps panel.
    /// </summary>
    [SerializeField] private GameObject _dailyStampsPanel;

    /// <summary>
    /// Indica si is open está activo o habilitado.
    /// </summary>
    private bool isOpen;

    /// <summary>
    /// Botón de interfaz asociado a button functions.
    /// </summary>
    private ButtonFunctions _buttonFunctions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    void Start()
    {
        if(_RewardsMenu!= null)
        {
        _RewardsMenu.SetActive(false);
        _buttonFunctions = GetComponent<ButtonFunctions>();
        isOpen = false;
        }

    }

    /// <summary>
    /// Abre animation dentro del flujo de interfaz.
    /// </summary>
    /// <param name="num">Parámetro num empleado por el método.</param>
    public void OpenAnimation(int num)
    {
        Button button = GetComponent<Button>();
        Close();

        _cloudPrefab.SetActive(false);

        if(num == 1)
        {
            OpenUpgradeShip();
        }
        else if (num == 2)
        {
            OpenUpgradeCharacter();
        }
        else if (num == 3)
        {
            OpenDailyStamps();
        }

        if (!isOpen)
        {
            _RewardsMenu.SetActive(true);
            
            StartCoroutine(_buttonFunctions.InteractibleButton(button, _animator));
            _animator.SetTrigger("OpenTrigger");
        }

        isOpen = true;
    }
    /// <summary>
    /// Cierra animation dentro del flujo de interfaz.
    /// </summary>
    /// <param name="button">Parámetro button empleado por el método.</param>
    public void CloseAnimation(Button button)
    {
        _animator.SetTrigger("CloseTrigger");
        StartCoroutine(_buttonFunctions.CloseInteractibleButton(button, _animator, _RewardsMenu));

        Close();

        isOpen = false;
    }
    /// <summary>
    /// Cierra elemento dentro del flujo de interfaz.
    /// </summary>
    private void Close()
    {
        _cloudPrefab.SetActive(true);

        _upgradesPanel.SetActive(false);
        _shipUpgradesPanel.SetActive(false);
        _characterUpgradesPanel.SetActive(false);
        _dailyStampsPanel.SetActive(false);
    }

    /// <summary>
    /// Abre upgrade ship dentro del flujo de interfaz.
    /// </summary>
    private void OpenUpgradeShip()
    {
        _titleText.text = "Upgrade Ship";
        _upgradesPanel.SetActive(true);
        _shipUpgradesPanel.SetActive(true);
    }

    /// <summary>
    /// Abre upgrade character dentro del flujo de interfaz.
    /// </summary>
    private void OpenUpgradeCharacter()
    {
        _titleText.text = "Upgrade Character";
        _upgradesPanel.SetActive(true);
        _characterUpgradesPanel.SetActive(true);
    }

    /// <summary>
    /// Abre daily stamps dentro del flujo de interfaz.
    /// </summary>
    private void OpenDailyStamps()
    {
        _titleText.text = "Daily Stamps";
        _dailyStampsPanel.SetActive(true);
    }

    /// <summary>
    /// Ejecuta la lógica asociada a daily stamp click dentro de RewardsMenu.
    /// </summary>
    /// <param name="child">Parámetro child empleado por el método.</param>
    public void DailyStampClick(GameObject child)
    {
        Debug.Log("Hola");
        Image image = child.GetComponent<Image>();


        if (image == null)
        {
            Debug.Log("El objeto no tiene Image");
            return;
        }

       

        if (child.GetComponent<Image>().color != Color.white)
        {
            child.GetComponent<Image>().color = Color.white;
        }
        else
        {
            if (child.transform.childCount > 0)
            {
                Transform grandchildTransform = child.transform.GetChild(0);
                grandchildTransform.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("No hay hijo en �ndice 0");
            }
        }
                    
    }
    /// <summary>
    /// Campo de tipo CSVReader utilizado para almacenar o configurar reader.
    /// </summary>
    public CSVReader reader;

    /// <summary>
    /// Ejecuta la lógica asociada a buy upgrade dentro de RewardsMenu.
    /// </summary>
    public void BuyUpgrade()
    {
        Debug.Log("BuyUpgrade called");
        if (_characterUpgradesPanel.activeSelf)
        {
            UpgradeCharacter();
        }
        else if (_dailyStampsPanel.activeSelf)
        {
        }
        else if (_shipUpgradesPanel.activeSelf)
        {
            UpgradeShip();
        }
    }
    /// <summary>
    /// Ejecuta la lógica asociada a upgrade character dentro de RewardsMenu.
    /// </summary>
    private void UpgradeCharacter()
    {
        int cost = reader.GetUpgradeCost(GameManager.Instance.GetCurretLevel());
        bool newlevel = GameManager.Instance.SpendMoney(cost);

        if (newlevel)
        {
            GameManager.Instance.SetANewCurrentLevel();
        }
    }

    /// <summary>
    /// Ejecuta la lógica asociada a upgrade ship dentro de RewardsMenu.
    /// </summary>
    private void UpgradeShip()
    {
        int cost = reader.GetUpgradeCost(GameManager.Instance.GetCurretLevel());
        bool newlevel = GameManager.Instance.SpendMoney(cost);

        if (newlevel)
        {
            GameManager.Instance.SetANewCurrentLevel();
        }
    }
}
