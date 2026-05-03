using System.Collections;
using UnityEngine;

public class CoinClick : MonoBehaviour
{
    public AudioSource src;
    public AudioClip sound;
    public CircleCollider2D colliderCoin;
    public SpriteRenderer spriteCoin;
    public ParticleSystem shinyParticle;
    
    private void OnMouseDown()
    {
        GameManager.Instance.AddMoney(1);
        src.clip = sound;
        src.Play();
        StartCoroutine(DestroyObject());
        //Particulas
        
    }

    private IEnumerator DestroyObject()
    {
        shinyParticle.Stop();
        colliderCoin.enabled = false;
        spriteCoin.enabled = false;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

}
