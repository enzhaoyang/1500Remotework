using System.Collections;
using UnityEngine;

public class BlacksmithPipe : MonoBehaviour
{
    [Header("移动与销毁")]
    public float moveSpeed = 3.0f;
    public float grabbedMoveSpeed = 0.8f; 
    public float maxGrabTime = 3.0f;
    private float currentGrabTime = 0f; 

    [Header("音效与震动设置 (Feedback)")]
    public AudioClip warningSound;
    public AudioClip grabSound;
    public AudioClip cutSound;
    [Tooltip("左手持续摩擦的震动强度 (0~1)")]
    public float leftVibrateIntensity = 0.5f;
    [Tooltip("右手切断瞬间的爆发震动强度 (0~1)")]
    public float rightCutVibrateIntensity = 1.0f;

    private enum PipeState { Flying, InZone, ReadyToCut }
    private PipeState currentState = PipeState.Flying;

    private IronCutHUD hudManager;
    private Vector3 startPos;
    private bool isCounting = false;

    void Start()
    {
        hudManager = FindObjectOfType<IronCutHUD>();
        Destroy(gameObject, 8f); 

        if (warningSound != null) AudioSource.PlayClipAtPoint(warningSound, transform.position);
    }

    void Update()
    {
        if (currentState == PipeState.Flying || currentState == PipeState.InZone)
        {
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.Self);
        }
        else if (currentState == PipeState.ReadyToCut)
        {
            transform.Translate(Vector3.up * grabbedMoveSpeed * Time.deltaTime, Space.Self);
            currentGrabTime += Time.deltaTime;
            
            // ====== 核心修改 1：只要还抓着，左手就持续震动！ ======
            // OVRInput 的震动最多维持2秒，所以必须在 Update 里不断激活它
            OVRInput.SetControllerVibration(leftVibrateIntensity, leftVibrateIntensity, OVRInput.Controller.LTouch);

            if (currentGrabTime >= maxGrabTime)
            {
                Debug.Log("超时报废！");
                if (hudManager != null) hudManager.EvaluateCut(-999f);
                Destroy(gameObject); // 这里销毁会触发底部的 OnDestroy 关掉震动
            }
        }

        if (isCounting && hudManager != null)
        {
            float distanceTravelled = Vector3.Distance(transform.position, startPos);
            hudManager.UpdatePhysicalDistance(distanceTravelled);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("StartZone") && !isCounting)
        {
            startPos = transform.position; 
            isCounting = true; 
        }

        if (other.CompareTag("Zone") && currentState == PipeState.Flying)
        {
            currentState = PipeState.InZone;
        }

        if (other.CompareTag("LeftHand")) 
        {
            if (currentState == PipeState.Flying || currentState == PipeState.InZone)
            {
                currentState = PipeState.ReadyToCut; 
                GetComponent<Renderer>().material.color = Color.red;
                currentGrabTime = 0f; 

                if (grabSound != null) AudioSource.PlayClipAtPoint(grabSound, transform.position);
            }
        }

        if (other.CompareTag("Knife"))
        {
            if (currentState == PipeState.ReadyToCut)
            {
                if (hudManager != null)
                {
                    float finalDist = isCounting ? Vector3.Distance(transform.position, startPos) : 0f;
                    hudManager.EvaluateCut(finalDist);
                }

                if (cutSound != null) AudioSource.PlayClipAtPoint(cutSound, transform.position);

                // ====== 核心修改 2：不立刻销毁，启动右手震动结算协程 ======
                StartCoroutine(CutAndVibrateRightHand());
            }
        }
    }

    // ====== 核心修改 3：完美的切割反馈协程 ======
    private IEnumerator CutAndVibrateRightHand()
    {
        // 1. 关掉网格和碰撞体（视觉和物理上，铁管已经消失了，玩家觉得已经被切断）
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // 2. 停掉左手的摩擦震动，瞬间启动右手的最强震动！
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
        OVRInput.SetControllerVibration(rightCutVibrateIntensity, rightCutVibrateIntensity, OVRInput.Controller.RTouch);

        // 3. 让右手震动保持 0.15 秒（这是刀剑切割最干脆的震动时长）
        yield return new WaitForSeconds(0.15f);

        // 4. 彻底销毁这个铁管（依然会触发底部的 OnDestroy 确保安全关停）
        Destroy(gameObject);
    }

    // ====== 终极安全锁：不管铁管怎么死，死前必须关掉双手震动 ======
    private void OnDestroy()
    {
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
    }
}