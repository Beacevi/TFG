using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardsMenu : MonoBehaviour
{
    [Header("Animators")]
    [SerializeField] private Animator _animator;

    [Header("Plus")]
    [SerializeField] private GameObject _cloudPrefab;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI _titleText;

    [Header("Panels")]
    [SerializeField] private GameObject _RewardsMenu;
    [SerializeField] private GameObject _upgradesPanel;
    [SerializeField] private GameObject _shipUpgradesPanel;
    [SerializeField] private GameObject _characterUpgradesPanel;
    [SerializeField] private GameObject _dailyStampsPanel;

    private bool isOpen;

    private ButtonFunctions _buttonFunctions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _RewardsMenu.SetActive(false);
        _buttonFunctions = GetComponent<ButtonFunctions>();
        isOpen = false;
    }

    public void OpenAnimation(int num)
    {
        Button button = GetComponent<Button>();
        Close(num);

        _cloudPrefab.SetActive(false);

        if(num == 1)
        {
            OpenUpgradeShip();
        }
        else if (num == 2)
        {
            OpenUpgradeCharacter();
        }
        else if (num == 3)
        {
            OpenDailyStamps();
        }

        if (!isOpen)
        {
            _RewardsMenu.SetActive(true);
            
            StartCoroutine(_buttonFunctions.InteractibleButton(button, _animator));
            _animator.SetTrigger("OpenTrigger");
        }

        isOpen = true;
    }
    public void CloseAnimation(Button button)
    {
        _animator.SetTrigger("CloseTrigger");
        StartCoroutine(_buttonFunctions.CloseInteractibleButton(button, _animator, _RewardsMenu));

        Close(4);

        isOpen = false;
    }
    private void Close(int num)
    {
        _cloudPrefab.SetActive(true);

        _upgradesPanel.SetActive(false);
        _shipUpgradesPanel.SetActive(false);
        _characterUpgradesPanel.SetActive(false);
        _dailyStampsPanel.SetActive(false);
    }

    private void OpenUpgradeShip()
    {
        _titleText.text = "Upgrade Ship";
        _upgradesPanel.SetActive(true);
        _shipUpgradesPanel.SetActive(true);
    }

    private void OpenUpgradeCharacter()
    {
        _titleText.text = "Upgrade Character";
        _upgradesPanel.SetActive(true);
        _characterUpgradesPanel.SetActive(true);
    }

    private void OpenDailyStamps()
    {
        _titleText.text = "Daily Stamps";
        _dailyStampsPanel.SetActive(true);
    }

    public void DailyStampClick(GameObject child)
    {
        Image image = child.GetComponent<Image>();

        if (child.GetComponent<Image>().color != new Color32(255, 255, 255, 255))
        {
            child.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        else
        {
            Transform grandchildTransform = child.transform.GetChild(0);

            grandchildTransform.gameObject.SetActive(true);
        }
                    
    }

}
