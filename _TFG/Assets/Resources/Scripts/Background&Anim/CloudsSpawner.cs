using UnityEngine;

public class CloudsSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _coinPrefab;
    [SerializeField] private int _coinChance = 10; // 1 in 10


    [Header("Cloud Prefab")]
    [SerializeField] private GameObject _cloudsPrefab;

    [Header("Spawn Rate (Random Range)")]
    [SerializeField] private float _spawnRateMin = 0.5f;
    [SerializeField] private float _spawnRateMax = 1.5f;

    [Header("Area of the Spawn")]
    [SerializeField] private float xMin = -4f;
    [SerializeField] private float xMax = -4f;
    [SerializeField] private float yMin = -7f;
    [SerializeField] private float yMax = 3f;

    [Header("Range of Z")]
    [SerializeField] private float zMin = -0.3f;
    [SerializeField] private float zMax = -0.1f;

    [Header("Range of Scale")]
    [SerializeField] private float _scaleMin = 0.8f;
    [SerializeField] private float _scaleMax = 1.2f;

    [Header("Amount of Objects per Spawn")]
    [SerializeField] private int _minObjets = 1;
    [SerializeField] private int _maxObjets = 2;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private System.Collections.IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnObjeto();

            float nextSpawn = Random.Range(_spawnRateMin, _spawnRateMax);
            yield return new WaitForSeconds(nextSpawn);
        }
    }

    private void SpawnObjeto()
    {
        int _amount = Random.Range(_minObjets, _maxObjets + 1);

        for (int i = 0; i < _amount; i++)
        {
            Vector3 _spawnPoint = new Vector3(
                Random.Range(xMin, xMax),
                Random.Range(yMin, yMax),
                Random.Range(zMin, zMax)
            );

            //GameObject _newObject = Instantiate(_cloudsPrefab, _spawnPoint, Quaternion.identity);

            GameObject prefabToSpawn;

            // 1 out of every _coinChance spawns becomes a coin
            if (Random.Range(1, _coinChance + 1) == 1)
            {
                prefabToSpawn = _coinPrefab;
            }
            else
            {
                prefabToSpawn = _cloudsPrefab;
            }

            GameObject _newObject = Instantiate(prefabToSpawn, _spawnPoint, Quaternion.identity);


            float _scale = Random.Range(_scaleMin, _scaleMax);
            _newObject.transform.localScale = new Vector3(_scale, _scale, 1f);
        }
    }
}
