using System.Collections;
using UnityEngine;

public class NewPipeSpawner : MonoBehaviour
{
    [Header("生成设置")]
    [Tooltip("把你做好的铁管Prefab拖到这里")]
    public GameObject pipePrefab; 
    
    [Tooltip("每隔几秒生成一次铁管")]
    public float spawnInterval = 2.0f; 

    [Header("随机范围 (米)")]
    [Tooltip("左右随机生成的最大距离")]
    public float randomXRange = 1.5f; 
    
    [Tooltip("上下随机生成的最大距离")]
    public float randomYRange = 0.5f; 

    void Start()
    {
        // 游戏一开始，启动持续生成铁管的“协程”
        StartCoroutine(SpawnPipeRoutine());
    }

    IEnumerator SpawnPipeRoutine()
    {
        // 写一个死循环，只要游戏在运行，就一直刷
        while (true)
        {
            SpawnSinglePipe();
            // 停顿设定的秒数后，再执行下一次循环
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnSinglePipe()
    {
        // 如果你忘了拖入Prefab，这里会拦截报错，防止游戏崩溃
        if (pipePrefab == null)
        {
            
            Debug.LogWarning("生成器找不到铁管Prefab！请在Inspector里赋值。");
            return;
        }

        // 1. 计算随机偏移量
        float randomX = Random.Range(-randomXRange, randomXRange);
        float randomY = Random.Range(-randomYRange, randomYRange);

        // 2. 结合Spawner本身的位置，计算出最终的生成坐标
        Vector3 spawnPosition = transform.position + new Vector3(randomX, randomY, 0);

        // 3. 实例化铁管！(使用Spawner自身的朝向，确保铁管飞向玩家)
        Instantiate(pipePrefab, spawnPosition, transform.rotation);
    }
}