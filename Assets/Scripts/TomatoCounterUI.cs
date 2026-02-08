using TMPro;
using UnityEngine;

public class TomatoCounterUI : MonoBehaviour
{
    public static TomatoCounterUI Instance;

    public TextMeshProUGUI counterText;

    private int tomatoCount = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        UpdateText();
    }

    public void AddTomato(int amount = 1)
    {
        tomatoCount += amount;
        UpdateText();
    }

    void UpdateText()
    {
        counterText.text = tomatoCount.ToString();
    }
}
