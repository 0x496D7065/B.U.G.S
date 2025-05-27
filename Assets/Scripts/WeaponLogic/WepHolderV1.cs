using UnityEngine;
using UnityEngine.InputSystem;

public class WepHolderV1 : MonoBehaviour
{
    private IUsable currentWeapon;
    public GameObject currentWeaponObject;
    public Transform weaponMountPoint;

    public void EquipWeapon(GameObject weaponObject)
    {
        if (currentWeaponObject != null)
            Destroy(currentWeaponObject);
        GameObject newWeapon = Instantiate(weaponObject, weaponMountPoint);
        currentWeapon = newWeapon.GetComponent<IUsable>();
        currentWeaponObject = newWeapon;
    }

    public void OnAction1Input(InputAction.CallbackContext context)
    {
        currentWeapon?.Action1(context);
    }
    public void OnAction2Input()
    {
        currentWeapon?.Action2();
    }
    public void OnAction3Input(InputAction.CallbackContext context)
    {
        currentWeapon?.Action3(context);
    }
}
