using UnityEngine;

public class GameManager : MonoBehaviour
{
    private BalloonManager _balloon; 
    private Player _player;
    void Start()
    {
        _balloon = GetComponent<BalloonManager>();
        _player  = GetComponent<Player>();


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
