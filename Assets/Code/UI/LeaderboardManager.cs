using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// スコアをPlayerPrefsに永続保存し、上位ランキングを提供する
// GameHUDManager.SaveFinalResult()から呼ばれる想定
public static class LeaderboardManager
{
    private const string KeyPrefix = "Leaderboard_Score_";
    private const int MaxEntries = 10;

    // ゲーム終了時に新しいスコアを追加し、上位MaxEntries件だけ保持する
    public static void AddScore(int score)
    {
        List<int> scores = GetAllScores();
        scores.Add(score);
        scores.Sort((a, b) => b.CompareTo(a)); // 降順（高い順）

        if (scores.Count > MaxEntries)
        {
            scores.RemoveRange(MaxEntries, scores.Count - MaxEntries);
        }

        SaveScores(scores);
    }

    // 上位count件を取得（デフォルト5件）
    public static List<int> GetTopScores(int count = 5)
    {
        return GetAllScores().Take(count).ToList();
    }

    private static List<int> GetAllScores()
    {
        List<int> scores = new List<int>();
        for (int i = 0; i < MaxEntries; i++)
        {
            string key = KeyPrefix + i;
            if (PlayerPrefs.HasKey(key))
            {
                scores.Add(PlayerPrefs.GetInt(key));
            }
        }
        return scores;
    }

    private static void SaveScores(List<int> scores)
    {
        for (int i = 0; i < MaxEntries; i++)
        {
            string key = KeyPrefix + i;
            if (i < scores.Count)
            {
                PlayerPrefs.SetInt(key, scores[i]);
            }
            else if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
            }
        }
        PlayerPrefs.Save();
    }
}
