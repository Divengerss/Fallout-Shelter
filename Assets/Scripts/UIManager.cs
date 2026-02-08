using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Liens UI (A glisser depuis la scène)")]
    public Text PowerText;
    public Text FoodText;
    public Text WaterText;
    public Text CapsText;
    public Text TomatoText;


    public Button AutoAssignButton;
    public Button RecruitButton;
    public Button ExplorationButton;

    private void Start()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourcesChanged += UpdateUI;
            UpdateUI();
        }
        else
        {
            Debug.LogError("Attention : Pas de ResourceManager trouvé dans la scène !");
        }

        if (AutoAssignButton != null)
        {
            AutoAssignButton.onClick.AddListener(() => 
            {
                if (DwellerManager.Instance != null)
                {
                    DwellerManager.Instance.AutoAssignAllDwellers();
                }
            });
        }

        if (RecruitButton != null)
        {
            RecruitButton.onClick.AddListener(() => 
            {
                if (DwellerManager.Instance != null)
                {
                    DwellerManager.Instance.BuyDweller();
                }
            });
        }

        if (ExplorationButton != null)
        {
            ExplorationButton.onClick.AddListener(() => 
            {
                SceneManager.LoadScene("Exploration");
            });
        }
    }

    private void OnDestroy()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourcesChanged -= UpdateUI;
        }
    }

    public void UpdateUI()
    {
        if (ResourceManager.Instance == null) return;

        if (PowerText != null) PowerText.text = "Power: " + ResourceManager.Instance.Power + " / " + ResourceManager.Instance.MaxPower;
        if (FoodText != null) FoodText.text = "Food: " + ResourceManager.Instance.Food + " / " + ResourceManager.Instance.MaxFood;
        if (WaterText != null) WaterText.text = "Water: " + ResourceManager.Instance.Water + " / " + ResourceManager.Instance.MaxWater;
        if (CapsText != null) CapsText.text = "Caps: " + ResourceManager.Instance.Caps;
        if (TomatoText != null) TomatoText.text = "Tomatoes: " + ResourceManager.Instance.Tomatoes + " / " + ResourceManager.Instance.MaxTomatoes;
    }
}
