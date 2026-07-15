using UnityEngine;
using UnityEngine.Events;

public class CutDetector : MonoBehaviour
{
    [Header("销毁设置")]
    public float missZPosition = -2.0f;

    public UnityEvent onPerfect;
    public UnityEvent onGreat;
    public UnityEvent onMiss;
    public IntUnityEvent onScoreAdded;

    private IronCutHUD hudManager;
    
    // 物理起跑线的坐标
    private Vector3 startPos;
    // 是否已经离开起跑线，开始计米数
    private bool isCounting = false;

    void Start()
    {
        hudManager = FindObjectOfType<IronCutHUD>();
    }

    void Update()
    {
        // 只要开始测距，就每帧算出自己跑了多远，告诉大屏幕
        if (isCounting && hudManager != null)
        {
            float distanceTravelled = Vector3.Distance(transform.position, startPos);
            hudManager.UpdatePhysicalDistance(distanceTravelled);
        }

        // 错过销毁逻辑
        if (transform.position.z < missZPosition)
        {
            if (hudManager != null) hudManager.StartNewRound(RandomTarget());
            onMiss?.Invoke();
            Destroy(gameObject);
        }
    }

    // ====== 核心：离开起跑区，开始测距！ ======
    private void OnTriggerExit(Collider other)
    {
        // 如果我们离开了那个带有 "StartZone" 标签的门
        if (other.CompareTag("StartZone") && !isCounting)
        {
            startPos = transform.position; // 锁定此时的坐标为 0米 刻度
            isCounting = true; // 开始推数据给屏幕
        }
    }

    // 刀砍判定
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Knife"))
        {
            if (hudManager != null)
            {
                // 砍中的瞬间，算出最终距离，扔给屏幕去判定！
                float finalDist = isCounting ? Vector3.Distance(transform.position, startPos) : 0f;
                hudManager.EvaluateCut(finalDist);
            }
            Destroy(gameObject);
        }
    }
    
    private float RandomTarget()
    {
        return Mathf.Round(Random.Range(3.0f, 7.0f) * 10f) / 10f;
    }
}

[System.Serializable]
public class IntUnityEvent : UnityEvent<int> { }