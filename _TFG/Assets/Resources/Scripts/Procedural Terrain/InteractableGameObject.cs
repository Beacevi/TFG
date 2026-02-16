using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableGameObject : MonoBehaviour
{
    public void Interact(TileAStar script)
    {
        Debug.Log("You interacted with me!");

        //LOGICA PARA LA INTERACCION CON LOS OBJETOS
        GameObject.FindGameObjectWithTag("SceneChanger").GetComponent<ChangeScene>().LoadSceneAdditive("SimonSays");
        //SceneManager.LoadSceneAdditive("SimonSays", LoadSceneMode.Additive);

        StopInteraction(script);
    }

    private void StopInteraction(TileAStar script)
    {
        script.moving = false;

    }
}
