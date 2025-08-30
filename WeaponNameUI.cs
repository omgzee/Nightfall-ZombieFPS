using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class WeaponNameUI : MonoBehaviour
{
    public static WeaponNameUI Instance;

    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private CanvasGroup canvasGroup;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        canvasGroup.alpha = 0f;
    }

    public void Show(string name)
    {
        Debug.Log("Show called with: " + name);
        weaponNameText.text = name;

        if (canvasGroup != null)
        {
            // Ensure the UI fades in smoothly
            if (fadeRoutine != null) StopCoroutine(fadeRoutine); // Stop any existing fade
            fadeRoutine = StartCoroutine(FadeTo(1f)); // Fade in
            Debug.Log("CanvasGroup alpha set to: " + canvasGroup.alpha);
        }
        else
        {
            Debug.LogWarning("CanvasGroup not assigned!");
        }
    }

    public void Hide()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine); // Stop any ongoing fade
        fadeRoutine = StartCoroutine(FadeTo(0f)); // Fade out
        Debug.Log("Weapon UI hidden.");
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        float duration = 0.2f;
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}
