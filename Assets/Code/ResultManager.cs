using UnityEngine;
using TMPro; // 控制 TextMeshPro
using UnityEngine.SceneManagement; // 控制场景切换

public class ResultManager : MonoBehaviour
{
    [Header("UI 文本绑定")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI perfectText;
    public TextMeshProUGUI greatText;
    public TextMeshProUGUI missText;
    public TextMeshProUGUI rankText;

    void Start()
    {
        // 1. 读取 GamePlay 存下来的数据（如果没有，默认给 0）
        int finalScore = PlayerPrefs.GetInt("TotalScore", 0);
        int perfects = PlayerPrefs.GetInt("PerfectCount", 0);
        int greats = PlayerPrefs.GetInt("GreatCount", 0);
        int misses = PlayerPrefs.GetInt("MissCount", 0);

        // 2. 把数据填进 UI 里
        scoreText.text = $"SCORE : {finalScore}";
        perfectText.text = $"PERFECT : {perfects}";
        greatText.text = $"GREAT : {greats}";
        missText.text = $"MISS : {misses}";

        // 3. 顺便做一个简单的评级系统 (Rank)
        if (finalScore >= 500) rankText.text = "RANK : S";
        else if (finalScore >= 300) rankText.text = "RANK : A";
        else if (finalScore >= 100) rankText.text = "RANK : B";
        else rankText.text = "RANK : C";
    }

    // ====== 按钮点击事件 ======

    // 点击 Retry 时调用
    public void OnRetryClicked()
    {
        Debug.Log("再玩一次！");
        // 注意：这里的名字必须和你下面 Project 面板里的场景名字一模一样
        SceneManager.LoadScene("GamePlay"); 
    }

    // 点击 Menu 时调用
    public void OnMenuClicked()
    {
        Debug.Log("返回主菜单！");
        SceneManager.LoadScene("start"); 
    }
}