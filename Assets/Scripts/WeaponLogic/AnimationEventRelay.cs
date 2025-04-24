using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    public WeaponBase weaponBase;

    void Awake()
    {
        weaponBase = GetComponentInParent<WeaponBase>();
    }

    // Called by the animation event
    public void OnReloadFinished()
    {
        if (weaponBase != null)
        {
            weaponBase.OnReloadFinished();
        }
        else
        {
            Debug.LogWarning("WeaponBase reference not set on AnimationEventRelay.");
        }
    }
}
