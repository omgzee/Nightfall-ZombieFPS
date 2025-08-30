using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using StarterAssets;

public class SurvivalTimer : MonoBehaviour
{
    public FirstPersonController playerController; // Reference to the player controller script
    public float timeRemaining = 100f;
    public TextMeshProUGUI timerText;
    public ObjectiveManager objectiveManager;
    public GameObject winScreen;
    public GameObject failScreen;


    [Header("End Game Sounds")]
    public AudioSource winAudioSource;
    public AudioSource loseAudioSource;

    [Header("Chopper Arrival Settings")]
    public GameObject helicopter;                // Reference to the chopper GameObject
    public Animator helicopterAnimator;          // Animator that controls chopper descend
    public AudioSource chopperAudioSource;       // Chopper sound to play
    private bool chopperStarted = false;

    private bool timerRunning = true;
    

    void Update()
    {
        if (timerRunning)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();

            if (!chopperStarted && timeRemaining <= 10f)
            {
                StartChopperArrival();
            }

            if (timeRemaining <= 0)
            {
                timerRunning = false;
                EndSurvivalPhase();
            }
        }

    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = $"TIME UNTIL MILITARY ARRIVES: {minutes:00}:{seconds:00}";
    }


    void StartChopperArrival()
    {
        chopperStarted = true;

        if (helicopter != null)
            helicopter.SetActive(true);                      // Show chopper in scene

        if (helicopterAnimator != null)
            helicopterAnimator.SetTrigger("Descend");        // Trigger landing animation

        if (chopperAudioSource != null)
            chopperAudioSource.Play();                       // Play chopper sound
    }

    void EndSurvivalPhase()
    {
        if (playerController != null)
            playerController.enabled = false;

        // Unlock and show cursor for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Show the correct UI screen and play audio BEFORE pausing time/audio
        if (objectiveManager.HasCureSample())
        {
            winScreen.SetActive(true);
            if (winAudioSource != null)
            {
                winAudioSource.Play();
            }
        }
        else
        {
            failScreen.SetActive(true);
            if (loseAudioSource != null)
            {
                loseAudioSource.Play();
            }
        }

        // Pause the game after a short real-time delay to allow sound to play
        StartCoroutine(PauseGameAfterDelay(0.1f));
    }
    private IEnumerator PauseGameAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // not affected by Time.timeScale
        Time.timeScale = 0f;
        AudioListener.pause = false; // Keep it false so UI audio can play
    }

    public void PlayAgain()
    {
        // Unpause the game
        Time.timeScale = 1f;
        AudioListener.pause = false;

        // Reset cursor state BEFORE reloading
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game"); // Works only in build, this confirms it in editor
    }


}
