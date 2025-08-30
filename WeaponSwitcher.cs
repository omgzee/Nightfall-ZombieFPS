using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSwitcher : MonoBehaviour
{
    public int currentWeapon = 0;
    private bool isReloading = false;
    private Weapon currentWeaponScript;

    [Header("Weapon HUD UI")]
    [SerializeField] private GameObject pistolIcon;
    [SerializeField] private GameObject carbineIcon;
    [SerializeField] private GameObject shotgunIcon;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Image weaponIcon;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject weaponHUD;

    private bool hasWeapon = false;
    private bool[] weaponsPickedUp;

    [Header("Weapon Pickup Feedback")]
    [SerializeField] private AudioClip weaponPickupSound;
    [SerializeField] private GameObject weaponPickupEffect;

    [Header("Weapon Holder")]
    [SerializeField] private Transform weaponHolder;

    private void Start()
    {
        weaponsPickedUp = new bool[weaponHolder.childCount];

        if (weaponHolder != null)
        {
            foreach (Transform child in weaponHolder)
            {
                child.gameObject.SetActive(false);
            }
        }

        if (transform.childCount > 0 && transform.GetChild(currentWeapon).gameObject.activeSelf)
        {
            SetWeaponActive();
        }
        else
        {
            HideWeaponUI();
        }
    }

    private void Update()
    {
        // Continuously update reloading status from current weapon
        if (currentWeaponScript != null)
        {
            isReloading = currentWeaponScript.IsReloading();
        }

        if (isReloading) return;

        int previousWeapon = currentWeapon;
        ProcessInput();

        if (hasWeapon)
        {
            ProcessScrollWheel();
        }

        if (previousWeapon != currentWeapon)
        {
            if (!isReloading)
            {
                SetWeaponActive();
            }
            else
            {
                currentWeapon = previousWeapon;
            }
        }
    }

    private void ProcessScrollWheel()
    {
        if (!hasWeapon || isReloading) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            if (scroll > 0)
            {
                currentWeapon = GetNextWeapon(currentWeapon, 1); 
            }
            else if (scroll < 0)
            {
                currentWeapon = GetNextWeapon(currentWeapon, -1); 
            }

            SetWeaponActive();
        }
    }

    private int GetNextWeapon(int current, int direction)
    {
        int newWeapon = current;
        do
        {
            newWeapon = (newWeapon + direction + weaponHolder.childCount) % weaponHolder.childCount;
        } while (!weaponsPickedUp[newWeapon]);

        return newWeapon;
    }

    private void ProcessInput()
    {
        if (!hasWeapon || isReloading) return;

        if (Input.GetKeyDown(KeyCode.Alpha1) && weaponHolder.childCount >= 1 && weaponsPickedUp[0]) currentWeapon = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2) && weaponHolder.childCount >= 2 && weaponsPickedUp[1]) currentWeapon = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3) && weaponHolder.childCount >= 3 && weaponsPickedUp[2]) currentWeapon = 2;
    }

    private void SetWeaponActive()
    {
        bool weaponFound = false;

        for (int i = 0; i < weaponHolder.childCount; i++)
        {
            Transform weapon = weaponHolder.GetChild(i);
            bool isActive = i == currentWeapon && weaponsPickedUp[i];
            weapon.gameObject.SetActive(isActive);

            if (isActive)
            {
                currentWeaponScript = weapon.GetComponent<Weapon>();
                if (currentWeaponScript != null)
                {
                    UpdateHUD(i);
                }
                weaponFound = true;
            }
        }

        if (!weaponFound)
        {
            HideWeaponUI();
            hasWeapon = false;
        }
        else
        {
            hasWeapon = true;
        }
    }

    private void UpdateHUD(int index)
    {
        pistolIcon?.SetActive(index == 0);
        carbineIcon?.SetActive(index == 1);
        shotgunIcon?.SetActive(index == 2);

        if (weaponIcon != null)
        {
            weaponIcon.sprite = GetWeaponIcon(index);
        }

        if (ammoText != null && weaponHolder.GetChild(index).GetComponent<Weapon>() != null)
        {
            Weapon weaponScript = weaponHolder.GetChild(index).GetComponent<Weapon>();
            ammoText.text = $"{weaponScript.GetCurrentAmmo()} / {weaponScript.GetMaxAmmo()}";
        }

        crosshair?.SetActive(true);
        weaponHUD?.SetActive(true);
    }

    private Sprite GetWeaponIcon(int index)
    {
        switch (index)
        {
            case 0: return pistolIcon.GetComponent<Image>().sprite;
            case 1: return carbineIcon.GetComponent<Image>().sprite;
            case 2: return shotgunIcon.GetComponent<Image>().sprite;
            default: return null;
        }
    }

    private void HideWeaponUI()
    {
        crosshair?.SetActive(false);
        weaponHUD?.SetActive(false);
    }

    public void ActivateWeaponByIndex(int index)
    {
        if (index >= 0 && index < weaponHolder.childCount)
        {
            Transform weapon = weaponHolder.GetChild(index);
            weapon.gameObject.SetActive(true);
            weaponsPickedUp[index] = true;
            currentWeapon = index;
            SetWeaponActive();
            PlayWeaponPickupFeedback();
        }
    }

    public void PickUpWeapon()
    {
        hasWeapon = true;
    }

    private void PlayWeaponPickupFeedback()
    {
        if (weaponPickupSound)
            AudioSource.PlayClipAtPoint(weaponPickupSound, transform.position);

        if (weaponPickupEffect)
            Instantiate(weaponPickupEffect, transform.position, Quaternion.identity);
    }
}
