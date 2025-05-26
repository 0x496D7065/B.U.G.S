using UnityEngine;

public class WeaponLoadoutUI : MonoBehaviour
{
    [SerializeField] WepHolderV1 holder;
    [SerializeField] GameObject riflePrefab;
    public void EquipRifle()
    {
        holder.EquipWeapon(riflePrefab);
        Debug.Log("EquipWeapon() called!");
    }
}
