using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableGameObject : MonoBehaviour
{
    public Bird birdData;
    public void Interact(TileAStar script)
    {
        Debug.Log("You interacted with me!");
        Debug.Log("EL pajaro es: "+ birdData.birdName);
        ScenePersistentManager.instance.interactedBird = birdData;

        //LOGICA PARA LA INTERACCION CON LOS OBJETOS
        //GameObject.FindGameObjectWithTag("SceneChanger").GetComponent<ChangeScene>().LoadSceneAdditive("SimonSays");
        GameObject.FindGameObjectWithTag("SceneChanger").GetComponent<ChangeScene>().LoadSceneAdditive("SimonSaysPajaro");

        //SceneManager.LoadSceneAdditive("SimonSays", LoadSceneMode.Additive);

        StopInteraction(script);
    }

    private void StopInteraction(TileAStar script)
    {
        script.moving = false;

    }
}
