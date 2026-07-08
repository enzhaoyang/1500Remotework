using System.Collections;
using UnityEngine;

public class BlacksmithPipe : MonoBehaviour
{
    [Header("移动与销毁")]
    public float moveSpeed = 3.0f;

    [Header("打铁设置")]
    [Tooltip("被左手抓住后，需要等待多少秒才能砍")]
    public float waitTimeBeforeCut = 2.0f; 

    // 定义铁管的四种状态
    private enum PipeState { Flying, InZone, Grabbed, ReadyToCut }
    private PipeState currentState = PipeState.Flying;

    private float timer = 0f;

    void Start()
    {
        // 【修复2】把销毁代码放在 Start 里！
        // 铁管一出生就开始倒计时，5秒后必定销毁，绝不占用内存
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        // 状态1和2：如果没有被抓住，就一直往前飞
        if (currentState == PipeState.Flying || currentState == PipeState.InZone)
        {
            // 这是你要的最重要的代码行：向我的正前方、基于自身局部坐标移动
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.Self);

            // ⚠️ 物理清理代码（之前给你的正确版代码已经去掉了 Update 里的 Destroy，请确认你使用的是正确版本）
        }
        // 状态3：被左手抓住了，停在原地开始倒计时
        else if (currentState == PipeState.Grabbed)
        {
            timer += Time.deltaTime;
            if (timer >= waitTimeBeforeCut)
            {
                Debug.Log("叮！铁管烧红了！现在可以砍了！");
                currentState = PipeState.ReadyToCut; // 进入可切断状态
                
                // 视觉提示：把铁管变成红色，提醒玩家可以砍了！
                GetComponent<Renderer>().material.color = Color.red;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. 铁管碰到了黄色的判定框 -> 变成【可抓取】状态
        if (other.CompareTag("Zone") && currentState == PipeState.Flying)
        {
            currentState = PipeState.InZone;
        }

        // 2. 碰到了左手 -> 【改动】：去掉前置条件，只要左手碰到，无脑抓住！
        if (other.CompareTag("LeftHand")) 
        {
            currentState = PipeState.Grabbed; // 改变状态，停止移动
            Debug.Log("左手抓住了！开始加热...");
            
            // 让铁管变成左手的子物体
            transform.SetParent(other.transform); 
        }

        // 3. 碰到了右手拿的刀！
        if (other.CompareTag("Knife"))
        {
            if (currentState == PipeState.ReadyToCut)
            {
                Debug.Log("Perfect! 完美的锻造！");
                Destroy(gameObject);
            }
            else if (currentState == PipeState.Grabbed)
            {
                Debug.Log("Bad! 太急了，还没烧红呢！");
            }
            else
            {
                Debug.Log("Bad! 必须先用左手接住它！");
            }
        }
    }
}