using UnityEngine;

public class Recoil : MonoBehaviour
{
    //Rotations
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    //Bool
    private bool isAiming;

    //References
    private WepHolderV1 weaponHolder;
    private Animator weaponAnimator;
    private GameObject lastWeaponObject;
    private IRecoilData recoilSource;
    private float snappiness;
    private float returnSpeed;


    void Start()
    {
        weaponHolder = GetComponentInChildren<WepHolderV1>(true);
    }

    void Update()
    {
        UpdateWeaponReferences();
        if (recoilSource == null)
            return;
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire()
    {
        if (recoilSource == null)
            return;
        isAiming = weaponAnimator != null && weaponAnimator.GetBool("isAiming");
        if (isAiming)
        {
            targetRotation += new Vector3(recoilSource.AimRecoilX, Random.Range(-recoilSource.AimRecoilY, recoilSource.AimRecoilY), Random.Range(-recoilSource.AimRecoilZ, recoilSource.AimRecoilZ));
        }
        else
        {
            targetRotation += new Vector3(recoilSource.RecoilX, Random.Range(-recoilSource.RecoilY, recoilSource.RecoilY), Random.Range(-recoilSource.RecoilZ, recoilSource.RecoilZ));
        }
    }
    private void UpdateWeaponReferences()
    {
        if (weaponHolder == null || weaponHolder.currentWeaponObject == null)
        {
            weaponAnimator = null;
            recoilSource = null;
            lastWeaponObject = null;
            return;
        }

        var currentWeapon = weaponHolder.currentWeaponObject;

        if (currentWeapon != lastWeaponObject)
        {
            weaponAnimator = currentWeapon.GetComponent<Animator>();
            recoilSource  = currentWeapon.GetComponent<IRecoilData>();
            if (recoilSource != null)
            {
                snappiness = recoilSource.Snappiness;
                returnSpeed = recoilSource.ReturnSpeed;
            }
            lastWeaponObject = currentWeapon;
        }
    }
}
