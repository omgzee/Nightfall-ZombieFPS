using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using StarterAssets;

public class PlayerHealth : MonoBehaviour
{
    private float maxHealth = 100f;
    public float currentHealth;

    public HealthBar healthBar;
    public GameoverScreen gameoverScreen;

    [Header("Player Damage Screen Settings")]
    public Image damageScreen;
    public float duration;
    public float fadeSpeed = 1f;
    private float durationTimer;

    [Header("Player Hurt Sounds")]
    public AudioClip[] hurtSounds; 
    private AudioSource audioSource;
    

    public AudioSource deathSplatter;
    private bool isDead = false;


    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
        else
            Debug.LogWarning("HealthBar not assigned to PlayerHealth!");

        damageScreen.color = new Color(damageScreen.color.r, damageScreen.color.g, damageScreen.color.b, 0);
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            Debug.LogWarning("No AudioSource found on Player for hurt sounds!");
    }

    void Update()
    {
        if (damageScreen.color.a > 0)
        {
            durationTimer += Time.deltaTime;
            if (durationTimer > duration)
            {
                float tempAlpha = damageScreen.color.a;
                tempAlpha -= fadeSpeed * Time.deltaTime;
                damageScreen.color = new Color(damageScreen.color.r, damageScreen.color.g, damageScreen.color.b, tempAlpha);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        durationTimer = 0;
        damageScreen.color = new Color(damageScreen.color.r, damageScreen.color.g, damageScreen.color.b, 0.3f);
        Debug.Log("Player took damage, remaining health: " + currentHealth);

        // 🔊 Play random hurt sound
        if (hurtSounds.Length > 0 && audioSource != null)
        {
            AudioClip randomClip = hurtSounds[Random.Range(0, hurtSounds.Length)];
            audioSource.PlayOneShot(randomClip);
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDead = true;
            StartCoroutine(HandleDeathSequence());
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.SetHealth(currentHealth);
        Debug.Log("Player healed. Current health: " + currentHealth);
    }

    private IEnumerator HandleDeathSequence()
    {
        if (deathSplatter != null && deathSplatter.clip != null)
            deathSplatter.PlayOneShot(deathSplatter.clip);

        if (TryGetComponent<FirstPersonController>(out var movement))
            movement.enabled = false;

        Quaternion fallRotation = Quaternion.Euler(-90f, transform.rotation.eulerAngles.y, 0f);
        float timer = 0f;
        float fallDuration = 1f;

        Quaternion initialRotation = transform.rotation;

        while (timer < fallDuration)
        {
            transform.rotation = Quaternion.Slerp(initialRotation, fallRotation, timer / fallDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.rotation = fallRotation;
        GetComponent<DeathHandler>().HandleDeath();
        gameoverScreen.ShowGameOver();
    }


}
