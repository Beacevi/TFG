using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BirdButton : MonoBehaviour
{
    [Header("Animators")]
    [SerializeField] private Animator _animator; 
    [SerializeField] private Animator _balloonAnimator; 
    [SerializeField] private Animator _birdAnimator; 

    [Header("BirdSelected & Boost")]
    [SerializeField] private GameObject _Bird1;
    [SerializeField] private GameObject _Bird2;
    [SerializeField] private GameObject _Bird3;
    [SerializeField] private GameObject _MyBird1;
    [SerializeField] private GameObject _MyBird2;
    [SerializeField] private GameObject _MyBird3;
    [SerializeField] private TMP_Text   _BoostText;
    [SerializeField] private GameObject _BoostImage;
    [SerializeField] private Sprite     _NoBird;
    [SerializeField] private Button     _Deletebirdbutton;

    [Header("Scripts")]
    private ButtonFunctions  _buttonFunctions;
    private BoostsManager    _boostManager;

    [Header("Panels")]
    [SerializeField] private GameObject _BirdPanel;

    [Header("Icons")]
    [SerializeField] private GameObject _openedIcon;
    [SerializeField] private GameObject _closedIcon;

    [Header("Plus")]
    [SerializeField] private Button _buttonExpandBird;

    private bool isOpen        = false;
    public Queue<string> BirdSelected = new Queue<string>();

    private Image _panelBird1;
    private Image _panelBird2;
    private Image _panelBird3;

    public List<GameObject> _listOfAvailableBirds;
    public List<Bird> _listOfScriptableObjectBirds;

    Button button;

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


    [Header("Feedback")]
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private float _scaleDuration;
    [SerializeField] private float _scaleMultiplier;

    [SerializeField] private Image _imageBird1;
    [SerializeField] private Image _imageBird2;
    [SerializeField] private Image _imageBird3;

    [SerializeField] private Image _frameBird1;
    [SerializeField] private Image _frameBird2;
    [SerializeField] private Image _frameBird3;
    private Color _defaultFrameColor;

    private void Start()
    {
        _buttonFunctions = GetComponent<ButtonFunctions>();
        _boostManager    = GetComponent<BoostsManager>();
        _audioSource = GetComponent<AudioSource>();


    if (_frameBird1 != null)
        {
            _defaultFrameColor = _frameBird1.color;
        }
        

        if(_MyBird1 != null && _MyBird2 != null && _MyBird3 != null)
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
    
    private bool IsValidCombo(Queue<string> queue)
    {
        string[] tags = queue.ToArray();

        if (tags.Length != 3)
            return false;

        // Obtener posiciones en grid
        Vector2Int a = birdGrid[tags[0]];
        Vector2Int b = birdGrid[tags[1]];
        Vector2Int c = birdGrid[tags[2]];

        Vector2Int ab = b - a;
        Vector2Int bc = c - b;

        // Normalizar dirección (evita problemas de magnitud)
        Vector2Int dirAB = new Vector2Int(
            Mathf.Clamp(ab.x, -1, 1),
            Mathf.Clamp(ab.y, -1, 1)
        );

        Vector2Int dirBC = new Vector2Int(
            Mathf.Clamp(bc.x, -1, 1),
            Mathf.Clamp(bc.y, -1, 1)
        );

        // Deben seguir la misma dirección
        if (dirAB != dirBC)
            return false;

        // Debe ser línea recta (horizontal, vertical o diagonal)
        bool horizontal = dirAB.y == 0 && dirAB.x != 0;
        bool vertical   = dirAB.x == 0 && dirAB.y != 0;
        bool diagonal   = Mathf.Abs(dirAB.x) == 1 && Mathf.Abs(dirAB.y) == 1;

        return horizontal || vertical || diagonal;
    }

    public void OpenBirdMenu(Button button)
    {
        _BirdPanel.SetActive(true);

        _balloonAnimator.SetTrigger("EditingTrigger");
        _birdAnimator.SetTrigger("EditingTrigger");

        _animator.SetTrigger("Open");
        StartCoroutine(_buttonFunctions.InteractibleButton(button, _animator));

        _buttonFunctions.OpenBirdMenu(_listOfAvailableBirds,_listOfScriptableObjectBirds);
    }
    public void CloseBirdMenu(Button button)
    {
        if(isOpen) BirdMenu(true);

        _balloonAnimator.SetTrigger("NotEditingTrigger");
        _birdAnimator   .SetTrigger("NotEditingTrigger");

        if(!isOpen)
        {
            _animator.SetTrigger("Close");
            StartCoroutine(_buttonFunctions.CloseInteractibleButton(button, _animator, _BirdPanel));
        }
        

        _buttonFunctions.CloseMenu();
    }
    public void BirdMenu(bool close)
    {
        if (isOpen)
        {
            StartCoroutine(_buttonFunctions.InteractibleButton(_buttonExpandBird, _animator));
            _animator.SetTrigger("CloseTrigger");

            _openedIcon.SetActive(true);
            _closedIcon.SetActive(false);
            isOpen = false;

            if(close)
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
    private void TypeOfBoosts()
    {
        if (BirdSelected.Count != 3)
        {
            _BoostText.text = "No Boost";
            _BoostImage.SetActive(false);
            return;
        }

        var boost = _boostManager.GetBoostFromSelection(BirdSelected);

        if (boost != null)
        {
            _BoostText.text = boost.name;
            _BoostImage.SetActive(true);
        }
        else
        {
            _BoostText.text = "No Boost";
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
    private void SetGrandpaText(GameObject Bird, string UpdateText)
    {
        Transform grandparentBird = Bird.transform.parent.parent;
        grandparentBird.GetComponent<TMP_Text>().text = UpdateText;
    }
    private string GetGrandpaText(GameObject Bird)
    {
        Transform grandparentBird = Bird.transform.parent.parent;
        return grandparentBird.GetComponent<TMP_Text>().text;
    }
    private void SetGreatGrandpaText(GameObject Bird, string UpdateText)
    {
        Transform greatgrandparentBird = Bird.transform.parent.parent.parent;
        greatgrandparentBird.GetComponent<TMP_Text>().text = UpdateText;
    }
    private string GetGreatGrandpaText(GameObject Bird)
    {
        Transform greatgrandparentBird = Bird.transform.parent.parent.parent;
        return greatgrandparentBird.GetComponent<TMP_Text>().text;
    }
    private void ChangeColorPanel(Image panel, bool isSelected)
    {
        if (panel == null) return;
        Color selectedColor = new Color32(210, 229, 245, 255);
        Color unselectedColor = new Color32(221, 227, 234, 255);

        panel.color = isSelected ? selectedColor : unselectedColor;
    }
    private void SetBird(GameObject bird, string Title, string Update, string Tag, Sprite ImageBird)
    {
        SetGrandpaText(bird, Title);
        SetGreatGrandpaText(bird, Update);

        bird.tag = Tag;

        bird.GetComponent<Image>().sprite = ImageBird;

        if(!_Deletebirdbutton.gameObject.activeSelf)
            _Deletebirdbutton.gameObject.SetActive(true);
    }
public void UpdateTextBasedOnTag(string buttonTag, Image buttonImage, string buttonTitle, string buttonUpdate, Image panel)
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
        SetImage(_Bird1, _MyBird1);
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
        SetImage(_Bird2, _MyBird2);

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
        SetImage(_Bird3, _MyBird3);

        SetImage(_Bird2, _MyBird2);

        if (_audioSource != null)
            _audioSource.Play();

        StartCoroutine(ScalePop(_panelBird1.transform));
        StartCoroutine(ScalePop(_panelBird2.transform));
        StartCoroutine(ScalePop(_panelBird3.transform));

        UpdateFrameColors();

        TypeOfBoosts();
    }

}

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
    private void ReaoganizePanel(ref Image bird1, ref Image bird2)
    {
        ChangeColorPanel(bird2, false); // Limpia el panel anterior
        bird1 = bird2;                  // Copia la referencia del siguiente
        ChangeColorPanel(bird1, true);  // Activa el nuevo panel
    }
    public void ClearOneTag(string tagToRemove)
    {
        if (_Bird1.tag == tagToRemove)
        {
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
    }
    public void CleanMyBirds()
    {
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

}
