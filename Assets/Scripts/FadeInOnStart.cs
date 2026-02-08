using UnityEngine;

public class FadeInOnStart : MonoBehaviour
{
    void Start()
    {
        if (ScreenFader.Instance)
            StartCoroutine(ScreenFader.Instance.FadeFromBlack());
    }
}
