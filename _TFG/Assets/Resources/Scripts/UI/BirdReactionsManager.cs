using UnityEngine;

public class BirdReactionsManager : MonoBehaviour
{
    public void TriggerReaction(BirdsReactions bird)
    {
        if (bird == null)
        {
            Debug.LogWarning("Bird es null en TriggerReaction");
            return;
        }

        int r = Random.Range(0, 3);

        switch (r)
        {
            case 0:
                Debug.Log("Se ha movido");
                bird.PlayMovement();
                break;

            case 1:
                Debug.Log("Se ha emitido un sonido");
                bird.PlaySound();
                break;

            case 2:
                Debug.Log("Se ha movido y emitido un sonido");
                bird.PlaySound();
                bird.PlayMovement();
                break;
        }
    }
}