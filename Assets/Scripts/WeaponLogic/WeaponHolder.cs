using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public Transform weaponMount;
    private WeaponBase currentWeapon;
    private float nextFireTime = 0f;
    bool wantsToFire = false;

    private void Update()
    {
        if (currentWeapon == null)
            return;
        if (currentWeapon.FullAuto == false)
            wantsToFire = Input.GetMouseButtonDown(0);
        else
            wantsToFire = Input.GetMouseButton(0);
        //wantsToFire = Input.GetMouseButtonDown(0) || Input.GetMouseButton(0);
        nextFireTime -= Time.deltaTime;
        if (wantsToFire && Time.deltaTime >= nextFireTime && currentWeapon.currentMag > 0)
        {
            currentWeapon.Fire();
            nextFireTime = Time.deltaTime + currentWeapon.fireRate;
        }
        if (Input.GetKeyDown(KeyCode.R) && currentWeapon.currentAmmoPool > 0)
        {
            currentWeapon.Reload();
        }
    }

    public void EquipWeapon(GameObject weaponPrefab)
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);   
        }

        GameObject weaponobj = Instantiate(weaponPrefab, weaponMount);
        currentWeapon = weaponobj.GetComponent<WeaponBase>();
    }
}
