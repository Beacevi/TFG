/**
 * @file BirdButton.cs
 * @brief Representa un botón de selección asociado a un ave dentro de los menús.
 * @author Hortensia Studio - Beatriz Ceballos Vidal
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Representa un botón de selección asociado a un ave dentro de los menús.
/// </summary>
public class BirdButton : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo Animator utilizado para almacenar o configurar animator.
    /// </summary>
    [Header("Animators")]
    [SerializeField] private Animator _animator;
    /// <summary>
    /// Campo de tipo Animator utilizado para almacenar o configurar balloon animator.
    /// </summary>
    [SerializeField] private Animator _balloonAnimator;
    /// <summary>
    /// Campo de tipo Animator utilizado para almacenar o configurar bird animator.
    /// </summary>
    [SerializeField] private Animator _birdAnimator;

    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar bird 1.
    /// </summary>
    [Header("BirdSelected & Boost")]
    [SerializeField] private GameObject _Bird1;//Bird 1 UI
    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar bird 2.
    /// </summary>
    [SerializeField] private GameObject _Bird2;//Bird 2 UI
    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar bird 3.
    /// </summary>
    [SerializeField] private GameObject _Bird3;//Bird 3 UI
    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar my bird 1.
    /// </summary>
    [SerializeField] private GameObject _MyBird1;//Equipped bird 1
    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar my bird 2.
    /// </summary>
    [SerializeField] private GameObject _MyBird2;//Equipped bird 1
    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar my bird 3.
    /// </summary>
    [SerializeField] private GameObject _MyBird3;//Equipped bird 1
    /// <summary>
    /// Referencia de interfaz utilizada para mostrar o actualizar boost text.
    /// </summary>
    [SerializeField] private TMP_Text _BoostText;
    /// <summary>
    /// Referencia de interfaz utilizada para mostrar o actualizar boost info text.
    /// </summary>
    [SerializeField] private TMP_Text _BoostInfoText;
    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar boost image.
    /// </summary>
    [SerializeField] private GameObject _BoostImage;
    /// <summary>
    /// Campo de tipo Sprite utilizado para almacenar o configurar no bird.
    /// </summary>
    [SerializeField] private Sprite _NoBird;
    /// <summary>
    /// Botón de interfaz asociado a deletebirdbutton.
    /// </summary>
    [SerializeField] private Button _Deletebirdbutton;

    /// <summary>
    /// Botón de interfaz asociado a button functions.
    /// </summary>
    [Header("Scripts")]
    private ButtonFunctions _buttonFunctions;
    /// <summary>
    /// Referencia al gestor encargado de boost manager.
    /// </summary>
    private BoostsManager _boostManager;

    /// <summary>
    /// Panel de interfaz asociado a bird panel.
    /// </summary>
    [Header("Panels")]
    [SerializeField] private GameObject _BirdPanel;

    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar opened icon.
    /// </summary>
    [Header("Icons")]
    [SerializeField] private GameObject _openedIcon;
    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar closed icon.
    /// </summary>
    [SerializeField] private GameObject _closedIcon;

    /// <summary>
    /// Botón de interfaz asociado a button expand bird.
    /// </summary>
    [Header("Plus")]
    [SerializeField] private Button _buttonExpandBird;

    /// <summary>
    /// Indica si is open está activo o habilitado.
    /// </summary>
    private bool isOpen = false;

    // Tracks which Bird ScriptableObject is in each equip slot so we can
    // keep EquippedBirdsData in sync when slots are reorganized on removal.
    /// <summary>
    /// Campo de tipo Bird utilizado para almacenar o configurar bird data slot 1.
    /// </summary>
    private Bird _birdDataSlot1;
    /// <summary>
    /// Campo de tipo Bird utilizado para almacenar o configurar bird data slot 2.
    /// </summary>
    private Bird _birdDataSlot2;
    /// <summary>
    /// Campo de tipo Bird utilizado para almacenar o configurar bird data slot 3.
    /// </summary>
    private Bird _birdDataSlot3;

    /// <summary>
    /// Campo de tipo Queue<string> utilizado para almacenar o configurar bird selected.
    /// </summary>
    public Queue<string> BirdSelected = new Queue<string>();

    /// <summary>
    /// Panel de interfaz asociado a panel bird 1.
    /// </summary>
    private Image _panelBird1;
    /// <summary>
    /// Panel de interfaz asociado a panel bird 2.
    /// </summary>
    private Image _panelBird2;
    /// <summary>
    /// Panel de interfaz asociado a panel bird 3.
    /// </summary>
    private Image _panelBird3;

    /// <summary>
    /// Colección de list of available birds utilizada por este componente.
    /// </summary>
    public List<GameObject> _listOfAvailableBirds;
    /// <summary>
    /// Colección de list of scriptable object birds utilizada por este componente.
    /// </summary>
    public List<Bird> _listOfScriptableObjectBirds;

    /// <summary>
    /// Botón de interfaz asociado a button.
    /// </summary>
    Button button;

    /// <summary>
    /// Colección de bird grid utilizada por este componente.
    /// </summary>
    private Dictionary<string, Vector2Int> birdGrid = new Dictionary<string, Vector2Int>()
    {
        { "A", new Vector2Int(0, 2) },
        { "B", new Vector2Int(1, 2) },
        { "C", new Vector2Int(2, 2) },

        { "D", new Vector2Int(0, 1) },
        { "E", new Vector2Int(1, 1) },
        { "F", new Vector2Int(2, 1) },

        { "G", new Vector2Int(0, 0) },
        { "H", new Vector2Int(1, 0) },
        { "I", new Vector2Int(2, 0) },
    };

    /// <summary>
    /// Campo de tipo AudioSource utilizado para almacenar o configurar audio source.
    /// </summary>
    [Header("Feedback")]
    [SerializeField] private AudioSource _audioSource;

    /// <summary>
    /// Tiempo o duración asociado a scale duration.
    /// </summary>
    [SerializeField] private float _scaleDuration;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar scale multiplier.
    /// </summary>
    [SerializeField] private float _scaleMultiplier;

    /// <summary>
    /// Campo de tipo Image utilizado para almacenar o configurar image bird 1.
    /// </summary>
    [SerializeField] private Image _imageBird1;
    /// <summary>
    /// Campo de tipo Image utilizado para almacenar o configurar image bird 2.
    /// </summary>
    [SerializeField] private Image _imageBird2;
    /// <summary>
    /// Campo de tipo Image utilizado para almacenar o configurar image bird 3.
    /// </summary>
    [SerializeField] private Image _imageBird3;

    /// <summary>
    /// Campo de tipo Image utilizado para almacenar o configurar frame bird 1.
    /// </summary>
    [SerializeField] private Image _frameBird1;
    /// <summary>
    /// Campo de tipo Image utilizado para almacenar o configurar frame bird 2.
    /// </summary>
    [SerializeField] private Image _frameBird2;
    /// <summary>
    /// Campo de tipo Image utilizado para almacenar o configurar frame bird 3.
    /// </summary>
    [SerializeField] private Image _frameBird3;
    /// <summary>
    /// Color utilizado para representar default frame color.
    /// </summary>
    private Color _defaultFrameColor;

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    private void Start()
    {
        _buttonFunctions = GetComponent<ButtonFunctions>();
        _boostManager = GetComponent<BoostsManager>();
        _audioSource = GetComponent<AudioSource>();

        if (_frameBird1 != null)
        {
            _defaultFrameColor = _frameBird1.color;
        }

        if (_MyBird1 != null && _MyBird2 != null && _MyBird3 != null)
        {
            _MyBird1.SetActive(false);
            _MyBird2.SetActive(false);
            _MyBird3.SetActive(false);
        }

        if (_openedIcon != null && _closedIcon != null)
        {
            _openedIcon.SetActive(true);
            _closedIcon.SetActive(false);
        }

        if (_BirdPanel != null && _BoostImage != null && _Deletebirdbutton != null)
        {
            _BirdPanel.SetActive(false);
            _BoostImage.SetActive(false);
            _Deletebirdbutton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Ejecuta la lógica asociada a is valid combo dentro de BirdButton.
    /// </summary>
    /// <param name="queue">Parámetro queue empleado por el método.</param>
    /// <returns>true si la operación se ha completado correctamente; false en caso contrario.</returns>
    private bool IsValidCombo(Queue<string> queue)
    {
        string[] tags = queue.ToArray();

        if (tags.Length != 3)
            return false;

        Vector2Int a = birdGrid[tags[0]];
        Vector2Int b = birdGrid[tags[1]];
        Vector2Int c = birdGrid[tags[2]];

        // Columna vertical: misma X, Y consecutivas (span de 2)
        bool isVertical = (a.x == b.x && b.x == c.x) &&
                        (Mathf.Max(a.y, Mathf.Max(b.y, c.y)) - Mathf.Min(a.y, Mathf.Min(b.y, c.y))) == 2;

        // Fila horizontal: misma Y, X consecutivas (span de 2)
        bool isHorizontal = (a.y == b.y && b.y == c.y) &&
                            (Mathf.Max(a.x, Mathf.Max(b.x, c.x)) - Mathf.Min(a.x, Mathf.Min(b.x, c.x))) == 2;

        return isVertical || isHorizontal;
    }

    /// <summary>
    /// Abre bird menu dentro del flujo de interfaz.
    /// </summary>
    /// <param name="button">Parámetro button empleado por el método.</param>
    public void OpenBirdMenu(Button button)
    {
        _BirdPanel.SetActive(true);

        _balloonAnimator.SetTrigger("EditingTrigger");
        _birdAnimator.SetTrigger("EditingTrigger");

        _animator.SetTrigger("Open");//Esto abre el menu
        StartCoroutine(_buttonFunctions.InteractibleButton(button, _animator));

        _buttonFunctions.OpenBirdMenu(_listOfAvailableBirds, _listOfScriptableObjectBirds);
    }

    /// <summary>
    /// Cierra bird menu dentro del flujo de interfaz.
    /// </summary>
    /// <param name="button">Parámetro button empleado por el método.</param>
    public void CloseBirdMenu(Button button)
    {
        if (isOpen) BirdMenu(true);

        _balloonAnimator.SetTrigger("NotEditingTrigger");
        _birdAnimator.SetTrigger("NotEditingTrigger");

        if (!isOpen)
        {
            _animator.SetTrigger("Close");
            StartCoroutine(_buttonFunctions.CloseInteractibleButton(button, _animator, _BirdPanel));
        }

        _buttonFunctions.CloseMenu();
    }

    /// <summary>
    /// Ejecuta la lógica asociada a bird menu dentro de BirdButton.
    /// </summary>
    /// <param name="close">Parámetro close empleado por el método.</param>
    public void BirdMenu(bool close)
    {
        if (isOpen)
        {
            StartCoroutine(_buttonFunctions.InteractibleButton(_buttonExpandBird, _animator));
            _animator.SetTrigger("CloseTrigger");

            _openedIcon.SetActive(true);
            _closedIcon.SetActive(false);
            isOpen = false;

            if (close)
            {
                _animator.SetTrigger("Close");
                StartCoroutine(_buttonFunctions.CloseInteractibleButton(button, _animator, _BirdPanel));
            }

            return;
        }
        else
        {
            StartCoroutine(_buttonFunctions.InteractibleButton(_buttonExpandBird, _animator));
            _animator.SetTrigger("OpenTrigger");

            _closedIcon.SetActive(true);
            _openedIcon.SetActive(false);
            isOpen = true;
        }
    }

    /// <summary>
    /// Ejecuta la lógica asociada a type of boosts dentro de BirdButton.
    /// </summary>
    private void TypeOfBoosts()
    {
        if (BirdSelected.Count != 3)
        {
            _BoostText.text = "No Boost";
            _BoostInfoText.text = "No boost info available";
            _BoostImage.SetActive(false);

            GameManager.Instance.buffMoneyActive = false;
            GameManager.Instance.buffEnergyActive = false;
            GameManager.Instance.buffBothActive = false;
            return;
        }

        var boost = _boostManager.GetBoostFromSelection(BirdSelected);

        if (boost != null)
        {
            _BoostImage.SetActive(true);

            if (boost.coinMultiplier > 0 && boost.energyMultiplier > 0)
            {
                _BoostText.text = "Money + Energy";
                _BoostInfoText.text = "+10% Money & Energy";
            }
            else if (boost.coinMultiplier > 0)
            {
                _BoostText.text = "Money";
                _BoostInfoText.text = "+10% Money";
            }
            else if (boost.energyMultiplier > 0)
            {
                _BoostText.text = "Energy";
                _BoostInfoText.text = "+10% Energy";
            }
        }
        else
        {
            _BoostText.text = "No Boost";
            _BoostInfoText.text = "No boost info available";
            _BoostImage.SetActive(false);
        }
    }

    /// <summary>
    /// Add Birds
    /// </summary>
    public void SetImage(GameObject bird, GameObject _myBird)
    {
        Sprite uiSprite = bird.GetComponent<Image>().sprite;
        _myBird.GetComponent<SpriteRenderer>().sprite = uiSprite;
    }

    /// <summary>
    /// Establece o actualiza grandpa text dentro del sistema.
    /// </summary>
    /// <param name="Bird">Parámetro bird empleado por el método.</param>
    /// <param name="UpdateText">Parámetro update text empleado por el método.</param>
    private void SetGrandpaText(GameObject Bird, string UpdateText)
    {
        Transform grandparentBird = Bird.transform.parent.parent;
        grandparentBird.GetComponent<TMP_Text>().text = UpdateText;
    }

    /// <summary>
    /// Obtiene grandpa text a partir del estado actual del sistema.
    /// </summary>
    /// <param name="Bird">Parámetro bird empleado por el método.</param>
    /// <returns>Texto resultante de la consulta o procesamiento.</returns>
    private string GetGrandpaText(GameObject Bird)
    {
        Transform grandparentBird = Bird.transform.parent.parent;
        return grandparentBird.GetComponent<TMP_Text>().text;
    }

    /// <summary>
    /// Establece o actualiza great grandpa text dentro del sistema.
    /// </summary>
    /// <param name="Bird">Parámetro bird empleado por el método.</param>
    /// <param name="UpdateText">Parámetro update text empleado por el método.</param>
    private void SetGreatGrandpaText(GameObject Bird, string UpdateText)
    {
        Transform greatgrandparentBird = Bird.transform.parent.parent.parent;
        greatgrandparentBird.GetComponent<TMP_Text>().text = UpdateText;
    }

    /// <summary>
    /// Obtiene great grandpa text a partir del estado actual del sistema.
    /// </summary>
    /// <param name="Bird">Parámetro bird empleado por el método.</param>
    /// <returns>Texto resultante de la consulta o procesamiento.</returns>
    private string GetGreatGrandpaText(GameObject Bird)
    {
        Transform greatgrandparentBird = Bird.transform.parent.parent.parent;
        return greatgrandparentBird.GetComponent<TMP_Text>().text;
    }

    /// <summary>
    /// Cambia change color panel según la interacción o parámetro recibido.
    /// </summary>
    /// <param name="panel">Parámetro panel empleado por el método.</param>
    /// <param name="isSelected">Parámetro is selected empleado por el método.</param>
    private void ChangeColorPanel(Image panel, bool isSelected)
    {
        if (panel == null) return;
        Color selectedColor = new Color32(210, 229, 245, 255);
        Color unselectedColor = new Color32(221, 227, 234, 255);

        panel.color = isSelected ? selectedColor : unselectedColor;
    }

    /// <summary>
    /// Establece o actualiza bird dentro del sistema.
    /// </summary>
    /// <param name="bird">Parámetro bird empleado por el método.</param>
    /// <param name="Title">Parámetro title empleado por el método.</param>
    /// <param name="Update">Parámetro update empleado por el método.</param>
    /// <param name="Tag">Parámetro tag empleado por el método.</param>
    /// <param name="ImageBird">Parámetro image bird empleado por el método.</param>
    private void SetBird(GameObject bird, string Title, string Update, string Tag, Sprite ImageBird)
    {
        SetGrandpaText(bird, Title);
        SetGreatGrandpaText(bird, Update);

        bird.tag = Tag;

        bird.GetComponent<Image>().sprite = ImageBird;

        if (!_Deletebirdbutton.gameObject.activeSelf)
            _Deletebirdbutton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Actualiza text based on tag para reflejar el estado actual del sistema.
    /// </summary>
    /// <param name="birdData">Datos de entrada que se van a procesar.</param>
    /// <param name="buttonTag">Parámetro button tag empleado por el método.</param>
    /// <param name="buttonImage">Parámetro button image empleado por el método.</param>
    /// <param name="buttonTitle">Parámetro button title empleado por el método.</param>
    /// <param name="buttonUpdate">Parámetro button update empleado por el método.</param>
    /// <param name="panel">Parámetro panel empleado por el método.</param>
    public void UpdateTextBasedOnTag(Bird birdData, string buttonTag, Image buttonImage, string buttonTitle, string buttonUpdate, Image panel)
    {
        if (BirdSelected.Contains(buttonTag))
            return;

        if (BirdSelected.Count >= 3)
            return;

        BirdSelected.Enqueue(buttonTag);

        TypeOfBoosts();

        // =========================
        // SLOT 1
        // =========================
        if (BirdSelected.Count == 1)
        {
            SetBird(_Bird1, buttonTitle, buttonUpdate, buttonTag, buttonImage.sprite);

            _panelBird1 = panel;
            ChangeColorPanel(_panelBird1, true);

            _MyBird1.SetActive(true);
            _MyBird1.GetComponent<BirdsReactions>().birdData = birdData;

            Animator anim = _MyBird1.GetComponent<Animator>();
            if (anim != null && birdData.animator != null)
            {
                anim.runtimeAnimatorController = birdData.animator;
            }

            _birdDataSlot1 = birdData;

            EquippedBirdsData.Set(0, birdData, buttonImage.sprite);

            SetImage(_Bird1, _MyBird1);

            GameManager.Instance.SetEquippedBird(1, _MyBird1);

            return;
        }

        // =========================
        // SLOT 2
        // =========================
        if (BirdSelected.Count == 2)
        {
            SetBird(_Bird2, buttonTitle, buttonUpdate, buttonTag, buttonImage.sprite);

            _panelBird2 = panel;
            ChangeColorPanel(_panelBird2, true);

            _MyBird2.SetActive(true);
            _MyBird2.GetComponent<BirdsReactions>().birdData = birdData;

            Animator anim = _MyBird2.GetComponent<Animator>();
            if (anim != null && birdData.animator != null)
            {
                anim.runtimeAnimatorController = birdData.animator;
            }

            _birdDataSlot2 = birdData;
            EquippedBirdsData.Set(1, birdData, buttonImage.sprite);

            SetImage(_Bird2, _MyBird2);

            GameManager.Instance.SetEquippedBird(2, _MyBird2);

            return;
        }

        // =========================
        // SLOT 3
        // =========================
        if (BirdSelected.Count == 3)
        {
            // VALIDAR ANTES DE APLICAR UI FINAL
            if (!IsValidCombo(BirdSelected))
            {
                CleanMyBirds();
                return;
            }

            SetBird(_Bird3, buttonTitle, buttonUpdate, buttonTag, buttonImage.sprite);

            _panelBird3 = panel;
            ChangeColorPanel(_panelBird3, true);

            _MyBird3.SetActive(true);
            _MyBird3.GetComponent<BirdsReactions>().birdData = birdData;

            Animator anim = _MyBird3.GetComponent<Animator>();
            if (anim != null && birdData.animator != null)
            {
                anim.runtimeAnimatorController = birdData.animator;
            }

            _birdDataSlot3 = birdData;
            EquippedBirdsData.Set(2, birdData, buttonImage.sprite);

            SetImage(_Bird3, _MyBird3);
            SetImage(_Bird2, _MyBird2);

            GameManager.Instance.SetEquippedBird(3, _MyBird3);

            if (_audioSource != null)
                _audioSource.Play();

            StartCoroutine(ScalePop(_panelBird1.transform));
            StartCoroutine(ScalePop(_panelBird2.transform));
            StartCoroutine(ScalePop(_panelBird3.transform));

            UpdateFrameColors();

            TypeOfBoosts();
        }
    }

    /// <summary>
    /// Ejecuta la lógica asociada a swap tag image dentro de BirdButton.
    /// </summary>
    private void SwapTagImage()
    {
        string tempTitle = GetGrandpaText(_Bird2);
        string tempUpdate = GetGreatGrandpaText(_Bird2);
        string tempTag = _Bird2.tag;
        Sprite tempImage = _Bird2.GetComponent<Image>().sprite;

        SetBird(_Bird2, GetGrandpaText(_Bird1), GetGreatGrandpaText(_Bird1), _Bird1.tag, _Bird1.GetComponent<Image>().sprite);

        Image temp = _panelBird2;
        _panelBird2 = _panelBird1;
        ChangeColorPanel(_panelBird2, true);

        if (BirdSelected.Count > 2)
        {
            _panelBird3 = temp;
            ChangeColorPanel(_panelBird3, true);

            SetBird(_Bird3, tempTitle, tempUpdate, tempTag, tempImage);
        }
    }

    /// <summary>
    /// Remove Birds
    /// </summary>
    public void QuitInfoOfBird(GameObject bird)
    {
        SetBird(bird, "Bird Name", "No Level", "Untagged", Resources.Load<Sprite>("Birds/UnselectedBird"));
    }

    /// <summary>
    /// Cambia change bird 1 info to bird 2 según la interacción o parámetro recibido.
    /// </summary>
    /// <param name="bird1">Parámetro bird 1 empleado por el método.</param>
    /// <param name="bird2">Parámetro bird 2 empleado por el método.</param>
    private void ChangeBird1InfoToBird2(GameObject bird1, GameObject bird2)
    {
        SetBird(bird1, GetGrandpaText(bird2), GetGreatGrandpaText(bird2), bird2.tag, bird2.GetComponent<Image>().sprite);

        if (bird1 == _Bird1 && BirdSelected.Count == 3)
        {
            SetBird(bird2, GetGrandpaText(_Bird3), GetGreatGrandpaText(_Bird3), _Bird3.tag, _Bird3.GetComponent<Image>().sprite);
        }
        else if (BirdSelected.Count == 2)
        {
            QuitInfoOfBird(_Bird2);
            _MyBird2.SetActive(false);
        }
    }

    /// <summary>
    /// Ejecuta la lógica asociada a reaoganize panel dentro de BirdButton.
    /// </summary>
    /// <param name="bird1">Parámetro bird 1 empleado por el método.</param>
    /// <param name="bird2">Parámetro bird 2 empleado por el método.</param>
    private void ReaoganizePanel(ref Image bird1, ref Image bird2)
    {
        ChangeColorPanel(bird2, false); // Limpia el panel anterior
        bird1 = bird2;                  // Copia la referencia del siguiente
        ChangeColorPanel(bird1, true);  // Activa el nuevo panel
    }

    /// <summary>
    /// Ejecuta la lógica asociada a clear one tag dentro de BirdButton.
    /// </summary>
    /// <param name="tagToRemove">Parámetro tag to remove empleado por el método.</param>
    public void ClearOneTag(string tagToRemove)
    {
        if (_Bird1.tag == tagToRemove)
        {
            // Shift bird data to match the UI shift that follows
            if (BirdSelected.Count >= 2)
            {
                _birdDataSlot1 = _birdDataSlot2;
                _birdDataSlot2 = (BirdSelected.Count == 3) ? _birdDataSlot3 : null;
                _birdDataSlot3 = null;
            }
            else
            {
                _birdDataSlot1 = null;
            }

            QuitInfoOfBird(_Bird1);
            _MyBird1.SetActive(false);
            ChangeColorPanel(_panelBird1, false);

            if (BirdSelected.Count >= 2)
            {
                ChangeBird1InfoToBird2(_Bird1, _Bird2);
                ReaoganizePanel(ref _panelBird1, ref _panelBird2);

                if (BirdSelected.Count == 3)
                {
                    ChangeBird1InfoToBird2(_Bird2, _Bird3);
                    QuitInfoOfBird(_Bird3);
                    _MyBird3.SetActive(false);
                    ReaoganizePanel(ref _panelBird2, ref _panelBird3);
                    _Bird3.GetComponent<Image>().sprite = _NoBird;
                    _panelBird3 = null;
                }
                else
                {
                    QuitInfoOfBird(_Bird2);
                    _MyBird2.SetActive(false);
                    _Bird2.GetComponent<Image>().sprite = _NoBird;
                    _panelBird2 = null;
                }

                ChangeColorPanel(_panelBird1, true);
            }
            else
            {
                _Bird1.GetComponent<Image>().sprite = _NoBird;
                _panelBird1 = null;
            }
        }
        else if (_Bird2.tag == tagToRemove)
        {
            if (BirdSelected.Count == 3)
            {
                _birdDataSlot2 = _birdDataSlot3;
                _birdDataSlot3 = null;
            }
            else
            {
                _birdDataSlot2 = null;
            }

            QuitInfoOfBird(_Bird2);
            ChangeColorPanel(_panelBird2, false);

            if (BirdSelected.Count == 3)
            {
                ChangeBird1InfoToBird2(_Bird2, _Bird3);
                QuitInfoOfBird(_Bird3);
                _MyBird3.SetActive(false);
                ReaoganizePanel(ref _panelBird2, ref _panelBird3);
                _Bird3.GetComponent<Image>().sprite = _NoBird;
                _panelBird3 = null;
            }
            else
            {
                _MyBird2.SetActive(false);
                ChangeColorPanel(_panelBird2, false);
                ReaoganizePanel(ref _panelBird2, ref _panelBird1);
                _Bird2.GetComponent<Image>().sprite = _NoBird;
                _panelBird2 = null;
            }
        }
        else if (_Bird3.tag == tagToRemove)
        {
            _birdDataSlot3 = null;
            QuitInfoOfBird(_Bird3);
            _Bird3.GetComponent<Image>().sprite = _NoBird;
            ChangeColorPanel(_panelBird3, false);
            _panelBird3 = null;
        }

        Queue<string> updatedQueue = new Queue<string>();

        while (BirdSelected.Count > 0)
        {
            string currentTag = BirdSelected.Dequeue();
            if (currentTag != tagToRemove) updatedQueue.Enqueue(currentTag);
        }

        BirdSelected = updatedQueue;

        SetImage(_Bird1, _MyBird1);
        SetImage(_Bird2, _MyBird2);
        SetImage(_Bird3, _MyBird3);

        UpdateFrameColors();
        TypeOfBoosts();
        SyncRegistryAfterRemoval();
    }

    /// <summary>
    /// Limpia o normaliza my birds antes de utilizarlo.
    /// </summary>
    public void CleanMyBirds()
    {
        EquippedBirdsData.ClearAll();
        _birdDataSlot1 = null;
        _birdDataSlot2 = null;
        _birdDataSlot3 = null;

        // Limpiar datos del GameManager
        GameManager.Instance.datosPajaro1 = null;
        GameManager.Instance.datosPajaro2 = null;
        GameManager.Instance.datosPajaro3 = null;

        GameManager.Instance.BirdEquipped1 = null;
        GameManager.Instance.BirdEquipped2 = null;
        GameManager.Instance.BirdEquipped3 = null;

        // Limpiar datos primero
        BirdSelected.Clear();

        // Reset UI slots
        Clean(_MyBird1, _Bird1, ref _panelBird1);
        Clean(_MyBird2, _Bird2, ref _panelBird2);
        Clean(_MyBird3, _Bird3, ref _panelBird3);

        _MyBird1.SetActive(false);
        _MyBird2.SetActive(false);
        _MyBird3.SetActive(false);

        _Deletebirdbutton.gameObject.SetActive(false);

        // Reset frames
        _frameBird1.color = _defaultFrameColor;
        _frameBird2.color = _defaultFrameColor;
        _frameBird3.color = _defaultFrameColor;

        TypeOfBoosts();
    }

    /// <summary>
    /// Limpia o normaliza elemento antes de utilizarlo.
    /// </summary>
    /// <param name="MyBird">Parámetro my bird empleado por el método.</param>
    /// <param name="Bird">Parámetro bird empleado por el método.</param>
    /// <param name="PanelBird">Parámetro panel bird empleado por el método.</param>
    private void Clean(GameObject MyBird, GameObject Bird, ref Image PanelBird)
    {
        if (Bird != null)
        {
            QuitInfoOfBird(Bird);
            Bird.GetComponent<Image>().sprite = _NoBird;
        }

        if (MyBird != null)
            MyBird.SetActive(false);

        if (PanelBird != null)
            ChangeColorPanel(PanelBird, false);

        PanelBird = null;
    }

    /// <summary>
    /// Ejecuta la lógica asociada a scale pop dentro de BirdButton.
    /// </summary>
    /// <param name="target">Parámetro target empleado por el método.</param>
    /// <returns>Corrutina que permite ejecutar la operación de forma diferida en Unity.</returns>
    private IEnumerator ScalePop(Transform target)
    {
        if (target == null) yield break;

        Vector3 startScale = target.localScale;
        Vector3 endScale = startScale * _scaleMultiplier;

        GameObject clone = Instantiate(target.gameObject, target.position, target.rotation, target.root);
        clone.transform.SetAsLastSibling();

        LayoutElement le = clone.GetComponent<LayoutElement>();
        if (le != null)
            le.ignoreLayout = true;

        RectTransform rt = clone.GetComponent<RectTransform>();
        rt.localScale = startScale;

        float time = 0f;

        while (time < _scaleDuration)
        {
            float t = time / _scaleDuration;
            rt.localScale = Vector3.Lerp(startScale, endScale, t);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        rt.localScale = endScale;
        time = 0f;

        while (time < _scaleDuration)
        {
            float t = time / _scaleDuration;
            rt.localScale = Vector3.Lerp(endScale, startScale, t);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        rt.localScale = startScale;
        Destroy(clone);
    }

    /// <summary>
    /// Actualiza frame colors para reflejar el estado actual del sistema.
    /// </summary>
    private void UpdateFrameColors()
    {
        if (BirdSelected.Count == 3)
        {
            _frameBird1.color = Color.yellow;
            _frameBird2.color = Color.yellow;
            _frameBird3.color = Color.yellow;
        }
        else
        {
            _frameBird1.color = _defaultFrameColor;
            _frameBird2.color = _defaultFrameColor;
            _frameBird3.color = _defaultFrameColor;
        }
    }

    /// <summary>
    /// Ejecuta la lógica asociada a sync registry after removal dentro de BirdButton.
    /// </summary>
    private void SyncRegistryAfterRemoval()
    {
        EquippedBirdsData.ClearAll();

        // BirdSelected has already been rebuilt by ClearOneTag at this point.
        // Slots always compact toward slot 1, so we check count to know which
        // slots are in use.
        int count = BirdSelected.Count;

        if (count >= 1 && _birdDataSlot1 != null)
            EquippedBirdsData.Set(0, _birdDataSlot1, _Bird1.GetComponent<Image>().sprite);

        if (count >= 2 && _birdDataSlot2 != null)
            EquippedBirdsData.Set(1, _birdDataSlot2, _Bird2.GetComponent<Image>().sprite);

        if (count >= 3 && _birdDataSlot3 != null)
            EquippedBirdsData.Set(2, _birdDataSlot3, _Bird3.GetComponent<Image>().sprite);
    }
}