using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Coins : MonoBehaviour
{
    [Header("CoinsNumber")]
    public int _actualCoins = 200;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _textActualCoins; //> Se actualiza el texto de Coins de la UI

    private const int _maxCoins = 1000;
    private const int _minCoins = 0;
    
    void Start()
    {
        ActualiceCoinsUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ActualiceCoinsUI()
    {
        _textActualCoins.text  = _actualCoins.ToString();
    }
}
