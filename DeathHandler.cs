using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class DeathHandler : MonoBehaviour
{
    [SerializeField] Canvas gameOverCanvas;
    [SerializeField] GameObject reticle;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameOverCanvas.enabled = false;
    }

    public void HandleDeath()
    {
        gameOverCanvas.enabled = true;
        reticle.SetActive(false);
        Time.timeScale = 0f; // Pause the game
        AudioListener.pause = true;
        

    }
    
}
