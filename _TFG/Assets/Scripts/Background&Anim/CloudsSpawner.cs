using UnityEngine;

public class CloudsSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _cloudsPrefab;    //> Prefab que vamos a spawnear
    [SerializeField] private float      _timepNextSpawns = .5f;

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

    [Header("Amount of Objets for Spawn")]
    [SerializeField] private int _minObjets = 1;   // Mínimo a spawnear
    [SerializeField] private int _maxObjets = 2;   // Máximo a spawnear

    private void Start()
    {
        InvokeRepeating(nameof(SpawnObjeto), 0f, _timepNextSpawns); //> Repetir spawn cada cierto tiempo
    }

    void SpawnObjeto()
    {
        int _amount = Random.Range(_minObjets, _maxObjets + 1);

        for (int i = 0; i < 1; i++)
        {
            Vector3 _spawnPoint = new Vector3(
            Random.Range(xMin, xMax),           //> Posición X
            Random.Range(yMin, yMax),           //> Posición Y
            Random.Range(zMin, zMax)            //> Posición Z
            );

            /// Instancia el objeto
            GameObject _newObject = Instantiate(_cloudsPrefab, _spawnPoint, Quaternion.identity);

            /// Escala aleatoria (igual en X e Y para no deformar el objeto)
            float _scale = Random.Range(_scaleMin, _scaleMax);
            _newObject.transform.localScale = new Vector3(_scale, _scale, 1f);
        }
            
    }
}

