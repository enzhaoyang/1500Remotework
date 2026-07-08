using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Events;
using TMPro;

/// <summary>
/// Tutorialシーンに、MainMenuと同じ配色（暗い背景＋オレンジアクセント）の
/// 操作説明UIを自動生成するエディタ拡張。
///
/// 使い方：Tutorialシーンを開いた状態で
/// メニュー Tools > 1500Remotework > Setup Tutorial UI を実行。
/// 既にCanvas等が存在する場合は作り直さず、参照の再紐付けのみ行う（何度実行しても安全）。
/// </summary>
public static class TutorialUISetupTool
{
    private static readonly Color BackgroundColor = new Color(0.0666667f, 0.0666667f, 0.0666667f, 1f);
    private static readonly Color PanelColor = new Color(0.1333333f, 0.1333333f, 0.1333333f, 1f);
    private static readonly Color AccentColor = new Color(0.7529412f, 0.2549020f, 0.0392157f, 1f);
    private const string CanvasObjectName = "Canvas";

    [MenuItem("Tools/1500Remotework/Setup Tutorial UI")]
    public static void SetupTutorialUI()
    {
        Undo.SetCurrentGroupName("Setup Tutorial UI");
        int undoGroup = Undo.GetCurrentGroup();

        GameObject canvasObj = GameObject.Find(CanvasObjectName);
        if (canvasObj == null)
        {
            canvasObj = new GameObject(CanvasObjectName, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Undo.RegisterCreatedObjectUndo(canvasObj, "Create Canvas");
            Canvas canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            RectTransform canvasRt = canvasObj.GetComponent<RectTransform>();
            canvasRt.sizeDelta = new Vector2(800, 600);
            canvasRt.localScale = Vector3.one * 0.01f;
            canvasRt.position = new Vector3(0, 1.5f, 2f);

            if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject es = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
                Undo.RegisterCreatedObjectUndo(es, "Create EventSystem");
            }
        }

        // 背景パネル
        CreatePanel(canvasObj.transform, "Background", BackgroundColor, Vector2.zero, new Vector2(800, 600));

        // タイトル
        CreateText(canvasObj.transform, "TitleText", "あそびかた", 48, Color.white, new Vector2(0, 230), new Vector2(600, 80), FontStyles.Bold);

        // 説明本文パネル
        CreatePanel(canvasObj.transform, "BodyPanel", PanelColor, new Vector2(0, 30), new Vector2(680, 320));

        string body =
            "1. 片手（グリッパー）でノズルから伸びてきた\n" +
            "     赤熱した鉄をつかむ\n\n" +
            "2. タイミングを見て、もう片方（ナイフ）で切る\n\n" +
            "3. 方向表示（WARNING）に注意して、\n" +
            "     全方向からの鉄に対応する\n\n" +
            "タイミングが合うほど高スコア！";
        CreateText(canvasObj.transform, "BodyText", body, 26, Color.white, new Vector2(0, 30), new Vector2(640, 300), FontStyles.Normal, TextAlignmentOptions.TopLeft);

        // ボタン
        Button startButton = CreateButton(canvasObj.transform, "StartButton", "スタート", new Vector2(-160, -230));
        Button backButton = CreateButton(canvasObj.transform, "BackButton", "もどる", new Vector2(160, -230));

        // StartMenuManagerをCanvasに付与してボタンと紐付け
        StartMenuManager manager = canvasObj.GetComponent<StartMenuManager>();
        if (manager == null)
        {
            manager = Undo.AddComponent<StartMenuManager>(canvasObj);
        }

        BindButtonClick(startButton, manager, nameof(StartMenuManager.StartGame));
        BindButtonClick(backButton, manager, nameof(StartMenuManager.OpenMainMenu));

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkSceneDirty(canvasObj.scene);
        Selection.activeGameObject = canvasObj;

        Debug.Log("<color=cyan>[TutorialUISetupTool]</color> Tutorial UIのセットアップが完了しました。");
    }

    private static void BindButtonClick(Button button, Object target, string methodName)
    {
        // 既存の登録があれば重複させないようクリア
        while (button.onClick.GetPersistentEventCount() > 0)
        {
            UnityEventTools.RemovePersistentListener(button.onClick, 0);
        }
        var action = System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), target, methodName) as UnityEngine.Events.UnityAction;
        UnityEventTools.AddPersistentListener(button.onClick, action);
    }

    private static GameObject CreatePanel(Transform parent, string name, Color color, Vector2 anchoredPos, Vector2 size)
    {
        Transform existing = parent.Find(name);
        GameObject go = existing != null ? existing.gameObject : new GameObject(name, typeof(RectTransform));
        if (existing == null)
        {
            Undo.RegisterCreatedObjectUndo(go, "Create " + name);
            go.transform.SetParent(parent, false);
        }

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.anchoredPosition = anchoredPos;
        rt.localScale = Vector3.one;

        Image img = go.GetComponent<Image>();
        if (img == null) img = Undo.AddComponent<Image>(go);
        img.color = color;

        return go;
    }

    private static TextMeshProUGUI CreateText(Transform parent, string name, string text, int fontSize, Color color, Vector2 anchoredPos, Vector2 size, FontStyles style = FontStyles.Normal, TextAlignmentOptions align = TextAlignmentOptions.Center)
    {
        Transform existing = parent.Find(name);
        GameObject go = existing != null ? existing.gameObject : new GameObject(name, typeof(RectTransform));
        if (existing == null)
        {
            Undo.RegisterCreatedObjectUndo(go, "Create " + name);
            go.transform.SetParent(parent, false);
        }

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.anchoredPosition = anchoredPos;
        rt.localScale = Vector3.one;

        TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
        if (tmp == null) tmp = Undo.AddComponent<TextMeshProUGUI>(go);
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.fontStyle = style;
        tmp.alignment = align;

        return tmp;
    }

    private static Button CreateButton(Transform parent, string name, string label, Vector2 anchoredPos)
    {
        Transform existing = parent.Find(name);
        GameObject go = existing != null ? existing.gameObject : new GameObject(name, typeof(RectTransform));
        if (existing == null)
        {
            Undo.RegisterCreatedObjectUndo(go, "Create " + name);
            go.transform.SetParent(parent, false);
        }

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(260, 70);
        rt.anchoredPosition = anchoredPos;
        rt.localScale = Vector3.one;

        Image img = go.GetComponent<Image>();
        if (img == null) img = Undo.AddComponent<Image>(go);
        img.color = AccentColor;

        Button btn = go.GetComponent<Button>();
        if (btn == null) btn = Undo.AddComponent<Button>(go);

        Transform textChild = go.transform.Find("Text");
        GameObject textGo = textChild != null ? textChild.gameObject : new GameObject("Text", typeof(RectTransform));
        if (textChild == null)
        {
            Undo.RegisterCreatedObjectUndo(textGo, "Create Button Text");
            textGo.transform.SetParent(go.transform, false);
        }
        RectTransform textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;
        textRt.anchoredPosition = Vector2.zero;

        TextMeshProUGUI tmp = textGo.GetComponent<TextMeshProUGUI>();
        if (tmp == null) tmp = Undo.AddComponent<TextMeshProUGUI>(textGo);
        tmp.text = label;
        tmp.fontSize = 28;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;

        return btn;
    }
}
