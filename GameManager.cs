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

    void Start()
    {
        // 画面が起動したら自動的にリザルトの数字を表示する
        ShowResult();
    }

    // --- 画面切り替え用の関数（ボタンに設定する） ---
    public void LoadGameScene()
    {
        // データを一度リセットして、ゲーム画面（仮）へ切り替える
        PlayerPrefs.SetInt("LatestScore", 0);
        PlayerPrefs.SetInt("PerfectCount", 0);
        PlayerPrefs.SetInt("GreatCount", 0);
        PlayerPrefs.SetInt("MissCount", 0);
        SceneManager.LoadScene("GameScene"); // ※ゲーム画面のシーン名
    }

    public void LoadStartScene()
    {
        SceneManager.LoadScene("StartScene"); // ※スタート画面のシーン名
    }

    // ==========================================
    // ★吉岡さんのリザルト画面にデータを反映させる処理
    // ==========================================
    void ShowResult()
    {
        // 本番時はゲーム中から保存されたデータを読み込みます
        // （今はまだゲーム中を作っていないので、初期値として0が入ります）
        int score = PlayerPrefs.GetInt("LatestScore", 0);
        int perfects = PlayerPrefs.GetInt("PerfectCount", 0);
        int greats = PlayerPrefs.GetInt("GreatCount", 0);
        int misses = PlayerPrefs.GetInt("MissCount", 0);

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