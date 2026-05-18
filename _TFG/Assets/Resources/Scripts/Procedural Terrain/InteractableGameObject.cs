using UnityEngine;

public class InteractableGameObject : MonoBehaviour
{
    public Bird birdData;

    [SerializeField] private GameObject simonMinigamePrefab;

    public GameObject silhouette;
    public GameObject normalSprite;

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
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Tree"))
        {
            silhouette.SetActive(true);
            normalSprite.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Tree"))
        {
            silhouette.SetActive(false);
            normalSprite.SetActive(true);
        }
    }

}
