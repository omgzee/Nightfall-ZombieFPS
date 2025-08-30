using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] private int ammoAmount = 50;
    [SerializeField] private AmmoType ammoType;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private GameObject pickupEffect;

    private Ammo ammo;

    private void Start()
    {
        ammo = FindFirstObjectByType<Ammo>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && ammo != null)
        {
            ammo.IncreaseCurrentAmmo(ammoType, ammoAmount);

            if (pickupSound)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            if (pickupEffect)
                Instantiate(pickupEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
