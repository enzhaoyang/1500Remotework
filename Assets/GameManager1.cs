using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    [Header("=== Result UI ===")]
    public TextMeshProUGUI resultScoreText;   
    public TextMeshProUGUI perfectCountText; 
    public TextMeshProUGUI greatCountText;   
    public TextMeshProUGUI missCountText;    
    public TextMeshProUGUI rankText;         

    [Header("=== Scene Names (Must be in Build Settings) ===")]
    [SerializeField] private string gamePlaySceneName = "GamePlay";
    [SerializeField] private string mainMenuSceneName = "start";

    void Start()
    {
        ShowResult();
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(gamePlaySceneName);
    }

    public void LoadStartScene()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    void ShowResult()
    {
        int score = PlayerPrefs.GetInt("TotalScore", 0);
        int perfects = PlayerPrefs.GetInt("PerfectCount", 0);
        int greats = PlayerPrefs.GetInt("GreatCount", 0);
        int misses = PlayerPrefs.GetInt("MissCount", 0);

        if (resultScoreText != null) 
            resultScoreText.text = "SCORE : " + score.ToString();

        // 判定次数的颜色 (Perfect=黄, Great=青, Miss=红)
        if (perfectCountText != null) 
        {
            perfectCountText.text = $"PERFECT  {perfects} ";
            perfectCountText.color = Color.yellow;
        }

        if (greatCountText != null) 
        {
            greatCountText.text = $"GREAT    {greats} ";
            greatCountText.color = Color.cyan;
        }

        if (missCountText != null) 
        {
            missCountText.text = $"MISS     {misses} ";
            missCountText.color = Color.red;
        }

        // 核心修改：1分钟的 S, A, B, C 评级与颜色
        if (rankText != null)
        {
            if (score >= 1000) 
            {
                rankText.text = "S";
                rankText.color = Color.yellow; // 金色 S级
            } 
            else if (score >= 700) 
            {
                rankText.text = "A";
                rankText.color = new Color(1f, 0.5f, 0f); // 橙色 A级
            } 
            else if (score >= 400)
            {
                rankText.text = "B";
                rankText.color = Color.green; // 绿色 B级
            }
            else 
            {
                rankText.text = "C";
                rankText.color = Color.black; // 灰色 C级 (不及格)
            }
        }
    }
}