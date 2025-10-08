using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonFunctions : MonoBehaviour
{

    [Header("Panels")]
    [SerializeField] private GameObject _OptionsPanel;
    [SerializeField] private GameObject _MainMenuPanel;

    [Header("Plus")]
    [SerializeField] private GameObject _cloudPrefab;
    [SerializeField] private GameObject _myBirds;

    public IEnumerator InteractibleButton(Button button, Animator animator)
    {
        button.interactable = false;
        yield return null;
        float duration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(duration);
        button.interactable = true;
    }

    public void OpenMenu()
    {
        _cloudPrefab.SetActive(false);

        _OptionsPanel.SetActive(true);
        _MainMenuPanel.SetActive(true);
    }

    public void CloseMenu()
    {
        _cloudPrefab.SetActive(true);

        _OptionsPanel.SetActive(true);
        _MainMenuPanel.SetActive(true);
    }

    public void HideBirds(bool isSeen)
    {
        if(isSeen) _myBirds.SetActive(false);

        else _myBirds.SetActive(true);
    }
}
