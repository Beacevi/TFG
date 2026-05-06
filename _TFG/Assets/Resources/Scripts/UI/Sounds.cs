using UnityEngine;

public class Sounds : MonoBehaviour
{
    public AudioSource src;
    public AudioClip sonidoInterfazBoton;
    public AudioClip sonidoRecolectarPajaro;
    public AudioClip sonidoAlinearPajaros;

    public AudioClip sonidoTocarPajaro;
    public void SonidoInterfazBoton()
    {
        src.clip = sonidoInterfazBoton;
        src.Play();
    }
    public void SonidoRecolectarPajaro()
    {
        src.clip = sonidoRecolectarPajaro;
        src.Play();
    }

    public void SonidoAlinearPajaros(AudioSource source)
    {
        source.clip = sonidoAlinearPajaros;
        source.Play();
    }

    public void SonidoTocarPajaro(AudioSource source)
    {
        source.clip = sonidoTocarPajaro;
        source.Play();
    }



}
