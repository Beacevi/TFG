/**
 * @file CloudsSpawner.cs
 * @brief Gestiona la creación de nubes decorativas en la escena, instanciando prefabs dentro de un área configurable.
 * @author Hortensia Studio
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;

/// <summary>
/// Gestiona la creación de nubes decorativas en la escena, instanciando prefabs dentro de un área configurable.
/// </summary>
public class CloudsSpawner : MonoBehaviour
{
    /// <summary>
    /// Prefab o conjunto de prefabs utilizados para instanciar coin prefab.
    /// </summary>
    [SerializeField] private GameObject _coinPrefab;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar coin chance.
    /// </summary>
    [SerializeField] private int _coinChance = 10; // 1 in 10


    /// <summary>
    /// Prefab o conjunto de prefabs utilizados para instanciar clouds prefab.
    /// </summary>
    [Header("Cloud Prefab")]
    [SerializeField] private GameObject _cloudsPrefab;

    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar spawn rate min.
    /// </summary>
    [Header("Spawn Rate (Random Range)")]
    [SerializeField] private float _spawnRateMin = 0.5f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar spawn rate max.
    /// </summary>
    [SerializeField] private float _spawnRateMax = 1.5f;

    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar x min.
    /// </summary>
    [Header("Area of the Spawn")]
    [SerializeField] private float xMin = -4f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar x max.
    /// </summary>
    [SerializeField] private float xMax = -4f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar y min.
    /// </summary>
    [SerializeField] private float yMin = -7f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar y max.
    /// </summary>
    [SerializeField] private float yMax = 3f;

    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar z min.
    /// </summary>
    [Header("Range of Z")]
    [SerializeField] private float zMin = -0.3f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar z max.
    /// </summary>
    [SerializeField] private float zMax = -0.1f;

    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar scale min.
    /// </summary>
    [Header("Range of Scale")]
    [SerializeField] private float _scaleMin = 0.8f;
    /// <summary>
    /// Campo de tipo float utilizado para almacenar o configurar scale max.
    /// </summary>
    [SerializeField] private float _scaleMax = 1.2f;

    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar min objets.
    /// </summary>
    [Header("Amount of Objects per Spawn")]
    [SerializeField] private int _minObjets = 1;
    /// <summary>
    /// Campo de tipo int utilizado para almacenar o configurar max objets.
    /// </summary>
    [SerializeField] private int _maxObjets = 2;

    /// <summary>
    /// Inicializa el componente cuando la escena ya está cargada y lista para comenzar.
    /// </summary>
    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    /// <summary>
    /// Instancia o coloca loop dentro de la escena.
    /// </summary>
    /// <returns>Instancia o valor de tipo System.Collections.IEnumerator resultante de la operación.</returns>
    private System.Collections.IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnObjeto();

            float nextSpawn = Random.Range(_spawnRateMin, _spawnRateMax);
            yield return new WaitForSeconds(nextSpawn);
        }
    }

    /// <summary>
    /// Instancia o coloca objeto dentro de la escena.
    /// </summary>
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
