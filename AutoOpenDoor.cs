using UnityEngine;

public class AutoOpenDoor : MonoBehaviour
{
    public Animator doorAnimator;
    public string openTriggerName = "Open"; // Name of the animation trigger

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorAnimator.SetTrigger("Open");
        }
    }
}
