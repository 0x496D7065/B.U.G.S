using System.Collections;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    [Header("Projectile")]
    public ObjectPooler projectilePool;
    public Transform firePoint;

    [Header("Effects")]
    public GameObject muzzleFlashPrefab;
    public AudioClip shootSound;
    public ParticleSystem casingParticles;

    [Header("Stats")]
    public float fireRate = 0.2f;
    public float projectileSpeed = 100f;
    public bool  FullAuto = false;
    public int   magSize = 16;
    public int   ammoPoolSize = 32;
    public int   currentAmmoPool = 0;
    public int   currentMag = 0;
    // private float nextFireTime = 0f;
    // private bool fireBuffered = false;

    private AudioSource audioSource;
    private Animator animator;
    private AmmoHUD ammoHUD;
    private bool isReloading = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
        ammoHUD = GetComponentInChildren<AmmoHUD>();
        currentAmmoPool = ammoPoolSize;
    }

    public virtual void Fire()
    {
        if (isReloading) return;
        animator.Play("Fire", 0, 0f);
        projectilePool.GetFromPool(firePoint.position, firePoint.rotation);
        if (projectilePool == null)
            Debug.Log("Failed to queue projectile");
        if (muzzleFlashPrefab != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
            Destroy(flash, 0.1f);
        }
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
        currentMag -= 1;
        var emitParams = new ParticleSystem.EmitParams
        {
           // rotation3D = Random.rotation.eulerAngles,
        };
        casingParticles.Emit(1);
        ammoHUD.UpdateAmmoDisplay();
    }

    public virtual void Reload()
    {
        if (isReloading) return;
        isReloading = true;
        animator.Play("Reload", 0, 0f);
    }

    public void OnReloadFinished()
    {
        isReloading = false;
        int reloaded = magSize - currentMag;
        if (reloaded > currentAmmoPool)
            reloaded = currentAmmoPool;
        currentMag += reloaded;
        currentAmmoPool -= reloaded;
        ammoHUD.UpdateAmmoDisplay();
        Debug.Log("finished reloading");
    }
}
