using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UI.Image;

public class RifleV1 : MonoBehaviour, IUsable, IAmmoUser
{
    [Header("Stats")]
    public int damage;
    public float range;
    public float fireRate;
    public int magSize;
    public int ammoPoolSize;
    public int _currentAmmoPool;
    public int _currentMag;
    public bool FullAuto = false;

    [Header("Effects")]
    public Transform firePoint;
    public AudioClip shootSound;
    public ParticleSystem casingParticles;
    public ParticleSystem muzzleFlash;
    [Header("Display Logic")]
    public int currentMag => _currentMag;
    public int currentAmmoPool => _currentAmmoPool;

    private LayerMask targetMask;
    private AudioSource audioSource;
    private AmmoHUD ammoHUD;
    private bool isReloading = false;
    private float nextFireTime = 0f;
    public void Start()
    {
        targetMask = LayerMask.GetMask("Ground", "Enemy", "Construction", "Base", "Obstacles");
        audioSource = GetComponent<AudioSource>();
        ammoHUD = GetComponentInChildren<AmmoHUD>();
        _currentAmmoPool = ammoPoolSize;
    }

    public void Action1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (FullAuto)
                StartFullAuto();
            else
                Shoot(); // Semi-auto single tap
        }
        else if (context.canceled && FullAuto)
        {
            StopFullAuto();
        }
    }
    void StartFullAuto()
    {
        if (!IsInvoking(nameof(Shoot)))
            InvokeRepeating(nameof(Shoot), 0f, fireRate);
    }

    void StopFullAuto()
    {
        CancelInvoke(nameof(Shoot));
    }
    public void Action2()
    {
        Reload();
    }
    void Update()
    {
    }

    void Shoot()
    {
        if (isReloading || _currentMag <= 0)
            return;
        muzzleFlash.Play();
        //Add trail
        audioSource.PlayOneShot(shootSound);
        if (Physics.Raycast(firePoint.transform.position, firePoint.transform.forward, out RaycastHit hit, range, targetMask))
        {
            Debug.Log(hit.transform.name);
            IDamageable damageable = hit.transform.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                //Debug.Log("target hit, sending dmg");
                damageable.TakeDamage(damage);
            }
        }
        _currentMag -= 1;
        casingParticles.Emit(1);
        nextFireTime = Time.deltaTime + fireRate;
        ammoHUD.UpdateAmmoDisplay();
    }
    public virtual void Reload()
    {
        if (isReloading) return;
        isReloading = true;
        //animator.Play("Reload", 0, 0f);
        OnReloadFinished();
    }

    public void OnReloadFinished()
    {
        isReloading = false;
        int reloaded = magSize - _currentMag;
        if (reloaded > _currentAmmoPool)
            reloaded = _currentAmmoPool;
        _currentMag += reloaded;
        _currentAmmoPool -= reloaded;
        ammoHUD.UpdateAmmoDisplay();
        Debug.Log("finished reloading");
    }
}
