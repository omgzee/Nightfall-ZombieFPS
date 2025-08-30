using UnityEngine;
using System.Collections;
using TMPro;
using Cinemachine;

public class Weapon : MonoBehaviour
{
    public enum GunType
    {
        Pistol,
        Carbine,
        Shotgun
    }

    public GunType gunType;

    [Header("Shooting Settings")]
    [SerializeField] float range = 10f;
    [SerializeField] float damage = 30f;
    [SerializeField] float fireRate = 15f;
    [SerializeField] float magazineSize = 30f;
    [SerializeField] float reloadTime = 2f;
    [SerializeField] AmmoType ammoType;

    private float nextTimeToFire = 0f;
    [SerializeField] private float currentAmmo;
    private bool isReloading = false;
    private Ammo ammo;

    [Header("Recoil Settings")]
    public float recoilX = -2f;
    public float recoilY = 1f;
    public float recoilZ = 0.5f;
    public float recoilSmooth = 10f;
    public float recoilResetSpeed = 20f;
    public CameraRecoil cameraRecoil;

    [Header("Recoil Kickback Settings")]
    [SerializeField] Transform weaponModel;
    [SerializeField] float kickbackAmount = 0.05f;
    [SerializeField] float kickbackReturnSpeed = 5f;
    private Vector3 originalWeaponPosition;

    [Header("Bobbing Settings")]
    [SerializeField] private float bobFrequency = 6f;
    [SerializeField] private float bobAmount = 0.03f;
    private float bobTimer = 0f;
    private Vector3 bobOffset = Vector3.zero;

    [Header("Camera Settings")]
    [SerializeField] Camera FPCamera;
    [SerializeField] Camera weaponCamera;
    [SerializeField] private CinemachineImpulseSource impulseSource;

    [Header("Crosshair Settings")]
    [SerializeField] private CrosshairScaler crosshairScaler;

    [Header("Hit Marker Settings")]
    [SerializeField] GameObject hitmarkerObject;
    [SerializeField] float hitmarkerPopScale = 1.5f;
    [SerializeField] float hitmarkerShrinkSpeed = 5f;
    [SerializeField] float hitmarkerDuration = 0.1f;
    private float hitmarkerTimer;
    private Vector3 originalHitmarkerScale;

    [Header("Sound Effects")]
    public AudioSource shootingSound;
    public AudioSource reloadingSound;
    public AudioSource hitmarkerSound;
    public AudioSource NoAmmoCockSound;

    [Header("Effects")]
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] ParticleSystem cartdridgeEjectEffect;
    [SerializeField] GameObject hitEffect;
    [SerializeField] GameObject EnemyHitMarker;
    [SerializeField] GameObject bloodSplatterEffect;

    public Light flashlight;
    [SerializeField] GameObject shellPrefab;
    [SerializeField] private Transform shellEjectPoint;

    [SerializeField] private TextMeshProUGUI ammoText;
    private bool isAnimatingAmmo = false;
    private int lastAmmo = -1;

    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (ammo == null)
        {
            ammo = GetComponentInParent<Ammo>();
            if (ammo == null)
            {
                Debug.LogError("Weapon.cs: Ammo script NOT found on parent!");
            }
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Weapon.cs: Animator is missing!");
            }
        }

        if (weaponModel != null)
            originalWeaponPosition = weaponModel.localPosition;

        if (hitmarkerObject != null)
            originalHitmarkerScale = hitmarkerObject.transform.localScale;
    }

    private void Start()
    {
        currentAmmo = magazineSize;
    }

    private void OnEnable()
    {
        isReloading = false;
        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
            animator.SetBool("Reloading", false);
        }
    }

    void Update()
    {
        if (isReloading) return;

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < magazineSize && ammo.GetCurrentAmmo(ammoType) > 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (currentAmmo <= 0 && ammo.GetCurrentAmmo(ammoType) > 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo == 0)
        {
            Debug.Log("No ammo to reload!");
            return;
        }

        if (crosshairScaler != null)
        {
            crosshairScaler.isFiring = Input.GetButton("Fire1") && !isReloading && currentAmmo > 0;
            crosshairScaler.isAiming = Input.GetButton("Fire2");
            crosshairScaler.isMoving = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).magnitude > 0.1f;
        }

        Shoot();
        Flashlight();
        HandleHitmarker();
        UpdateAmmoUI();
        HandleWeaponBobbing();
    }

    void Shoot()
    {
        bool canShoot = gunType == GunType.Carbine ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");

        if (canShoot && Time.time >= nextTimeToFire)
        {
            if (currentAmmo > 0)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                currentAmmo--;
                ProcessRayCast();
                ApplyKickback();
                //cameraRecoil.ApplyRecoil(recoilX, recoilY, recoilZ);
                if (impulseSource != null)
                {
                    impulseSource.GenerateImpulse();
                }
            }
            else
            {
                if (Input.GetButtonDown("Fire1") && !NoAmmoCockSound.isPlaying)
                {
                    Debug.Log("Out of ammo - playing cock sound");
                    NoAmmoCockSound.Play();
                }
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        reloadingSound.Play();

        if (animator != null)
        {
            animator.SetBool("Reloading", true);
        }

        yield return new WaitForSeconds(reloadTime - 0.25f);

        if (animator != null)
        {
            animator.SetBool("Reloading", false);
        }

        int ammoAvailable = ammo.GetCurrentAmmo(ammoType);
        float ammoNeeded = magazineSize - currentAmmo;
        float ammoToReload = Mathf.Min(ammoNeeded, ammoAvailable);

        currentAmmo += ammoToReload;
        for (int i = 0; i < ammoToReload; i++)
        {
            ammo.ReduceCurrentAmmo(ammoType);
        }

        isReloading = false;
    }

    private void Flashlight()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            flashlight.enabled = !flashlight.enabled;
        }
    }

    private void ProcessRayCast()
    {
        if (animator != null)
        {
            animator.SetTrigger("Shooting");
        }

        muzzleFlash.Emit(1);
        shootingSound.Play();
        ShellSpawner();

        if (gunType == GunType.Shotgun)
        {
            int pelletCount = 6;
            float spreadAngle = 5f;

            for (int i = 0; i < pelletCount; i++)
            {
                Vector3 spreadDirection = FPCamera.transform.forward;
                spreadDirection += Random.insideUnitSphere * Mathf.Tan(spreadAngle * Mathf.Deg2Rad);
                spreadDirection.Normalize();

                if (Physics.Raycast(FPCamera.transform.position, spreadDirection, out RaycastHit hit, range))
                {
                    HandleHit(hit);
                }
            }
        }
        else
        {
            if (Physics.Raycast(FPCamera.transform.position, FPCamera.transform.forward, out RaycastHit hit, range))
            {
                HandleHit(hit);
            }
        }
    }

    private void HandleHit(RaycastHit hit)
    {
        EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
        if (target != null)
        {
            target.TakeDamage(damage);
            CreateBloodEffect(hit);
            ShowHitmarker();
        }
        else
        {
            CreateHitImpact(hit);
        }
    }

    private void CreateHitImpact(RaycastHit hit)
    {
        if (hitEffect != null)
        {
            GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, 2f);
        }
    }

    private void CreateBloodEffect(RaycastHit hit)
    {
        if (EnemyHitMarker != null)
        {
            GameObject marker = Instantiate(EnemyHitMarker, hit.point, Quaternion.LookRotation(hit.normal));
            foreach (var p in marker.GetComponentsInChildren<ParticleSystem>())
            {
                p.Play();
            }
            Destroy(marker, 0.8f);
        }

        if (bloodSplatterEffect != null)
        {
            GameObject splatter = Instantiate(bloodSplatterEffect, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(-hit.normal));
            splatter.transform.SetParent(hit.transform);
            Destroy(splatter, 10f);
        }
    }

    private void ShowHitmarker()
    {
        if (hitmarkerObject == null) return;

        hitmarkerObject.SetActive(true);
        hitmarkerObject.transform.localScale = originalHitmarkerScale * Random.Range(1f, hitmarkerPopScale);
        hitmarkerSound.Play();
        hitmarkerTimer = hitmarkerDuration;
    }

    private void HandleHitmarker()
    {
        if (hitmarkerObject == null || !hitmarkerObject.activeSelf) return;

        hitmarkerTimer -= Time.deltaTime;

        float randomShrinkSpeed = Random.Range(hitmarkerShrinkSpeed - 0.5f, hitmarkerShrinkSpeed + 0.5f);
        hitmarkerObject.transform.localScale = Vector3.Lerp(
            hitmarkerObject.transform.localScale,
            originalHitmarkerScale,
            randomShrinkSpeed * Time.deltaTime
        );

        if (hitmarkerTimer <= 0)
        {
            hitmarkerObject.SetActive(false);
        }
    }

    private void ApplyKickback()
    {
        if (weaponModel != null)
        {
            weaponModel.localPosition -= new Vector3(0f, 0f, kickbackAmount);
            FPCamera.transform.localPosition -= new Vector3(0f, 0f, recoilZ);
        }
    }

    private void ShellSpawner()
    {
        if (shellPrefab != null && (gunType == GunType.Pistol || gunType == GunType.Carbine))
        {
            if (shellEjectPoint != null)
            {
                GameObject shell = Instantiate(shellPrefab, shellEjectPoint.position, shellEjectPoint.rotation);
                Rigidbody rb = shell.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(shellEjectPoint.right * Random.Range(1.5f, 2.5f), ForceMode.Impulse);
                    rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
                }

                Destroy(shell, 3f);
            }
        }
        else if (gunType == GunType.Shotgun && cartdridgeEjectEffect != null)
        {
            cartdridgeEjectEffect.Emit(1);
        }
    }

    public void StopReloading()
    {
        isReloading = false;
        if (animator != null)
        {
            animator.SetBool("Reloading", false);
        }
    }

    public bool IsReloading()
    {
        return isReloading;
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null && ammo != null)
        {
            int totalAmmo = ammo.GetCurrentAmmo(ammoType);
            ammoText.text = $"<color=#00FFF7>{currentAmmo}</color> / <color=#0055FF>{totalAmmo}</color>";

            if ((int)currentAmmo != lastAmmo)
            {
                StartCoroutine(AnimateAmmoPop());
                lastAmmo = (int)currentAmmo;
            }
        }
    }

    IEnumerator AnimateAmmoPop()
    {
        if (isAnimatingAmmo) yield break;
        isAnimatingAmmo = true;

        Vector3 originalScale = ammoText.transform.localScale;
        Vector3 targetScale = originalScale * 1.1f;
        float duration = 0.2f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            float scale = Mathf.SmoothStep(0f, 1f, Mathf.Sin(t * Mathf.PI / 2));
            ammoText.transform.localScale = Vector3.Lerp(originalScale, targetScale, scale);
            yield return null;
        }

        ammoText.transform.localScale = originalScale;
        isAnimatingAmmo = false;
    }

    public float GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public float GetMaxAmmo()
    {
        return magazineSize;
    }

    private void HandleWeaponBobbing()
    {
        if (weaponModel == null) return;

        float moveAmount = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).magnitude;

        if (moveAmount > 0.1f && !isReloading)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            bobOffset = new Vector3(0f, Mathf.Sin(bobTimer) * bobAmount, 0f);
        }
        else
        {
            bobTimer = 0f;
            bobOffset = Vector3.Lerp(bobOffset, Vector3.zero, Time.deltaTime * 5f);
        }

        weaponModel.localPosition = Vector3.Lerp(
            weaponModel.localPosition,
            originalWeaponPosition + bobOffset,
            Time.deltaTime * kickbackReturnSpeed
        );
    }
}
