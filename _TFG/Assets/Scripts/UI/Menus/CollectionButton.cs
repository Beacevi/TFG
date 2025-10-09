using UnityEngine;

public class CollectionButton : MonoBehaviour
{
    [Header("Scripts")]
    private ButtonFunctions  _buttonFunctions;

    [Header("Panels")]
    [SerializeField] private GameObject _CollectionPanel;
    [SerializeField] private GameObject _AllCollection;
    [SerializeField] private GameObject _InfoCollection;
    [SerializeField] private GameObject _ButtonCloseCollection; //>Boton que cierra el panel de collection

    private void Start()
    {
        _buttonFunctions = GetComponent<ButtonFunctions>();

        _CollectionPanel.SetActive(false);
        _InfoCollection.SetActive(false);
    }

    public void OpenCollectionMenu()
    {
        _CollectionPanel.SetActive(true);

        _buttonFunctions.OpenMenu();
    }

    public void CloseCollectionMenu()
    {
        _CollectionPanel.SetActive(false);

        _buttonFunctions.CloseMenu();
    }

    public void OpenInfo() ///Con un sistema de tags se cambia el texto de la carta
    {
        _AllCollection.SetActive(false);
        _ButtonCloseCollection.SetActive(false);

        _InfoCollection.SetActive(true);
    }
    public void CloseInfo()
    {
        _AllCollection.SetActive(true);
        _ButtonCloseCollection.SetActive(true);

        _InfoCollection.SetActive(false);
    }

    
}
