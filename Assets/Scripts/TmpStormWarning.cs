using System.Collections;
using UnityEngine;
using TMPro;

public class TMPStormWarning : MonoBehaviour
{
    [Header("Storm Timer")]
    public float stormDuration = 300f; // 5 minutes total

    [Header("Warning Times (minutes remaining)")]
    public int[] warningMinutes = { 5, 3, 1 };

    [Header("Fade Timings")]
    public float fadeInDuration = 1f;
    public float visibleDuration = 5f;
    public float fadeOutDuration = 1f;

    [Header("References")]
    public TextMeshProUGUI text;
    public AudioSource audioSource;

    void Awake()
    {
        if (!text)
            text = GetComponent<TextMeshProUGUI>();

        SetAlpha(0f);
    }

    void Start()
    {
        StartCoroutine(StormWarningSequence());
    }

    IEnumerator StormWarningSequence()
    {
        foreach (int minutes in warningMinutes)
        {
            float triggerTime = stormDuration - (minutes * 60f);

            if (triggerTime < 0f)
                continue;

            yield return new WaitForSeconds(triggerTime - GetElapsedTime());

            ShowWarning(minutes);
            yield return PlayTextSequence();
        }
    }

    float startTime;
    float GetElapsedTime()
    {
        if (startTime == 0f)
            startTime = Time.time;

        return Time.time - startTime;
    }

    void ShowWarning(int minutes)
    {
        text.text = $"{minutes} minute{(minutes > 1 ? "s" : "")} avant la tempête";

        if (audioSource)
            audioSource.Play();
    }

    IEnumerator PlayTextSequence()
    {
        yield return FadeText(0f, 1f, fadeInDuration);
        yield return new WaitForSeconds(visibleDuration);
        yield return FadeText(1f, 0f, fadeOutDuration);
    }

    IEnumerator FadeText(float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(to);
    }

    void SetAlpha(float alpha)
    {
        Color c = text.color;
        c.a = alpha;
        text.color = c;
    }
}
