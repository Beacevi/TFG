using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Coins : MonoBehaviour
{
    [Header("CoinsNumber")]
    public int _actualCoins = 0;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _textActualCoins; //> Se actualiza el texto de Coins de la UI

    private const int _maxCoins = 1000;
    private const int _minCoins = 0;
    
    void Start()
    {
        _actualCoins = GameManager.Instance.coins;

        ActualiceCoinsUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ActualiceCoinsUI()
    {
        GameManager.Instance.coins = _actualCoins;
        GameManager.Instance.SaveGame();

        _textActualCoins.text  = _actualCoins.ToString();
    }
}
