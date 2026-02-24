using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpgradeLevel()
    {
        // Lógica para subir de nivel

        GameManager.Instance.AddMoney(-200); // Insertar valor del CSV
    }
}
