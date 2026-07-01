using UnityEngine;
using TMPro;
using System.Collections;

public class WarningDisplay : MonoBehaviour
{
    [Header("WARNING テキスト")]
    [SerializeField] private TextMeshProUGUI frontWarning;  // 正面
    [SerializeField] private TextMeshProUGUI leftWarning;   // 左
    [SerializeField] private TextMeshProUGUI rightWarning;  // 右

    [Header("表示時間（秒）")]
    [SerializeField] private float displayDuration = 2f;

    private void Start()
    {
        HideAll();
    }

    [ContextMenu("Test: Show Front")]
    public void ShowFront()  { Show(frontWarning); }

    [ContextMenu("Test: Show Left")]
    public void ShowLeft()   { Show(leftWarning); }

    [ContextMenu("Test: Show Right")]
    public void ShowRight()  { Show(rightWarning); }

    private void Show(TextMeshProUGUI target)
    {
        StopAllCoroutines();
        HideAll();
        target.gameObject.SetActive(true);
        StartCoroutine(HideAfterDelay(target));
    }

    private IEnumerator HideAfterDelay(TextMeshProUGUI target)
    {
        yield return new WaitForSeconds(displayDuration);
        target.gameObject.SetActive(false);
    }

    private void HideAll()
    {
        if (frontWarning) frontWarning.gameObject.SetActive(false);
        if (leftWarning)  leftWarning.gameObject.SetActive(false);
        if (rightWarning) rightWarning.gameObject.SetActive(false);
    }
}
