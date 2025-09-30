using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Coins : MonoBehaviour
{
    public int _actualCoins;
    private const int _maxCoins = 2;
    private const int _minCoins = 0;
    public TextMeshProUGUI _textActualCoins; //> Se actualiza el texto de Coins de la UI
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
        _textActualCoins.text = _actualCoins.ToString()  + "$";
    }
}
