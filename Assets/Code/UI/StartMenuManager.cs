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
}
