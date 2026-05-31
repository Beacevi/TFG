/**
 * @file InteractableGameObject.cs
 * @brief Clase base para objetos interactuables del terreno procedural.
 * @author Hortensia Studio - Alejandro Romero Burgada
 * @date 2026
 * @details Archivo perteneciente al proyecto Cloud Wander. La documentación se ha preparado con comentarios XML compatibles con Doxygen para describir clases, campos y métodos relevantes.
 */

using UnityEngine;

/// <summary>
/// Clase base para objetos interactuables del terreno procedural.
/// </summary>
public class InteractableGameObject : MonoBehaviour
{
    /// <summary>
    /// Campo de tipo Bird utilizado para almacenar o configurar bird data.
    /// </summary>
    public Bird birdData;

    /// <summary>
    /// Prefab o conjunto de prefabs utilizados para instanciar simon minigame prefab.
    /// </summary>
    [SerializeField] private GameObject simonMinigamePrefab;

    /// <summary>
    /// Campo de tipo GameObject utilizado para almacenar o configurar silhouette.
    /// </summary>
    public GameObject silhouette;
    /// <summary>
    /// Referencia visual o sprite asociado a normal sprite.
    /// </summary>
    public GameObject normalSprite;

    /// <summary>
    /// Ejecuta la lógica asociada a interact dentro de InteractableGameObject.
    /// </summary>
    /// <param name="script">Parámetro script empleado por el método.</param>
    public void Interact(TileAStar script)
    {
        Debug.Log("You interacted with me!");
        if (birdData != null)
        {
            Debug.Log("EL pajaro es: " + birdData.birdName);
            ScenePersistentManager.instance.interactedBird = birdData;

            if (simonMinigamePrefab != null && script.stepsAvailable > 0)
            {
                Debug.Log("Tenia: "+script.stepsAvailable+" pasos");
                Instantiate(simonMinigamePrefab);
            }
            else
            {
                Debug.LogError("InteractableGameObject: simonMinigamePrefab no asignado en el Inspector o no se pudo llegar.");
            }
        }

        else
        {
            Debug.Log("Es otro interactuable o birdData esta vacio");
        }

        if (CompareTag("Coin"))
        {
            GameManager.Instance.AddMoney(1);
            script.RemoveCoinAtLastNode();
            //TODO: Añadir al final de la isla
        }
    }

    /// <summary>
    /// Procesa la entrada de otro collider 2D en el área de detección.
    /// </summary>
    /// <param name="col">Parámetro col empleado por el método.</param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Tree"))
        {
            silhouette.SetActive(true);
            normalSprite.SetActive(false);
        }
    }

    /// <summary>
    /// Procesa la salida de otro collider 2D del área de detección.
    /// </summary>
    /// <param name="col">Parámetro col empleado por el método.</param>
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Tree"))
        {
            silhouette.SetActive(false);
            normalSprite.SetActive(true);
        }
    }

}
