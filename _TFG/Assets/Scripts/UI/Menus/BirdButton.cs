using System;
using System.Collections;
using System.Collections.Generic;
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

    public void UpdateTextBasedOnTag(string buttonTag, Image buttonImage, string buttonTitle, string buttonUpdate)
    {
        if(BirdSelected.Contains(buttonTag)) return;
        else
        {
            if (BirdSelected.Count >= 3)
            {
                BirdSelected.Dequeue(); //< Expulsa del queu el ultimo tag que hay en el queu
            }

            BirdSelected.Enqueue(buttonTag); //< Añade el tag al principio del queu
            TypeOfBoosts();
            if (BirdSelected.Count > 1)
            {
                SwapTagImage();
            }

            _Bird1.tag = buttonTag;
            _Bird1.GetComponent<Image>().sprite = buttonImage.sprite;

            SetGrandpaText(_Bird1, buttonTitle);
            SetGreatGrandpaText(_Bird1, buttonUpdate);



            if (BirdSelected.Count == 1)
            {
                _MyBird1.SetActive(true);
                SetImage(_Bird1, _MyBird1);
            }
            else if (BirdSelected.Count == 2)
            {
                _MyBird2.SetActive(true);
                SetImage(_Bird2, _MyBird2);
                SetImage(_Bird1, _MyBird1);
            }
            else
            {
                _MyBird3.SetActive(true);
                SetImage(_Bird3, _MyBird3);
                SetImage(_Bird2, _MyBird2);
                SetImage(_Bird1, _MyBird1);
            }
            
        }
        
    }

    private void SwapTagImage()
    {
        string tempTitle = GetGrandpaText(_Bird2);
        SetGrandpaText(_Bird2, GetGrandpaText(_Bird1));


        string tempUpdate = GetGreatGrandpaText(_Bird2);
        SetGreatGrandpaText(_Bird2, GetGreatGrandpaText(_Bird1));

        string tempTag = _Bird2.tag;
        _Bird2.tag = _Bird1.tag;

        Sprite tempImage = _Bird2.GetComponent<Image>().sprite;
        _Bird2.GetComponent<Image>().sprite = _Bird1.GetComponent<Image>().sprite;

        if (BirdSelected.Count > 2)
        {
            SetGrandpaText(_Bird3, tempTitle);
            SetGreatGrandpaText(_Bird3, tempUpdate);

            _Bird3.tag = tempTag;

            _Bird3.GetComponent<Image>().sprite = tempImage;
        }
    }
    


    /// <summary>
    /// Remove Birds
    /// </summary>
    public void ClearOneTag(string tagToRemove)
    {
        if (_Bird1.tag == tagToRemove)
        {
            QuitInfoOfBird(_Bird1);
            _MyBird1.SetActive(false);

            if (BirdSelected.Count >= 2)
            {
                ChangeBird1InfoToBird2(_Bird1, _Bird2);
                if (BirdSelected.Count == 3)
                {
                    ChangeBird1InfoToBird2(_Bird2, _Bird3);
                    QuitInfoOfBird(_Bird3);
                    _MyBird3.SetActive(false);
                }
                else
                {
                    QuitInfoOfBird(_Bird2);
                    _MyBird2.SetActive(false);
                }
            }
        }
        else if (_Bird2.tag == tagToRemove)
        {
            QuitInfoOfBird(_Bird2);
            if (BirdSelected.Count == 3)
            {
                ChangeBird1InfoToBird2(_Bird2, _Bird3);
                QuitInfoOfBird(_Bird3);
                _MyBird3.SetActive(false);
            }
        }
        else if (_Bird3.tag == tagToRemove) QuitInfoOfBird(_Bird3);

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
    public void ChangeBird1InfoToBird2(GameObject bird1, GameObject bird2)
    {
        bird1.tag = bird2.tag;

        SetGrandpaText(bird1, GetGrandpaText(bird2));
        SetGreatGrandpaText(bird1, GetGreatGrandpaText(bird2));

        bird1.GetComponent<Image>().sprite = bird2.GetComponent<Image>().sprite;

        if (bird1 == _Bird1 && BirdSelected.Count == 3)
        {
            bird2.tag = _Bird3.tag;

            SetGrandpaText(bird2, GetGrandpaText(_Bird3));
            SetGreatGrandpaText(bird2, GetGreatGrandpaText(_Bird3));

            bird2.GetComponent<Image>().sprite = _Bird3.GetComponent<Image>().sprite;
        }
        else if(BirdSelected.Count == 2)
        {
            QuitInfoOfBird(_Bird2);
            _MyBird2.SetActive(false);
        }
            
    }
    public void QuitInfoOfBird(GameObject bird)
    {
        bird.tag = "Untagged";

        SetGrandpaText(bird, "Tittle");
        SetGreatGrandpaText(bird, "Update Today");

        bird.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Buttons/Birds/Bird");
    }
    public void CleanMyBirds()
    {
        if(BirdSelected.Count()>3) QuitInfoOfBird(_Bird3);
        else if(BirdSelected.Count()>2) QuitInfoOfBird(_Bird2);
        QuitInfoOfBird(_Bird1);
        BirdSelected.Clear();
        _MyBird1.SetActive(false);
        _MyBird2.SetActive(false);
        _MyBird3.SetActive(false);
    }
}
