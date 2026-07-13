using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("把刚才做好的 TMP 文本拖到这里")]
    public TextMeshProUGUI timerText;

    [Header("在这里设置倒计时时间（秒），可在面板随时修改！")]
    public float timeLimit = 60f; // 默认60秒

    private float currentTime;
    private bool isTimerRunning = false;

    void Start()
    {
        // 游戏开始时，把当前时间设为你面板里填写的限时
        currentTime = timeLimit;
        
        // 自动开始倒计时，不需要可以注释掉
        StartTimer(); 
    }

    void Update()
    {
        if (isTimerRunning)
        {
            // 核心改变：从加时间变成了减时间
            currentTime -= Time.deltaTime; 

            // 防止时间变成负数
            if (currentTime <= 0)
            {
                currentTime = 0;
                isTimerRunning = false; // 停止计时
                Debug.Log("时间到！任务结束！");
                
                // ⚠️ 以后你可以把时间到了、打铁失败的代码写在这里
            }

            UpdateTimerDisplay();
        }
    }

    // 格式化时间并显示到屏幕上
    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60F);
        int seconds = Mathf.FloorToInt(currentTime - minutes * 60);
        
        // 如果倒计时不需要毫秒，把这一行和下面的 {2:00} 删掉即可
        int milliseconds = Mathf.FloorToInt((currentTime * 100F) % 100F); 

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    // 控制计时器的方法
    public void StartTimer() => isTimerRunning = true;
    public void StopTimer() => isTimerRunning = false;
    
    // 如果你想重新开始任务，调用这个方法就能恢复满时间
    public void ResetTimer()
    {
        currentTime = timeLimit;
        UpdateTimerDisplay();
    }
}