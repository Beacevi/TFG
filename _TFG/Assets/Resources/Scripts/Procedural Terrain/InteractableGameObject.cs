using UnityEngine;

public class InteractableGameObject : MonoBehaviour
{
    public Bird birdData;

    [SerializeField] private GameObject simonMinigamePrefab;

    public void Interact(TileAStar script)
    {
        Debug.Log("You interacted with me!");
        Debug.Log("EL pajaro es: " + birdData.birdName);
        ScenePersistentManager.instance.interactedBird = birdData;

        if (simonMinigamePrefab != null)
        {
            Instantiate(simonMinigamePrefab);
        }
        else
        {
            Debug.LogError("InteractableGameObject: simonMinigamePrefab no asignado en el Inspector.");
        }
    }
}
