using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BirdButton : MonoBehaviour
{
    [Header("Animators")]
    [SerializeField] private Animator _animator; //>Animator del boton
    [SerializeField] private Animator _balloonAnimator; //>Animator del glovo

    [Header("Scripts")]
    [SerializeField] private HamburguerButton _hamburguerButtonScript;

    [Header("Panels")]
    [SerializeField] private GameObject _BirdPanel;
    [SerializeField] private GameObject _OptionsPanel;
    [SerializeField] private GameObject _MainMenuPanel;

    [Header("Icons")]
    [SerializeField] private GameObject _openedIcon;
    [SerializeField] private GameObject _closedIcon;

    [Header("Plus")]
    [SerializeField] private GameObject _cloudPrefab;
    [SerializeField] private Button _buttonExpandBird;

    private bool isOpen = false;


    private void Start()
    {
        _openedIcon.SetActive(true); 
        _closedIcon.SetActive(false);

        _BirdPanel.SetActive(false);
    }
    public void OpenBirdMenu()
    {
        _BirdPanel.SetActive(true);

        _hamburguerButtonScript.HamburguerMenu();

        _balloonAnimator.SetTrigger("EditingTrigger");
        _cloudPrefab.SetActive(false);

        _OptionsPanel.SetActive(false);
        _MainMenuPanel.SetActive(false);
    }
    public void CloseBirdMenu()
    {
        _BirdPanel.SetActive(false);

        BirdMenu();

        _balloonAnimator.SetTrigger("NotEditingTrigger");
        _cloudPrefab.SetActive(true);

        _OptionsPanel.SetActive(true);
        _MainMenuPanel.SetActive(true);
    }
    public void BirdMenu()
    {
        if (isOpen)
        {
            StartCoroutine(InteractibleButton());
            _animator.SetTrigger("CloseTrigger");

            _openedIcon.SetActive(true);
            _closedIcon.SetActive(false);
            isOpen = false;
        }
        else
        {
            StartCoroutine(InteractibleButton());
            _animator.SetTrigger("OpenTrigger");

            _closedIcon.SetActive(true);
            _openedIcon.SetActive(false);
            isOpen = true;
        }

    }
    private IEnumerator InteractibleButton()
    {
        _buttonExpandBird.interactable = false;
        yield return null; 
        float duration = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duration);
        _buttonExpandBird.interactable = true;
    }
}
