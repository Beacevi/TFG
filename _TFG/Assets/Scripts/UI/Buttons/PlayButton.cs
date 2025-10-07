using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    [Header("Icons")]
    [SerializeField] private GameObject _openedIcon;
    [SerializeField] private GameObject _closedIcon;

    [Header("Plus")]
    [SerializeField] private Button _buttonPlay;

    private bool isOpen = false;


    private void Start()
    {
        _openedIcon.SetActive(true);
        _closedIcon.SetActive(false);
    }
    public void PlayMenu()
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
        _buttonPlay.interactable = false;

        yield return null;

        float duration = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duration);

        _buttonPlay.interactable = true;
    }
}
