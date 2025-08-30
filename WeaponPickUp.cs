using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    [Header("Weapon Pickup Settings")]
    public int weaponIndexToActivate;
    public string weaponName;
    public float pickUpRange = 3f;
    public AudioClip pickupSound;

    public ObjectiveManager objectiveManager;

    private Transform player;
    private WeaponSwitcher weaponSwitcher;
    private bool isPlayerInRange = false;
    private bool isPickedUp = false;

    private void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        weaponSwitcher = FindFirstObjectByType<WeaponSwitcher>();
    }

    private void Update()
    {
        if (player == null || weaponSwitcher == null) return;

        float distance = Vector3.Distance(player.position, transform.position);
        bool isCurrentlyInRange = distance <= pickUpRange;

        if (WeaponNameUI.Instance != null && isCurrentlyInRange && !isPlayerInRange)
        {
            WeaponNameUI.Instance.Show(weaponName);
            isPlayerInRange = true;
        }

        if (WeaponNameUI.Instance != null && !isCurrentlyInRange && isPlayerInRange)
        {
            WeaponNameUI.Instance.Hide();
            isPlayerInRange = false;
        }

        if (!isPickedUp && isCurrentlyInRange && Input.GetKeyDown(KeyCode.E))
        {
            weaponSwitcher.ActivateWeaponByIndex(weaponIndexToActivate);

            if (objectiveManager != null)
            {
                objectiveManager.MarkWeaponPicked();
            }

            isPickedUp = true;

            if (WeaponNameUI.Instance != null)
                WeaponNameUI.Instance.Hide();

            PlayPickupEffects();
        }
    }

    private void PlayPickupEffects()
    {
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        // Hide weapon visuals/colliders
        foreach (var renderer in GetComponentsInChildren<Renderer>())
            renderer.enabled = false;

        foreach (var col in GetComponentsInChildren<Collider>())
            col.enabled = false;

        Destroy(gameObject, 1f); // Destroy after delay so sound can play
    }
}
