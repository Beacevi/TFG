using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gems : MonoBehaviour
{
    public  int              _actualGems = 0;
    [SerializeField] private TextMeshProUGUI _textActualGems; //> Se actualiza el texto de Gems de la UI

    private const int       _maxGems = 2;
    private const int       _minGems = 0;


    void Start()
    {
        _actualGems = GameManager.Instance.GetGems();
        ActualiceGemsUI();
    }

    public void ActualiceGemsUI()
    {
        _textActualGems.text = GameManager.Instance.GetGems().ToString();
        GameManager.Instance.SaveGame();
    }
}
