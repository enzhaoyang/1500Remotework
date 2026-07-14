using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Events;
using TMPro;

/// <summary>
/// Leaderboardシーンに、ランキング表示用のUI（タイトル・順位テキスト×5・戻るボタン）を
/// 自動生成し、LeaderboardDisplayコンポーネントに紐付けるエディタ拡張。
/// WarningUISetupToolと同じ考え方：手動でCanvas右クリック→UI...を繰り返す代わりに、
/// メニューから一発でセットアップできるようにしたもの。
///
/// 使い方：Leaderboardシーンを開いた状態で
/// メニュー Tools > 1500Remotework > Setup Leaderboard UI を実行。
/// 何度実行しても安全（既存のCanvas/テキストがあれば再利用して紐付けのみ行う）。
/// </summary>
public static class LeaderboardSetupTool
{
    private const string CanvasObjectName = "Canvas";
    private const int EntryCount = 5;

    // 動作確認用：実際にGamePlayを最後までプレイしなくても、ランキングにダミースコアを
    // 1件追加できる（PlayモードでもEdit中でもOK。PlayerPrefsに直接書き込むため）。
    [MenuItem("Tools/1500Remotework/Debug/Add Test Score")]
    public static void AddTestScore()
    {
        int score = UnityEngine.Random.Range(100, 2000);
        LeaderboardManager.AddScore(score);
        Debug.Log($"<color=cyan>[LeaderboardSetupTool]</color> テストスコア {score} を追加しました。Leaderboardシーンを開き直す（またはPlayし直す）と反映されます。");
    }

    [MenuItem("Tools/1500Remotework/Setup Leaderboard UI")]
    public static void SetupLeaderboardUI()
    {
        GameObject canvasObj = GameObject.Find(CanvasObjectName);

        Undo.SetCurrentGroupName("Setup Leaderboard UI");
        int undoGroup = Undo.GetCurrentGroup();

        if (canvasObj == null)
        {
            canvasObj = CreateWorldSpaceCanvas();
        }
        else if (canvasObj.GetComponent<Canvas>() == null)
        {
            EditorUtility.DisplayDialog(
                "Leaderboard UI セットアップ",
                $"'{CanvasObjectName}' という名前のオブジェクトはありますが、Canvasコンポーネントがありません。",
                "OK");
            return;
        }

        CreateOrGetText(canvasObj.transform, "Title", "RANKING", new Vector2(0, 190), 44, FontStyles.Bold);

        TextMeshProUGUI[] rankTexts = new TextMeshProUGUI[EntryCount];
        for (int i = 0; i < EntryCount; i++)
        {
            float y = 100 - i * 45;
            rankTexts[i] = CreateOrGetText(canvasObj.transform, $"Rank{i + 1}Text", $"{i + 1}位　　-----", new Vector2(0, y), 28, FontStyles.Normal);
        }

        Button backButton = CreateOrGetButton(canvasObj.transform, "BackButton", "戻る", new Vector2(0, -150));

        LeaderboardDisplay display = canvasObj.GetComponent<LeaderboardDisplay>();
        if (display == null)
        {
            display = Undo.AddComponent<LeaderboardDisplay>(canvasObj);
        }

        SerializedObject so = new SerializedObject(display);
        SerializedProperty rankTextsProp = so.FindProperty("rankTexts");
        rankTextsProp.arraySize = EntryCount;
        for (int i = 0; i < EntryCount; i++)
        {
            rankTextsProp.GetArrayElementAtIndex(i).objectReferenceValue = rankTexts[i];
        }
        so.ApplyModifiedProperties();

        // BackButtonのOnClickにLeaderboardDisplay.BackToMenuを接続（重複登録は避ける）
        bool alreadyWired = false;
        for (int i = 0; i < backButton.onClick.GetPersistentEventCount(); i++)
        {
            if (backButton.onClick.GetPersistentMethodName(i) == nameof(LeaderboardDisplay.BackToMenu))
            {
                alreadyWired = true;
                break;
            }
        }
        if (!alreadyWired)
        {
            UnityEventTools.AddPersistentListener(backButton.onClick, display.BackToMenu);
        }

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkSceneDirty(canvasObj.scene);
        Selection.activeGameObject = canvasObj;

        Debug.Log("<color=cyan>[LeaderboardSetupTool]</color> ランキング表示UIを作成し、LeaderboardDisplayに紐付けました。");
    }

    private static GameObject CreateWorldSpaceCanvas()
    {
        GameObject canvasObj = new GameObject(CanvasObjectName, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Undo.RegisterCreatedObjectUndo(canvasObj, "Create " + CanvasObjectName);

        Canvas canvas = canvasObj.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        RectTransform rt = canvasObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(600, 550);
        rt.localScale = Vector3.one * 0.001f; // startシーンのCanvasと同じスケール感
        rt.position = new Vector3(0, 0, 2);

        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            // このプロジェクトはActive Input HandlerがInput System Package (New)のみのため、
            // 旧StandaloneInputModuleを使うとInvalidOperationExceptionが出る。新方式を使う。
            GameObject es = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule));
            Undo.RegisterCreatedObjectUndo(es, "Create EventSystem");
        }

        Debug.Log("<color=cyan>[LeaderboardSetupTool]</color> 'Canvas'が見つからなかったため、World Space Canvasを新規作成しました。");
        return canvasObj;
    }

    private static TextMeshProUGUI CreateOrGetText(Transform parent, string objectName, string label, Vector2 anchoredPos, int fontSize, FontStyles style)
    {
        Transform existing = parent.Find(objectName);
        GameObject go;

        if (existing != null)
        {
            go = existing.gameObject;
        }
        else
        {
            go = new GameObject(objectName, typeof(RectTransform));
            Undo.RegisterCreatedObjectUndo(go, "Create " + objectName);
            Undo.SetTransformParent(go.transform, parent, "Parent " + objectName);
        }

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(500, 50);
        rt.anchoredPosition = anchoredPos;
        rt.localScale = Vector3.one;

        TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
        if (tmp == null)
        {
            tmp = Undo.AddComponent<TextMeshProUGUI>(go);
        }
        tmp.text = label;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        return tmp;
    }

    private static Button CreateOrGetButton(Transform parent, string objectName, string label, Vector2 anchoredPos)
    {
        Transform existing = parent.Find(objectName);
        GameObject go;

        if (existing != null)
        {
            go = existing.gameObject;
        }
        else
        {
            go = new GameObject(objectName, typeof(RectTransform), typeof(Image), typeof(Button));
            Undo.RegisterCreatedObjectUndo(go, "Create " + objectName);
            Undo.SetTransformParent(go.transform, parent, "Parent " + objectName);
        }

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(200, 60);
        rt.anchoredPosition = anchoredPos;
        rt.localScale = Vector3.one;

        Image img = go.GetComponent<Image>();
        img.color = new Color(0.9f, 0.4f, 0.05f); // MainMenu/startのオレンジアクセントに合わせる

        Transform textTransform = go.transform.Find("Text");
        TextMeshProUGUI tmp;
        if (textTransform == null)
        {
            GameObject textGo = new GameObject("Text", typeof(RectTransform));
            Undo.RegisterCreatedObjectUndo(textGo, "Create Button Text");
            Undo.SetTransformParent(textGo.transform, go.transform, "Parent Button Text");

            RectTransform textRt = textGo.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.sizeDelta = Vector2.zero;
            textRt.anchoredPosition = Vector2.zero;
            textRt.localScale = Vector3.one;

            tmp = Undo.AddComponent<TextMeshProUGUI>(textGo);
        }
        else
        {
            tmp = textTransform.GetComponent<TextMeshProUGUI>();
        }
        tmp.text = label;
        tmp.fontSize = 28;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        return go.GetComponent<Button>();
    }
}
