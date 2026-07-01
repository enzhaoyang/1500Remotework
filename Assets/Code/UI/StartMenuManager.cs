using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    [Header("シーン名 (Build Settings に登録すること)")]
    [SerializeField] private string gamePlaySceneName    = "GamePlay";
    [SerializeField] private string tutorialSceneName    = "Tutorial";
    [SerializeField] private string leaderboardSceneName = "Leaderboard";

    public void StartGame()
    {
        SceneManager.LoadScene(gamePlaySceneName);
    }

    public void OpenTutorial()
    {
        SceneManager.LoadScene(tutorialSceneName);
    }

    public void OpenLeaderboard()
    {
        SceneManager.LoadScene(leaderboardSceneName);
    }

    [Header("スタート画面へ戻る用（チームはstartシーンを使用）")]
    [SerializeField] private string mainMenuSceneName = "start";

    public void OpenMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
