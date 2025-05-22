using UnityEngine;
using TMPro;
public class HealthUI : MonoBehaviour
{
    public PlayerController player;
    public TextMeshProUGUI HealthText;

    private void Awake()
    {
        HealthText = GetComponentInChildren<TextMeshProUGUI>();
        UpdateHealthDisplay();
    }
    public void UpdateHealthDisplay()
    {
        if (player != null && HealthText != null)
        {
            HealthText.text = $"Health: {player.health}";
        }
    }
}
