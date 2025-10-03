using UnityEngine;

public class HamburguerButton : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private GameObject _openedIcon;
    [SerializeField] private GameObject _closedIcon;

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
            animator.SetTrigger("CloseTrigger");
            _openedIcon.SetActive(true);
            _closedIcon.SetActive(false);
            isOpen = false;
        }
        else
        {
            animator.SetTrigger("OpenTrigger");
            _closedIcon.SetActive(true);
            _openedIcon.SetActive(false);
            isOpen = true;
        }

    }
}
