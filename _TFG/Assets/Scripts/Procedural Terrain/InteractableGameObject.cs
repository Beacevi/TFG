using System;
using UnityEngine;

public class InteractableGameObject : MonoBehaviour
{
    public void Interact(TileAStar script)
    {
        Debug.Log("You interacted with me!");

        //LOGICA PARA LA INTERACCION CON LOS OBJETOS

        StopInteraction(script);
    }

    private void StopInteraction(TileAStar script)
    {
        script.moving = false;

    }
}
