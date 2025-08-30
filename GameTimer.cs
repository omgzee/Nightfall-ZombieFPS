using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float timeRemaining = 1800f; // 30 minutes in seconds
    public TextMeshProUGUI timerText;

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();
        }
        else
        {
            timeRemaining = 0;
            timerText.text = "MILITARY ARRIVED";
            // Call game over logic here if needed
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = $"MILITARY ARRIVAL IN: {minutes:00}:{seconds:00}";
    }
}
