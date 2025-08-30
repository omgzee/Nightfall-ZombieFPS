using UnityEngine;
using UnityEngine.SceneManagement;

public class GameoverScreen : MonoBehaviour
{
    public GameObject gameOverUI;
    public GameObject crosshair;

    public MonoBehaviour cameraController; // Assign your camera control script here in the Inspector

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame called.");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void ShowGameOver()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;
        crosshair.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (cameraController != null)
            cameraController.enabled = false; 
    }
}
