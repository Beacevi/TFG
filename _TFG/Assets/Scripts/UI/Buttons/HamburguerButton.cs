using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HamburguerButton : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    [Header("Icons")]
    [SerializeField] private GameObject _openedIcon;
    [SerializeField] private GameObject _closedIcon;

    [Header("Plus")]
    [SerializeField] private Button _buttonHamburguer;
    private bool isOpen = false;


    private void Start()
    {
        _openedIcon.SetActive(true);
        _closedIcon.SetActive(false);
    }
    public void HamburguerMenu()
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
        _buttonHamburguer.interactable = false;
        yield return null;

        float duration = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duration);

        _buttonHamburguer.interactable = true;
    }
}
