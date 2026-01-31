using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonFunctions : MonoBehaviour
{
    [Header("Scripts")]
    private HamburguerButton _hamburguerButtonScript;
    private PlayButton _playButtonScript;

    [Header("Panels")]
    [SerializeField] private GameObject _OptionMenuPanel;

    [Header("Plus")]
    [SerializeField] private GameObject _cloudPrefab;
    [SerializeField] private GameObject _myBirds;

    public IEnumerator InteractibleButton(Button button, Animator animator)
    {
        if(button)
            button.interactable = false;
        yield return null;
        float duration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duration);
        if (button)
            button.interactable = true;
    }
    public IEnumerator CloseInteractibleButton(Button button, Animator animator, GameObject panel)
    {
        if (button)
            button.interactable = false;
        yield return null;
        float duration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duration);
        if (button)
            button.interactable = true;
        panel.SetActive(false);
    }
    private void Start()
    {
        _hamburguerButtonScript = GetComponent<HamburguerButton>();
        _playButtonScript = GetComponent<PlayButton>();

        _cloudPrefab.SetActive(true);
    }
    public void OpenMenu()
    {
        _cloudPrefab.SetActive(false);

        _hamburguerButtonScript.HamburguerMenu();
        if(_playButtonScript.isOpen)
            _playButtonScript.PlayMenu();

        _OptionMenuPanel.SetActive(true);
    }

    public void CloseMenu()
    {
        _cloudPrefab.SetActive(true);

        _OptionMenuPanel.SetActive(true);
    }

    public void HideBirds(bool isSeen)
    {
        if(isSeen) _myBirds.SetActive(false);

        else _myBirds.SetActive(true);
    }
}
