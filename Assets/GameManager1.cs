using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // 文字を表示するための機能
using UnityEngine.SceneManagement; // 画面切り替えのための機能

public class GameManager : MonoBehaviour
{
    // ==========================================
    // 【リザルト画面】のパーツ（吉岡さんが図面で作った枠！）
    // ==========================================
    [Header("=== Result UI ===")]
    public TextMeshProUGUI resultScoreText;   // SCORE用のテキスト
    public TextMeshProUGUI perfectCountText; // PERFECT用のテキスト
    public TextMeshProUGUI greatCountText;   // GREAT用のテキスト
    public TextMeshProUGUI missCountText;    // MISS用のテキスト
    public TextMeshProUGUI rankText;         // 右上のデカいS,A,Bランク用テキスト

    [Header("=== シーン名 (Build Settings に登録すること) ===")]
    [SerializeField] private string gamePlaySceneName = "GamePlay";
    [SerializeField] private string mainMenuSceneName = "start";

    void Start()
    {
        // 画面が起動したら自動的にリザルトの数字を表示する
        ShowResult();
    }

    // --- 画面切り替え用の関数（ボタンに設定する） ---
    public void LoadGameScene()
    {
        SceneManager.LoadScene(gamePlaySceneName);
    }

    public void LoadStartScene()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // ==========================================
    // ★リザルト画面にデータを反映させる処理
    // GameHUDManager(GamePlayシーン)のstatic変数からスコア・判定回数を読み込む
    // ==========================================
    void ShowResult()
    {
        int score = GameHUDManager.FinalScore;
        int perfects = GameHUDManager.FinalPerfectCount;
        int greats = GameHUDManager.FinalGreatCount;
        int misses = GameHUDManager.FinalMissCount;

        // ※テスト用：もしUnityの再生ボタンを押してすぐに数字が変わるか確認したい場合は、
        // 下の「//」を消して、好きな数字を入れてテストしてみてください。
        // score = 1600;
        // perfects = 15;
        // greats = 5;
        // misses = 2;

        // 吉岡さんが配置した文字パーツ（Text）に数字を書き込む
        resultScoreText.text = "SCORE : " + score.ToString();
        perfectCountText.text = $"PERFECT  {perfects} ";
        greatCountText.text = $"GREAT    {greats} ";
        missCountText.text = $"MISS     {misses} ";

        // スコアに応じて、右上のランク（S, A, B）を自動で計算して切り替える
        if (score >= 1500) {
            rankText.text = "S";
            rankText.color = Color.yellow; // Sランクは金色
        } else if (score >= 800) {
            rankText.text = "A";
            rankText.color = Color.red; // Aランクは赤色
        } else {
            rankText.text = "B";
            rankText.color = Color.white; // Bランクは白色
        }
    }
}