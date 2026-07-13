using UnityEngine;

public class BlacksmithPipe : MonoBehaviour
{
    [Header("移动与销毁")]
    public float moveSpeed = 3.0f;
    public float missZPosition = -2.0f;

    [Header("打铁设置")]
    [Tooltip("被左手抓住后，需要等待多少秒才能砍")]
    public float waitTimeBeforeCut = 2.0f; 

    // 定义铁管的四种状态
    private enum PipeState { Flying, InZone, Grabbed, ReadyToCut }
    private PipeState currentState = PipeState.Flying;

    private float timer = 0f;

    void Update()
    {
        // 状态1和2：如果没有被抓住，就一直往前飞
        if (currentState == PipeState.Flying || currentState == PipeState.InZone)
        {
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);

            // 飞过头了，回收垃圾
            if (transform.position.z < missZPosition)
            {
                Debug.Log("Miss... 铁管掉地上了！");
                Destroy(gameObject);
            }
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

        // 2. 碰到了左手 -> 如果在框里，就被抓住！
        if (other.CompareTag("LeftHand") && currentState == PipeState.InZone)
        {
            currentState = PipeState.Grabbed; // 改变状态，停止移动
            Debug.Log("左手抓住了！开始加热...");
            
            // 神奇的交互：让铁管变成左手的子物体，这样它就会死死黏在你的左手上！
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