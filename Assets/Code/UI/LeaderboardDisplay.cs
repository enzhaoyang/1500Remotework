using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

// Leaderboardシーンで、保存済みの上位スコアを表示する
public class LeaderboardDisplay : MonoBehaviour
{
    [Header("順位テキスト（上から1位, 2位...の順で登録）")]
    [SerializeField] private TextMeshProUGUI[] rankTexts;

    [Header("戻り先シーン名 (Build Settings に登録すること)")]
    [SerializeField] private string mainMenuSceneName = "start";

    void Start()
    {
        ShowRanking();
    }

    private void ShowRanking()
    {
        var topScores = LeaderboardManager.GetTopScores(rankTexts.Length);

        for (int i = 0; i < rankTexts.Length; i++)
        {
            if (i < topScores.Count)
            {
                rankTexts[i].text = $"{i + 1}位　　{topScores[i]}";
            }
            else
            {
                rankTexts[i].text = $"{i + 1}位　　-----";
            }
        }
    }

    // 戻るボタンに設定する
    public void BackToMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
