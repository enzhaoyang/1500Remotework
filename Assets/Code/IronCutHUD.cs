using UnityEngine;
using TMPro;

public class IronCutHUD : MonoBehaviour
{
    [Header("UI 文本")]
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI currentText;
    public TextMeshProUGUI resultText;

    [Header("判定参数")]
    public float perfectRange = 0.2f;
    public float greatRange = 0.5f;

    [Header("得分设置 (梯度分)")]
    public int perfectScore = 100; // Perfect 给 100分
    public int greatScore = 50;    // Great 给 50分
    public int missScore = 10;     // Miss 给 10分 (或者改成0)

    private float targetLength;
    private bool isRunning = false;

    void Start()
    {
        // 游戏一开始，重置所有的统计数据
        PlayerPrefs.SetInt("TotalScore", 0);
        PlayerPrefs.SetInt("PerfectCount", 0);
        PlayerPrefs.SetInt("GreatCount", 0);
        PlayerPrefs.SetInt("MissCount", 0);
        
        StartNewRound(5.0f);
    }

    public void StartNewRound(float newTarget)
    {
        targetLength = newTarget;
        targetText.text = $"Goal: {targetLength:F2}m";
        currentText.text = $"Current: 0.00m";
        resultText.text = "";
        isRunning = true;
    }

    public void UpdatePhysicalDistance(float realDistance)
    {
        if (isRunning)
        {
            currentText.text = $"Current: {realDistance:F2}m";
        }
    }

    public void EvaluateCut(float finalLength)
    {
        if (!isRunning) return;
        isRunning = false;

        float diff = Mathf.Abs(finalLength - targetLength);
        int earnedScore = 0; // 这次砍中了多少分

        // 1. 判断等级，记录次数，并决定获得多少分
        if (diff <= perfectRange) 
        {
            ShowResult("PERFECT!!", Color.yellow);
            PlayerPrefs.SetInt("PerfectCount", PlayerPrefs.GetInt("PerfectCount", 0) + 1);
            earnedScore = perfectScore;
        }
        else if (diff <= greatRange) 
        {
            ShowResult("GREAT!", Color.cyan);
            PlayerPrefs.SetInt("GreatCount", PlayerPrefs.GetInt("GreatCount", 0) + 1);
            earnedScore = greatScore;
        }
        else 
        {
            ShowResult("MISS...", Color.red);
            PlayerPrefs.SetInt("MissCount", PlayerPrefs.GetInt("MissCount", 0) + 1);
            earnedScore = missScore;
        }

        // 2. ====== 核心修改：统一在这里加分，并同步保存 ======
        if (BlacksmithManager.Instance != null)
        {
            BlacksmithManager.Instance.AddScore(earnedScore);
            
            // 存下最新的总分，等时间到了进 Result 场景就能直接看到了
            PlayerPrefs.SetInt("TotalScore", BlacksmithManager.Instance.currentScore);
        }

        Invoke("StartRandomNextRound", 2.0f);
    }

    private void ShowResult(string message, Color color)
    {
        resultText.text = message;
        resultText.color = color;
    }

    private void StartRandomNextRound()
    {
        float nextTarget = Random.Range(3.0f, 7.0f);
        nextTarget = Mathf.Round(nextTarget * 10f) / 10f;
        StartNewRound(nextTarget);
    }
}