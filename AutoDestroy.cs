using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] private float lifeTime = 10f;

    private void OnEnable()
    {
        Invoke(nameof(DestroySelf), lifeTime);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        CancelInvoke(); // safety cleanup
    }
}
