using UnityEngine;
using UnityEngine.UI;

public class HorrorFadeIn : MonoBehaviour
{
    public float duration = 3f;
    private Image fadeImage;
    private float timer = 0f;

    void Start()
    {
        fadeImage = GetComponent<Image>();
        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;
    }

    void Update()
    {
        if (timer < duration)
        {
            timer += Time.deltaTime;
            Color c = fadeImage.color;
            c.a = Mathf.Lerp(1f, 0f, timer / duration);
            fadeImage.color = c;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
