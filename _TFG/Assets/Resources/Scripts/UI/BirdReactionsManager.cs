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

        int r = 0;

        switch (r)
        {
            case 0:
                Debug.Log("Se ha emitido un sonido");
                bird.PlaySound();
                break;
        }
    }
}