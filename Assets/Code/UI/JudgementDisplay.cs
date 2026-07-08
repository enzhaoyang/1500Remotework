using UnityEngine;
using TMPro;
using System.Collections;

public class JudgementDisplay : MonoBehaviour
{
    [Header("判定テキスト")]
    [SerializeField] private TextMeshProUGUI judgementText;

    [Header("表示時間（秒）")]
    [SerializeField] private float displayDuration = 1.5f;

    private void Start()
    {
        judgementText.text = "";
    }

    public void ShowPerfect()
    {
        Show("PERFECT!!!", new Color(1f, 0.84f, 0f)); // 金色
    }

    public void ShowGreat()
    {
        Show("GREAT!", new Color(0.91f, 0.38f, 0.04f)); // オレンジ
    }

    public void ShowMiss()
    {
        Show("MISS...", new Color(0.8f, 0.2f, 0.2f)); // 赤
    }

    private void Show(string message, Color color)
    {
        StopAllCoroutines();
        judgementText.text = message;
        judgementText.color = color;
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        judgementText.text = "";
    }
}
