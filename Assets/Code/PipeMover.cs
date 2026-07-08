using UnityEngine;

public class PipeMover : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("铁管向玩家飞行的速度")]
    public float moveSpeed = 3.0f;

    void Update()
    {
        // 让铁管每一帧都沿着 Z 轴的反方向（也就是朝着玩家的面朝方向）移动
        transform.Translate(-1 * Vector3.back * moveSpeed * Time.deltaTime, Space.World);
    }
}