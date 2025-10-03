using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gems : MonoBehaviour
{
    public  int             _actualGems = 15;
    [SerializeField] private TextMeshProUGUI _textActualGems; //> Se actualiza el texto de Gems de la UI

    private const int       _maxGems = 2;
    private const int       _minGems = 0;
    
    void Start()
    {
        ActualiceGemsUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ActualiceGemsUI()
    {
        _textActualGems.text = _actualGems.ToString();
    }
}
