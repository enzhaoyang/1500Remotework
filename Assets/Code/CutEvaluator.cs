using UnityEngine;

public class CutEvaluator : MonoBehaviour
{
    [Header("🎯 目标任务设定")]
    [Tooltip("本次客人要求的铁棒长度 (米/单位)")]
    public float targetLength = 5.0f; 

    [Header("⚖️ 判定区间 (正负公差)")]
    [Tooltip("Perfect 判定范围 (例如 0.2，即 4.8 ~ 5.2)")]
    public float perfectTolerance = 0.2f; 
    [Tooltip("Great 判定范围 (例如 0.5，即 4.5 ~ 5.5)")]
    public float greatTolerance = 0.5f;   

    [Header("🏆 得分设置")]
    public int perfectScore = 100;
    public int greatScore = 50;
    public int missScore = 10; // 即使Miss也给点保底分，或者设为0

    // 这个总分可以之后交给你们的 GameManager 统一管理
    private int totalScore = 0;

    /// <summary>
    /// 当玩家的刀砍中铁棒时，调用这个方法，并把砍下的实际长度传进来
    /// </summary>
    /// <param name="actualLength">玩家实际切出的长度</param>
    public void EvaluateCut(float actualLength)
    {
        // 1. 计算误差的绝对值 (不管切长了还是切短了，只看差了多少)
        float difference = Mathf.Abs(actualLength - targetLength);

        // 2. 开始判定等级
        if (difference <= perfectTolerance)
        {
            TriggerPerfect(actualLength);
        }
        else if (difference <= greatTolerance)
        {
            TriggerGreat(actualLength);
        }
        else
        {
            TriggerMiss(actualLength);
        }
    }

    // --- 下面是具体的反馈表现 ---

    private void TriggerPerfect(float length)
    {
        totalScore += perfectScore;
        Debug.Log($"<color=yellow><b>PERFECT!</b></color> Goal:{targetLength} 实际:{length:F2} | 得分: +{perfectScore}");
        
        // TODO: 在这里播放 Perfect 的炫酷音效
        // TODO: 在 UI 画布上弹出黄色的 "PERFECT!!" 艺术字特效
    }

    private void TriggerGreat(float length)
    {
        totalScore += greatScore;
        Debug.Log($"<color=cyan><b>GREAT!</b></color> Goal:{targetLength} 实际:{length:F2} | 得分: +{greatScore}");
        
        // TODO: 在这里播放 Great 音效
        // TODO: 在 UI 画布上弹出青色的 "GREAT!" 特效
    }

    private void TriggerMiss(float length)
    {
        totalScore += missScore;
        Debug.Log($"<color=red><b>MISS...</b></color> Goal:{targetLength} 实际:{length:F2} | 误差太大啦!");
        
        // TODO: 播放打铁失败的沉闷音效 (比如“哐当”一声)
        // TODO: 弹出红色的 "MISS" 提示
    }
}