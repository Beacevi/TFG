using UnityEngine;

[System.Serializable]
public class Coin
{
    public string name;
    public GameObject coinPrefab;
    [Range(0f, 1f)] public float spawnChance = 1f;
}
