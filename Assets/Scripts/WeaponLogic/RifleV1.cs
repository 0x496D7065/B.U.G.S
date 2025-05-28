using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UI.Image;

public class RifleV1 : MonoBehaviour, IUsable, IAmmoUser, IRecoilData
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

    [Header("HipFire Recoil")]
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    [Header("Aim Recoil")]
    [SerializeField] private float aimRecoilX;
    [SerializeField] private float aimRecoilY;
    [SerializeField] private float aimRecoilZ;

    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;

    [Header("Effects")]
    public Transform firePoint;
    public AudioClip shootSound;
    public ParticleSystem casingParticles;
    public ParticleSystem muzzleFlash;
    //public TrailRenderer bulletTrail;
    public Animator animator;
    public WeaponKick weaponKick;
    public TrailPooler trailPool;
    [Header("HUD Logic")]
    public int currentMag => _currentMag;
    public int currentAmmoPool => _currentAmmoPool;
    //Recoil Script link
    private Recoil recoilScript;
    public float RecoilX => recoilX;
    public float RecoilY => recoilY;
    public float RecoilZ => recoilZ;

    public float AimRecoilX => aimRecoilX;
    public float AimRecoilY => aimRecoilY;
    public float AimRecoilZ => aimRecoilZ;

    public float Snappiness => snappiness;
    public float ReturnSpeed => returnSpeed;
    //References
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
        animator = GetComponent<Animator>();
        recoilScript = GetComponentInParent<Recoil>();
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
    public void Action3(InputAction.CallbackContext context)
    {
        if (context.performed)
            animator.SetBool("isAiming", true);
        else if (context.canceled)
            animator.SetBool("isAiming", false);
    }
    void Update()
    {
    }

    void Shoot()
    {
        if (isReloading || _currentMag <= 0)
            return;
        Vector3 endPoint;
        IDamageable damageable = null;
        muzzleFlash.Emit(1);
        audioSource.PlayOneShot(shootSound);
        recoilScript.RecoilFire();
        weaponKick.PlayKick();
        if (Physics.Raycast(firePoint.transform.position, firePoint.transform.forward, out RaycastHit hit, range, targetMask))
        {
            Debug.Log($"{hit.transform.name}");
            endPoint = hit.point;
            damageable = hit.transform.GetComponentInParent<IDamageable>();
        }
        else
        {
            endPoint = firePoint.transform.position + firePoint.transform.forward * range;
        }
        TrailRenderer trail = trailPool.GetFromPool(firePoint.transform.position, Quaternion.identity);
        StartCoroutine(SpawnTrail(trail, endPoint, damageable, hit));
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
    private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 endPosition, IDamageable damageTarget, RaycastHit hit)
    {
        float time = 0f;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, endPosition, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }
        trail.transform.position = endPosition;
        damageTarget?.TakeDamage(damage, hit);
        trailPool.ReturnTrail(trail);
    }
}
