using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    [Header("Scene")]
    public string targetSceneName;

    [Header("Audio")]
    public AudioClip enterSound;
    [Range(0f, 1f)] public float volume = 1f;

    private bool triggered;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        // 🔊 Play sound once at trigger position
        if (enterSound)
            AudioSource.PlayClipAtPoint(enterSound, transform.position, volume);

        StartCoroutine(Transition());
    }

    IEnumerator Transition()
    {
        yield return ScreenFader.Instance.FadeToBlack();
        SceneManager.LoadScene(targetSceneName);
    }
}
