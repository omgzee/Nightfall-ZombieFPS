using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUIManager : MonoBehaviour
{
    public Image pistolImage;
    public Image rifleImage;
    public Image shotgunImage;
    public TextMeshProUGUI ammoText;

    public void UpdateWeaponUI(string weaponName, int currentAmmo, int maxAmmo)
    {
        // Hide all
        pistolImage.gameObject.SetActive(false);
        rifleImage.gameObject.SetActive(false);
        shotgunImage.gameObject.SetActive(false);

        // Show relevant
        switch (weaponName)
        {
            case "Pistol":
                pistolImage.gameObject.SetActive(true);
                break;
            case "Rifle":
                rifleImage.gameObject.SetActive(true);
                break;
            case "Shotgun":
                shotgunImage.gameObject.SetActive(true);
                break;
        }

        // Update ammo
        ammoText.text = $"{currentAmmo} / {maxAmmo}";
    }
}
