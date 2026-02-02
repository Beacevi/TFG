using System;
using UnityEngine;

public class InteractableGameObject : MonoBehaviour
{
    public void Interact(TileAStar script)
    {
        Debug.Log("You interacted with me!");

        //LOGICA PARA LA INTERACCION CON LOS OBJETOS
        GameObject.FindGameObjectWithTag("SceneChanger").GetComponent<ChangeScene>().Cambiar_A_Escena("SimonSays");

        StopInteraction(script);
    }

    private void StopInteraction(TileAStar script)
    {
        script.moving = false;

    }
}
