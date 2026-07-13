using UnityEngine;
using UnityEngine.Events;

public class CutDetector : MonoBehaviour
{
    [Header("销毁设置")]
    [Tooltip("铁管飞到玩家身后多远时自动销毁（Z轴坐标）")]
    public float missZPosition = -2.0f;

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

    void Update()
    {
        // 垃圾回收：如果铁管飞过了玩家身后（Z坐标小于设定值），就判定为 Miss 并销毁
        if (transform.position.z < missZPosition)
        {
            Debug.Log("Miss... 铁管飞走了！");
            onMiss?.Invoke();
            Destroy(gameObject); // 销毁自己
        }
    }

    // 当有其他碰撞体（比如刀）进入铁管的触发器时
    private void OnTriggerEnter(Collider other)
    {
        // 判断碰我们的是不是标签为 "Knife" 的物体
        if (other.CompareTag("Knife"))
        {
            // 这里可以接入更复杂的逻辑，比如判断此时铁管是不是在那个“判定框”里面
            Debug.Log("Perfect! 切割成功！");
            onPerfect?.Invoke();
            onScoreAdded?.Invoke(perfectScore);

            // 稍后我们可以换成断成两截的动画或粒子特效，现在先直接让它消失
            Destroy(gameObject);
        }
    }
}

// UnityEvent<int>はInspectorに表示するため非ジェネリックなサブクラスが必要
[System.Serializable]
public class IntUnityEvent : UnityEvent<int> { }
