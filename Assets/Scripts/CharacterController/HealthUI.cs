using UnityEngine;
using TMPro;
public class HealthUI : MonoBehaviour
{
    public TextMeshProUGUI HealthText;
    public GameObject TargetObject;

    private void Awake()
    {
        HealthText = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void SetTargetObject(GameObject obj)
    {
        TargetObject = obj;
    }
    public void UpdateHealthDisplay(int health, string customLabel)
    {
        //if (player != null && HealthText != null)
        //{
        //    HealthText.text = $"Health: {player.health}";
        //}
        HealthText.text = $"{customLabel}: {health}";
    }
}
