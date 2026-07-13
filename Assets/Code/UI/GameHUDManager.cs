using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameHUDManager : MonoBehaviour
{
    [Header("HUD テキスト")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;

    [Header("シーン名 (Build Settings に登録すること)")]
    [SerializeField] private string resultSceneName = "Result";
    [SerializeField] private string quitSceneName = "start";

    // シーンをまたいでリザルト画面から参照できるStatic変数
    public static int FinalScore = 0;
    public static int FinalPerfectCount = 0;
    public static int FinalGreatCount = 0;
    public static int FinalMissCount = 0;

    private int score = 0;
    private int combo = 0;
    private int perfectCount = 0;
    private int greatCount = 0;
    private int missCount = 0;
    private float timeRemaining = 60f;
    private bool isGameRunning = true;

    void Update()
    {
        // プレイ中に途中で抜けたい場合の暫定対応（VRコントローラーのメニューボタン対応は別途）
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(quitSceneName);
            return;
        }

        if (!isGameRunning) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isGameRunning = false;
            SaveFinalResult(); // タイムアップ時に結果を保存
            SceneManager.LoadScene(resultSceneName); // Resultシーンへ遷移
        }

        UpdateHUD();
    }

    private void SaveFinalResult()
    {
        FinalScore = score;
        FinalPerfectCount = perfectCount;
        FinalGreatCount = greatCount;
        FinalMissCount = missCount;
        LeaderboardManager.AddScore(score); // ランキングに今回のスコアを保存
    }

    private void UpdateHUD()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timeText.text  = $"TIME: {minutes:00}:{seconds:00}";
        scoreText.text = $"SCORE: {score}";
        comboText.text = $"COMBO: {combo}";
    }

    // CutDetector等から呼び出す
    public void AddScore(int points)
    {
        score += points;
        combo++;
    }

    public void ResetCombo()
    {
        combo = 0;
    }

    // CutDetectorのonPerfect/onGreat/onMissイベントからInspectorで接続する
    // (CutDetector.cs自体は変更しない。判定ロジックはenzhaoyangさんの担当)
    public void RecordPerfect()
    {
        perfectCount++;
    }

    public void RecordGreat()
    {
        greatCount++;
    }

    public void RecordMiss()
    {
        missCount++;
        ResetCombo();
    }

    private void OnDestroy()
    {
        // シーン遷移など、タイムアップ以外の経路でHUDが破棄される場合の保険
        if (isGameRunning)
        {
            SaveFinalResult();
        }
    }
}
