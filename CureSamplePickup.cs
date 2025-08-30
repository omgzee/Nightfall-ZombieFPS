using UnityEngine;

public class CureSamplePickup : MonoBehaviour
{
    public GameObject pickupPromptUI;
    public GameObject cureFoundText;
    public AudioClip pickupSound;
    public GameObject pickupEffect; // Optional VFX prefab

    private bool isNearPlayer = false;
    private bool pickedUp = false;

    private void Start()
    {
        pickupPromptUI.SetActive(false);
        cureFoundText.SetActive(false);
    }

    private void Update()
    {
        if (isNearPlayer && !pickedUp && Input.GetKeyDown(KeyCode.E))
        {
            pickedUp = true;

            // Show acquired text
            cureFoundText.SetActive(true);
            Invoke("HideCureText", 2f);

            // Play sound
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            // Play visual effect
            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }

            // Inform objective system
            FindObjectOfType<ObjectiveManager>().FoundCureSample();

            // Disable object
            pickupPromptUI.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !pickedUp)
        {
            isNearPlayer = true;
            pickupPromptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearPlayer = false;
            pickupPromptUI.SetActive(false);
        }
    }

    private void HideCureText()
    {
        cureFoundText.SetActive(false);
    }
}
