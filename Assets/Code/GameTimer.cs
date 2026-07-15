using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // 1. Added this line for scene switching (Very Important!)

public class GameTimer : MonoBehaviour
{
    [Header("Drag the TMP Text component here")]
    public TextMeshProUGUI timerText;

    [Header("Set the time limit (in seconds) here")]
    public float timeLimit = 60f; 

    private float currentTime;
    private bool isTimerRunning = false;

    void Start()
    {
        // Set current time to the configured time limit when game starts
        currentTime = timeLimit;
        
        // Automatically start the timer
        StartTimer(); 
    }

    void Update()
    {
        if (isTimerRunning)
        {
            // Subtract time
            currentTime -= Time.deltaTime; 

            // Prevent time from dropping below zero
            if (currentTime <= 0)
            {
                currentTime = 0;
                isTimerRunning = false; // Stop the timer
                
                Debug.Log("Time's up! Loading Result scene...");
                
                // 2. Added this line: Load the Result scene when time hits 0
                SceneManager.LoadScene("Result"); 
            }

            UpdateTimerDisplay();
        }
    }

    // Format and display the timer
    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60F);
        int seconds = Mathf.FloorToInt(currentTime - minutes * 60);
        
        int milliseconds = Mathf.FloorToInt((currentTime * 100F) % 100F); 

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    // Timer control methods
    public void StartTimer() => isTimerRunning = true;
    public void StopTimer() => isTimerRunning = false;
    
    // Reset the timer to full time limit
    public void ResetTimer()
    {
        currentTime = timeLimit;
        UpdateTimerDisplay();
    }
}