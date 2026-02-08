using UnityEngine;

public class TomatoCollectible : MonoBehaviour
{
    public int value = 1;

    [Header("Audio")]
    public AudioClip collectSound;
    [Range(0f, 1f)] public float volume = 1f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Collect();
    }

    void Collect()
    {
        if (TomatoCounterUI.Instance)
            TomatoCounterUI.Instance.AddTomato(value);

        if (collectSound)
            AudioSource.PlayClipAtPoint(collectSound, transform.position, volume);

        Destroy(gameObject);
    }
}
