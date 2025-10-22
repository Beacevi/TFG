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
    [SerializeField] private Animator _animator; //>Animator del boton
    [SerializeField] private Animator _balloonAnimator; //>Animator del glovo
    [SerializeField] private Animator _birdAnimator; //>Animator del glovo

    [Header("BirdSelected & Boost")]
    [SerializeField] private GameObject _Bird1;
    [SerializeField] private GameObject _Bird2;
    [SerializeField] private GameObject _Bird3;
    [SerializeField] private GameObject _MyBird1;
    [SerializeField] private GameObject _MyBird2;
    [SerializeField] private GameObject _MyBird3;
    [SerializeField] private TMP_Text   _BoostText;

    [Header("Scripts")]
    private ButtonFunctions  _buttonFunctions;
    private Boosts           _boostManager;

    [Header("Panels")]
    [SerializeField] private GameObject _BirdPanel;

    [Header("Icons")]
    [SerializeField] private GameObject _openedIcon;
    [SerializeField] private GameObject _closedIcon;

    [Header("Plus")]
    [SerializeField] private Button _buttonExpandBird;

    private bool isOpen        = false;
    public Queue<string> BirdSelected = new Queue<string>();

    [SerializeField] private Image _panelBird1;
    [SerializeField] private Image _panelBird2;
    [SerializeField] private Image _panelBird3;

    private void Start()
    {
        _buttonFunctions = GetComponent<ButtonFunctions>();
        _boostManager    = GetComponent<Boosts>();

        _MyBird1.SetActive(false);
        _MyBird2.SetActive(false);
        _MyBird3.SetActive(false);

        _openedIcon.SetActive(true); 
        _closedIcon.SetActive(false);

        _BirdPanel.SetActive(false);
    }
    public void OpenBirdMenu()
    {
        _BirdPanel.SetActive(true);

        _balloonAnimator.SetTrigger("EditingTrigger");
        _birdAnimator   .SetTrigger("EditingTrigger");

        _buttonFunctions.OpenMenu();
    }
    public void CloseBirdMenu()
    {
        _BirdPanel.SetActive(false);

        if(isOpen) BirdMenu();

        _balloonAnimator.SetTrigger("NotEditingTrigger");
        _birdAnimator   .SetTrigger("NotEditingTrigger");

        _buttonFunctions.CloseMenu();
    }
    public void BirdMenu()
    {
        if (isOpen)
        {
            StartCoroutine(_buttonFunctions.InteractibleButton(_buttonExpandBird, _animator));
            _animator.SetTrigger("CloseTrigger");

            _openedIcon.SetActive(true);
            _closedIcon.SetActive(false);
            isOpen = false;
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
        var boost = _boostManager.GetBoostFromSelection(BirdSelected);

        if (boost != null)
        {
            _BoostText.text = boost.name;
        }
        else
        {
            _BoostText.text = "No Boost";
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
    }
    public void UpdateTextBasedOnTag(string buttonTag, Image buttonImage, string buttonTitle, string buttonUpdate, Image panel)
    {
        if(BirdSelected.Contains(buttonTag)) return;
        else
        {
            if (BirdSelected.Count >= 3)
            {
                BirdSelected.Dequeue(); //< Expulsa del queu el ultimo tag que hay en el queu
                ChangeColorPanel(_panelBird3, false);
            }

            BirdSelected.Enqueue(buttonTag); //< Añade el tag al principio del queu
            TypeOfBoosts();
            if (BirdSelected.Count > 1)
            {
                SwapTagImage();
            }

            SetBird(_Bird1, buttonTitle, buttonUpdate, buttonTag, buttonImage.sprite);

            _panelBird1 = panel;
            ChangeColorPanel(_panelBird1, true);

            if (BirdSelected.Count == 1)
            {
                _MyBird1.SetActive(true);
            }
            else if (BirdSelected.Count == 2)
            {
                _MyBird2.SetActive(true);
                SetImage(_Bird2, _MyBird2);
            }
            else
            {
                _MyBird3.SetActive(true);
                SetImage(_Bird3, _MyBird3);
                SetImage(_Bird2, _MyBird2);
            }

            SetImage(_Bird1, _MyBird1);

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
        SetBird(bird, "Tittle", "Update Today", "Untagged", Resources.Load<Sprite>("Birds/UnselectedBird"));
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
                    _panelBird3 = null;
                }
                else
                {
                    QuitInfoOfBird(_Bird2);
                    _MyBird2.SetActive(false);
                    _panelBird2 = null;
                }

                ChangeColorPanel(_panelBird1, true);
            }
            else
            {
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
                _panelBird3 = null;
            }
            else
            {
                _MyBird2.SetActive(false);

                ChangeColorPanel(_panelBird2, false);
                ReaoganizePanel(ref _panelBird2, ref _panelBird1);

                _panelBird2 = null;
            }

            
        }
        else if (_Bird3.tag == tagToRemove)
        {
            QuitInfoOfBird(_Bird3);

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

        TypeOfBoosts();
    }
    public void CleanMyBirds()
    {
        if (BirdSelected.Count() >= 2)
        {
            QuitInfoOfBird(_Bird2);
            _MyBird2.SetActive(false);
            ChangeColorPanel(_panelBird2, false);
            _panelBird2 = null;
            if (BirdSelected.Count() == 3)
            {
                QuitInfoOfBird(_Bird3);
                _MyBird3.SetActive(false);
                ChangeColorPanel(_panelBird3, false);
                _panelBird3 = null;
            }
        }

        QuitInfoOfBird(_Bird1);

        BirdSelected.Clear();

        _MyBird1.SetActive(false);
        
        ChangeColorPanel(_panelBird1, false);
        
        _panelBird1 = null;
        
        TypeOfBoosts();
    }
}
