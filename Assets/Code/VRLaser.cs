using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class VRLaser : MonoBehaviour
{
    [Header("射线长度 (米)")]
    public float laserLength = 2.0f; 

    private LineRenderer line;

    void Start()
    {
        // 自动获取并设置 LineRenderer
        line = GetComponent<LineRenderer>();
        
        // 把激光调得非常细，更有科技感
        line.startWidth = 0.002f; 
        line.endWidth = 0.002f;
        
        // 自动分配一个不需要打光就能亮的无光照材质
        line.material = new Material(Shader.Find("Sprites/Default"));
        
        // 设置激光颜色：起点是青色，终点也是青色（你可以改成 Color.red 或其他颜色）
        line.startColor = Color.cyan; 
        line.endColor = Color.cyan;
    }

    void Update()
    {
        // 让激光的起点死死锁定在手柄位置，终点指向手柄的正前方
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position + transform.forward * laserLength);
    }
}