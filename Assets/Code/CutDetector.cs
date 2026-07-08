using UnityEngine;
using UnityEngine.Events;

public class CutDetector : MonoBehaviour
{
    [Header("判定设置")]
    [Tooltip("设定的完美切断时间（秒）")]
    public float targetTime = 3.0f; // 假设铁管伸出后第3秒是最佳切断点

    [Tooltip("Perfect 允许的时间误差")]
    public float perfectWindow = 0.05f;

    [Tooltip("Great 允许的时间误差")]
    public float greatWindow = 0.12f;

    [Header("スコア設定")]
    [SerializeField] private int perfectScore = 100;
    [SerializeField] private int greatScore = 50;

    [Header("判定結果イベント")]
    [Tooltip("InspectorでJudgementDisplay.ShowPerfect等を接続する")]
    public UnityEvent onPerfect;
    public UnityEvent onGreat;
    public UnityEvent onMiss;

    [Tooltip("InspectorでGameHUDManager.AddScore(int)を接続する。引数は加算スコア")]
    public IntUnityEvent onScoreAdded;

    // 临时用来模拟游戏运行的时间计时器
    private float gameTimer = 0f;
    private bool isCut = false;

    void Update()
    {
        // 游戏开始后时间不断增加
        gameTimer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. 检查碰上来的是不是刀，而且还没被切断过
        if (other.CompareTag("Knife") && !isCut)
        {
            // 2. 计算当前切下去的时间，和目标完美时间的差值（取绝对值）
            float timeDiff = Mathf.Abs(gameTimer - targetTime);

            // 3. 执行打分逻辑
            CalculateScore(timeDiff);
            
            isCut = true; // 防止一刀穿过去触发两次
        }
    }

    private void CalculateScore(float diff)
    {
        if (diff <= perfectWindow)
        {
            Debug.Log($"<color=green>【Perfect!!】</color> 误差: {diff:F3}秒");
            onPerfect?.Invoke();
            onScoreAdded?.Invoke(perfectScore);
        }
        else if (diff <= greatWindow)
        {
            Debug.Log($"<color=yellow>【Great!】</color> 误差: {diff:F3}秒");
            onGreat?.Invoke();
            onScoreAdded?.Invoke(greatScore);
        }
        else
        {
            Debug.Log($"<color=red>【Miss...】</color> 误差: {diff:F3}秒 (太早或太晚)");
            onMiss?.Invoke();
        }

        // 测试阶段：2秒后重置状态，方便你在场景里反复拖拽测试
        Invoke("ResetCutState", 2f);
    }

    private void ResetCutState()
    {
        isCut = false;
        gameTimer = 0f; // 重置时间，再切一次
        Debug.Log("--- 铁管已重置，可以再次测试切断 ---");
    }
}

// UnityEvent<int>はInspectorに表示するため非ジェネリックなサブクラスが必要
[System.Serializable]
public class IntUnityEvent : UnityEvent<int> { }