using UnityEngine;
using TMPro;
using System.Collections; // 必须引入这个才能使用协程时间轴

public class BlacksmithManager : MonoBehaviour
{
    public static BlacksmithManager Instance;

    [Header("⚠️ 必须按顺序拖入：0=左, 1=中, 2=右")]
    public Transform[] spawnPoints;
    
    public GameObject pipePrefab;

    [Header("节奏设置 (秒)")]
    public float spawnInterval = 3f; // 两个铁棒出现的总间隔
    public float warningTime = 1f;   // 提前多少秒给出预警提示？

    [Header("UI 引用")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI warningText; // 【新增】拖入刚刚做好的 WarningText
    
    public int currentScore = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        UpdateScoreDisplay(); 
        if (warningText != null) warningText.text = ""; // 游戏开始时清空提示
        
        // 启动流水线协程
        StartCoroutine(SpawnRoutine()); 
    }

    // ⏳ 核心时间轴：流水线循环
    IEnumerator SpawnRoutine()
    {
        // 游戏刚开始，先等 2 秒给玩家准备时间
        yield return new WaitForSeconds(2f);

        while (true) // 开启无限打铁循环
        {
            if (spawnPoints.Length == 3 && pipePrefab != null)
            {
                // 1. 掷骰子，决定下一个从哪出 (0左, 1中, 2右)
                int randomIndex = Random.Range(0, spawnPoints.Length);
                Transform selectedSpawner = spawnPoints[randomIndex];
                
                // 2. 在眼前亮起预警提示！
                if (warningText != null)
                {
                    if (randomIndex == 0) 
                        warningText.text = "Ready：left！";
                    else if (randomIndex == 1) 
                        warningText.text = "Ready：middle！";
                    else 
                        warningText.text = "Ready：right！";
                        
                    warningText.color = Color.yellow; // 变成醒目的警告黄
                }

                // 3. 等待预警时间 (比如让提示在眼前亮 1 秒)
                yield return new WaitForSeconds(warningTime);

                // 4. 时间到！清除眼前提示，并“吐”出铁棒
                if (warningText != null) warningText.text = "";
                Instantiate(pipePrefab, selectedSpawner.position, selectedSpawner.rotation);

                // 5. 等待剩余的间隔时间，再进入下一轮循环
                // 比如总间隔 3 秒，提示占了 1 秒，这里就再等 2 秒
                yield return new WaitForSeconds(spawnInterval - warningTime);
            }
            else
            {
                // 防止还没配置好数组时游戏卡死
                yield return null; 
            }
        }
    }

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreDisplay();
        Debug.Log("得分了！当前分数：" + currentScore);
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null) scoreText.text = "SCORE: " + currentScore;
    }
}