using UnityEngine;
using TMPro;

public class AmmoHUD : MonoBehaviour
{
    public WeaponBase weapon;
    public TextMeshProUGUI ammoText;

    private void Awake()
    {
        weapon = GetComponentInParent<WeaponBase>();
        ammoText = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void UpdateAmmoDisplay()
    {
        if (weapon != null && ammoText != null)
        {
            ammoText.text = $"{weapon.currentMag} / {weapon.currentAmmoPool}";
        }
    }
}

