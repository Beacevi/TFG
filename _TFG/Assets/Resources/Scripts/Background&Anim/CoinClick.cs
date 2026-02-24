using UnityEngine;

public class CoinClick : MonoBehaviour
{
    
    private void OnMouseDown()
    {
        GameManager.Instance.AddMoney(1);
        Destroy(gameObject);
        //Particulas
        //sonido
    }
}
