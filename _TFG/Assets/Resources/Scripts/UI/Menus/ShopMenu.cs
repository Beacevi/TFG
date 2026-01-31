using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ShopMenu : MonoBehaviour
{

    [Header("Animators")]
    [SerializeField] private Animator _animator;

    [Header("Panels")]
    [SerializeField] private GameObject _storeMenu;
    [SerializeField] private GameObject _resetStorePanel;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI _timerText;
    private DateTime _nextResetTime;
    private Color _normalColor;
    [SerializeField] private Color _lastHourColor = Color.red;

    [Header("Articles In Store")]
    [SerializeField] private GameObject _Article1;
    [SerializeField] private GameObject _Article2;
    [SerializeField] private GameObject _Article3;

    [Header("Plus")]
    [SerializeField] private GameObject _balloon;
    [SerializeField] private GameObject _birds;

    [Header("Scripts")]
    private ButtonFunctions _buttonFunctions;

    private bool _isResetMessageOpen = false;
    void Start()
    {
        _buttonFunctions = GetComponent<ButtonFunctions>();

        _normalColor   = _timerText.color;
        _nextResetTime = DateTime.Today.AddDays(1);

        InvokeRepeating(nameof(UpdateTimerUI), 0f, 1f);

        _storeMenu.SetActive(false);
    }

    private void UpdateTimerUI()
    {
        TimeSpan remaining = _nextResetTime - DateTime.Now;

        if (remaining.TotalSeconds <= 0)
        {
            // Reinicia el timer a la siguiente medianoche
            _nextResetTime = DateTime.Today.AddDays(1);
            remaining = _nextResetTime - DateTime.Now;
        }

        _timerText.color = remaining.TotalHours < 1 ? _lastHourColor : _normalColor;

        if (remaining.TotalHours < 1)
        {
            // Última hora: mostrar horas:minutos:segundos
            _timerText.text = $"{remaining.Minutes:D2}:{remaining.Seconds:D2}";
        }
        else
        {
            // Más de una hora: mostrar solo horas:minutos
            _timerText.text = $"{(int)remaining.TotalHours:D2}:{remaining.Minutes:D2}";
        }
    }

    public void OpenStoreMenu(Button button)
    {
        _storeMenu.SetActive(true);
        
        _animator.SetTrigger("OpenTrigger");
        StartCoroutine(_buttonFunctions.InteractibleButton(button, _animator));

        _birds.SetActive(false);
        _balloon.SetActive(false);

        _buttonFunctions.OpenMenu();
    }
    public void ResetStorePanel()
    {
        if (!_isResetMessageOpen)
        {
            _resetStorePanel.SetActive(true);
            _isResetMessageOpen = true;
        }
        else
        {
            _resetStorePanel.SetActive(false);
            _isResetMessageOpen = false;
        }   
    }
    public void CloseAnimation(Button button)
    {
        
        _animator.SetTrigger("CloseTrigger");
        StartCoroutine(_buttonFunctions.CloseInteractibleButton(button, _animator, _storeMenu));

        Close();


        //_storeMenu.SetActive(false);
    }
    private void Close()
    {
        _resetStorePanel.SetActive(false);

        _birds.SetActive(true);
        _balloon.SetActive(true);
    }

    public void ResetStore()
    {
        //Change articles here
    }

    public void BuyArticle(int article)
    {
        //Buy article logic here

        if (article == 1)
        {
            Buy(_Article1);
        }
        else if (article == 2)
        {
            Buy(_Article2);
        }
        else if (article == 3)
        {
            Buy(_Article3);
        }
    }

    private void Buy(GameObject _article)
    {
        // Implement buy logic here
    }
}

