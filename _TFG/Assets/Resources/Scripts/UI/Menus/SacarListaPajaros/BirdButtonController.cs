using UnityEngine;
using UnityEngine.UI;

public class BirdButtonController : MonoBehaviour
{
    [Header("Birds y botones")]
    public Bird[] birds;      // Todos tus ScriptableObjects Bird
    public Button[] botones;  // Botones de la UI, uno por Bird

    // Este método se llama al pulsar el botón de comprobación
    public void ActualizarBotones()
    {
        for (int i = 0; i < botones.Length; i++)
        {
            if (i < birds.Length)
            {
                // Activar o desactivar el botón según conseguido
                botones[i].gameObject.SetActive(birds[i].obtenido);
            }
            else
            {
                // Si hay más botones que birds, desactivamos extras
                botones[i].gameObject.SetActive(false);
            }
        }
    }
}
