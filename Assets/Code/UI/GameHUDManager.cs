using UnityEngine;
using TMPro;

public class GameHUDManager : MonoBehaviour
{
    [Header("HUD テキスト")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;

    private int score = 0;
    private int combo = 0;
    private float timeRemaining = 60f;
    private bool isGameRunning = true;

    void Update()
    {
        if (!isGameRunning) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isGameRunning = false;
        }

        UpdateHUD();
    }

    private void UpdateHUD()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timeText.text  = $"TIME: {minutes:00}:{seconds:00}";
        scoreText.text = $"SCORE: {score}";
        comboText.text = $"COMBO: {combo}";
    }

    // CutDetector等から呼び出す
    public void AddScore(int points)
    {
        score += points;
        combo++;
    }

    public void ResetCombo()
    {
        combo = 0;
    }
}
