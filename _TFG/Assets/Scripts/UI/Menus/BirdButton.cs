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
    [SerializeField] private TMP_Text   _BoostText;

    [Header("Scripts")]
    private ButtonFunctions  _buttonFunctions;

    [Header("Panels")]
    [SerializeField] private GameObject _BirdPanel;

    [Header("Icons")]
    [SerializeField] private GameObject _openedIcon;
    [SerializeField] private GameObject _closedIcon;

    [Header("Plus")]
    [SerializeField] private Button _buttonExpandBird;

    private bool isOpen = false;
    Queue<string> BirdSelected = new Queue<string>();

    private void Start()
    {
        _buttonFunctions = GetComponent<ButtonFunctions>();

        _openedIcon.SetActive(true); 
        _closedIcon.SetActive(false);

        _BirdPanel.SetActive(false);
    }
    public void OpenBirdMenu()
    {
        _BirdPanel.SetActive(true);

        _balloonAnimator.SetTrigger("EditingTrigger");
        _birdAnimator.SetTrigger("EditingTrigger");

        _buttonFunctions.OpenMenu();
    }
    public void CloseBirdMenu()
    {
        _BirdPanel.SetActive(false);

        if(isOpen)
            BirdMenu();

        _balloonAnimator.SetTrigger("NotEditingTrigger");
        _birdAnimator.SetTrigger("NotEditingTrigger");

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
    public void UpdateTextBasedOnTag(string buttonTag, Image buttonImage, TMP_Text buttonTitle)
    {
        if(BirdSelected.Contains(buttonTag))
        {
            return;
        }
        else
        {
            if (BirdSelected.Count >= 3)
                BirdSelected.Dequeue(); //< Expulsa del queu el ultimo tag que hay en el queu

            BirdSelected.Enqueue(buttonTag); //< Añade el tag al principio del queu
            
            if (BirdSelected.Count > 1)
                SwapTagImage();

            _Bird1.tag = buttonTag;
            _Bird1.GetComponentInParent<TMP_Text>().text = buttonTitle.text;
            _Bird1.GetComponent<Image>().sprite = buttonImage.sprite;
        }
        
        _BoostText.text = "No Boost";

        TypeOfBoosts();
    }
    private void SwapTagImage()
    {
        string temp = _Bird2.GetComponentInParent<TMP_Text>().text;
        _Bird2.GetComponentInParent<TMP_Text>().text = _Bird1.GetComponentInParent<TMP_Text>().text;
        string tempTag = _Bird2.tag;
        _Bird2.tag = _Bird1.tag;

        if (BirdSelected.Count > 2)
        {
            _Bird3.GetComponentInParent<TMP_Text>().text = temp;
            _Bird3.tag = tempTag;
        }
        
        Sprite tempImage = _Bird2.GetComponent<Image>().sprite;
        _Bird2.GetComponent<Image>().sprite = _Bird1.GetComponent<Image>().sprite;
        _Bird3.GetComponent<Image>().sprite = tempImage;
    }
    private void TypeOfBoosts()
    {
        if (BirdSelected.Contains("A") && BirdSelected.Contains("B") && BirdSelected.Contains("C"))
        {
            _BoostText.text = "A+B+C";
        }
        if (BirdSelected.Contains("D") && BirdSelected.Contains("F") && BirdSelected.Contains("E"))
        {
            _BoostText.text = "D+F+G";
        }
        else
            _BoostText.text = "No Boost";
    }
    public void ClearOneTag(string tagToRemove)
    {
        if (_Bird1.tag == tagToRemove)
        {
            QuitInfoOfBird(_Bird1);
            if (BirdSelected.Count >= 2)
            {
                ChangeBird1InfoToBird2(_Bird1, _Bird2);
                if (BirdSelected.Count == 3)
                {
                    ChangeBird1InfoToBird2(_Bird2, _Bird3);
                    QuitInfoOfBird(_Bird3);
                }
                else
                {
                    QuitInfoOfBird(_Bird2);
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
            }
        }
        else if (_Bird3.tag == tagToRemove)
        {
            QuitInfoOfBird(_Bird3);
        }

        Queue<string> updatedQueue = new Queue<string>();

        while (BirdSelected.Count > 0)
        {
            string currentTag = BirdSelected.Dequeue();
            if (currentTag != tagToRemove)
            {
                updatedQueue.Enqueue(currentTag);
            }
        }

        BirdSelected = updatedQueue;

        TypeOfBoosts();
    }
    public void ChangeBird1InfoToBird2(GameObject bird1, GameObject bird2)
    {
        bird1.tag = bird2.tag;

        bird1.GetComponentInParent<TMP_Text>().text = bird2.GetComponentInParent<TMP_Text>().text;

        bird1.GetComponent<Image>().sprite = bird2.GetComponent<Image>().sprite;

        if (bird1 == _Bird1 && BirdSelected.Count == 3)
        {
            bird2.tag = _Bird3.tag;

            bird2.GetComponentInParent<TMP_Text>().text = _Bird3.GetComponentInParent<TMP_Text>().text;

            bird2.GetComponent<Image>().sprite = _Bird3.GetComponent<Image>().sprite;
        }
        if(BirdSelected.Count == 2)
        {
            QuitInfoOfBird(_Bird2);
        }

    }
    public void QuitInfoOfBird(GameObject bird)
    {
        bird.tag = "Untagged";
        bird.GetComponentInParent<TMP_Text>().text = "Tittle";
        bird.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Buttons/Birds/Bird");
    }
    public void CleanMyBirds()
    {
        BirdSelected.Clear();
    }
}
