using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    [Header("生成设置")]
    [Tooltip("拖入我们刚才做好的铁管 Prefab")]
    public GameObject pipePrefab; 
    
    [Tooltip("每隔多少秒生成一根（类似音乐的节拍间隔）")]
    public float spawnInterval = 2.0f; 

    private float timer = 0f;

    void Update()
    {
        // 就像打拍子一样，计时器不断增加
        timer += Time.deltaTime;
        
        // 当时间达到了设定的节拍间隔
        if (timer >= spawnInterval)
        {
            SpawnPipe();
            timer = 0f; // 重置计时器，准备下一个节拍
        }
    }

    void SpawnPipe()
    {
        // 在这个生成器所在的位置，按原来的角度，凭空复制出一个新的铁管
        Instantiate(pipePrefab, transform.position, transform.rotation);
    }
}