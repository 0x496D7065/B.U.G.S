using UnityEngine;

public class WeaponLoadoutUI : MonoBehaviour
{
    public WeaponHolder weaponHolder;
    public GameObject pistolPrefab;

    public void EquipPistol()
    {
        weaponHolder.EquipWeapon(pistolPrefab);
        Debug.Log("EquipPistol() called!");
    }
}
