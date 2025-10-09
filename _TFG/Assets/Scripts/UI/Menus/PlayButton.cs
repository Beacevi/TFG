using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    [Header("Icons")]
    [SerializeField] private GameObject _openedIcon;
    [SerializeField] private GameObject _closedIcon;

    [Header("Plus")]
    [SerializeField] private Button _buttonPlay;
    private ButtonFunctions _buttonFunctions;

    public bool isOpen = false;


    private void Start()
    {
        _buttonFunctions = GetComponent<ButtonFunctions>();

        _openedIcon.SetActive(true);
        _closedIcon.SetActive(false);
    }
    public void PlayMenu()
    {
        if (isOpen)
        {
            StartCoroutine(_buttonFunctions.InteractibleButton(_buttonPlay, _animator));     
            _animator.SetTrigger("CloseTrigger");

            _openedIcon.SetActive(true);
            _closedIcon.SetActive(false);
            isOpen = false;
        }
        else
        {
            StartCoroutine(_buttonFunctions.InteractibleButton(_buttonPlay, _animator));   
            _animator.SetTrigger("OpenTrigger");

            _closedIcon.SetActive(true);
            _openedIcon.SetActive(false);
            isOpen = true;
        }

    }
}
