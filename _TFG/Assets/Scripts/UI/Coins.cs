using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Coins : MonoBehaviour
{
    [Header("CoinsNumber")]
    public int _actualCoinsTop = 200;
    public int _actualCoinsDown = 250;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _textActualCoinsTop; //> Se actualiza el texto de Coins de la UI
    [SerializeField] private TextMeshProUGUI _textActualCoinsDown;

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
        _textActualCoinsTop.text  = _actualCoinsTop.ToString();
        _textActualCoinsDown.text = _actualCoinsDown.ToString();
    }
}
