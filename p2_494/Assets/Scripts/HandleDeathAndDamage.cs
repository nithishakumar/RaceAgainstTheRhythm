using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleDeathAndDamage : MonoBehaviour
{
    Subscription<DeathEvent> deathEventSub;
    Subscription<DamageEvent> damageEventSub;
    AudioClip deathSfx;

    void Start()
    {
        deathSfx = ResourceLoader.GetAudioClip("deathSfx");
        deathEventSub = EventBus.Subscribe<DeathEvent>(_OnDeath);
        damageEventSub = EventBus.Subscribe<DamageEvent>(_OnDamage);
    }

    void _OnDeath(DeathEvent e)
    {
        GameObject beatManager = GameObject.Find("BeatManager");
        if (beatManager != null)
        {   // Stop audio source
            Destroy(beatManager);
            AudioSource.PlayClipAtPoint(deathSfx, Camera.main.transform.position);
            // Stop player movement animaton
            GetComponent<Animate>().StopAnimation();
            // Disable player movement
            GetComponent<CharacterMovement>().enabled = false;
            StartCoroutine(ShrinkRotate());
        }
    }

    void _OnDamage(DamageEvent e)
    {
        StartCoroutine(DamageRoutine());
    }

    IEnumerator DamageRoutine()
    {
        GetComponent<Animate>().StopAnimation();

        // Flash Sprite
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.blue;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;

        GetComponent<Animate>().RestartAnimation();
    }

    IEnumerator ShrinkRotate()
    {
        Vector3 shrinkAmount = new Vector3(0.05f, 0.05f, 0.05f);
        Vector3 minimumScale = new Vector3(0f, 0f, 0f);

        for (int i = 0; i < 6; i++)
        {
            // Rotate
            transform.Rotate(0, 0, 30);

            // Shrink
            Vector3 newScale = transform.localScale - shrinkAmount;
            newScale = Vector3.Max(newScale, minimumScale);
            transform.localScale = newScale;

            yield return new WaitForSeconds(0.1f);
        }
        Destroy(gameObject);
        yield return new WaitForSeconds(1f);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(damageEventSub);
        EventBus.Unsubscribe(deathEventSub);
    }
}

public class DeathEvent
{

} 

public class DamageEvent
{

}
