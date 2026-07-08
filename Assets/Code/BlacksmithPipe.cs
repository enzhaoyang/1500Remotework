using System.Collections;
using UnityEngine;

public class BlacksmithPipe : MonoBehaviour
{
    [Header("移动与销毁")]
    public float moveSpeed = 1.0f;

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
        // 【修改点1】：把 Grabbed 和 ReadyToCut 也加进来！
        // 这样只要铁棒还活着，它就会一直顺着原来的方向滑动，穿过你的手心！
        if (currentState == PipeState.Flying || currentState == PipeState.InZone || currentState == PipeState.Grabbed || currentState == PipeState.ReadyToCut)
        {
            // 保持你测试成功的 Vector3.up，绝不动它！
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.Self);
        }

        // 【修改点2】：把 else if 改成独立的 if
        // 状态3：被左手抓住了，一边在手心里滑动，一边开始加热倒计时
        if (currentState == PipeState.Grabbed)
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

        // 2. 碰到了左手 -> 无脑抓住！
        if (other.CompareTag("LeftHand")) 
        {
            currentState = PipeState.Grabbed; // 改变状态，停止移动
            Debug.Log("左手抓住了！开始加热...");
            
            // 1. 认左手做父物体
            transform.SetParent(other.transform); 
            
            // 2. 物理急刹车（最关键！）
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // 开启运动学，完全无视物理碰撞和受力
                rb.useGravity = false; // 彻底关掉重力
                
                // 清空它身上的所有惯性和速度！防止挥手时滑脱！
                rb.linearVelocity = Vector3.zero; 
                rb.angularVelocity = Vector3.zero; 
            }

            // 3. 吸附到掌心
            transform.localPosition = Vector3.zero; 
            
            // （可选）如果你发现吸到手上后，铁棒的方向歪了，取消下面这行的注释并修改数字
            // transform.localEulerAngles = new Vector3(90, 0, 0); 
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