using UnityEngine;

[CreateAssetMenu(fileName = "Bird", menuName = "Scriptable Objects/Bird")]
public class Bird : ScriptableObject
{
    public bool obtenido = false;
    public GameObject birdPrefab;
    public string birdName;
    public int rondasNecesarias;
}
