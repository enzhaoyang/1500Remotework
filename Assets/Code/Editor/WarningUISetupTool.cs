using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

/// <summary>
/// Canvas配下に FrontWarning / LeftWarning / RightWarning の
/// TextMeshProUGUIを自動生成し、WarningDisplayコンポーネントに紐付けるエディタ拡張。
/// 手動でCanvas右クリック→UI→TextMeshPro×3...を繰り返す代わりに、
/// メニューから一発でセットアップできるようにしたもの。
///
/// 使い方：シーンを開いた状態で
/// メニュー Tools > 1500Remotework > Setup Warning UI を実行。
/// - 'Canvas'という名前のオブジェクトが既にあればそこに追加
/// - 無ければ新規にWorld Space Canvasを自動生成（隔離した検証用シーンでも1発で完結する）
/// 既にFrontWarning等が存在する場合は作り直さず、参照の再紐付けのみ行う（何度実行しても安全）。
/// </summary>
public static class WarningUISetupTool
{
    private const string CanvasObjectName = "Canvas";

    [MenuItem("Tools/1500Remotework/Setup Warning UI")]
    public static void SetupWarningUI()
    {
        GameObject canvasObj = GameObject.Find(CanvasObjectName);

        Undo.SetCurrentGroupName("Setup Warning UI");
        int undoGroup = Undo.GetCurrentGroup();

        if (canvasObj == null)
        {
            canvasObj = CreateWorldSpaceCanvas();
        }
        else if (canvasObj.GetComponent<Canvas>() == null)
        {
            EditorUtility.DisplayDialog(
                "Warning UI セットアップ",
                $"'{CanvasObjectName}' という名前のオブジェクトはありますが、Canvasコンポーネントがありません。",
                "OK");
            return;
        }

        TextMeshProUGUI front = CreateOrGetWarningText(canvasObj.transform, "FrontWarning", "WARNING: FRONT", new Vector2(0, 110));
        TextMeshProUGUI left  = CreateOrGetWarningText(canvasObj.transform, "LeftWarning",  "WARNING: LEFT",  new Vector2(-160, -20));
        TextMeshProUGUI right = CreateOrGetWarningText(canvasObj.transform, "RightWarning", "WARNING: RIGHT", new Vector2(160, -20));

        WarningDisplay warningDisplay = canvasObj.GetComponent<WarningDisplay>();
        if (warningDisplay == null)
        {
            warningDisplay = Undo.AddComponent<WarningDisplay>(canvasObj);
        }

        SerializedObject so = new SerializedObject(warningDisplay);
        so.FindProperty("frontWarning").objectReferenceValue = front;
        so.FindProperty("leftWarning").objectReferenceValue = left;
        so.FindProperty("rightWarning").objectReferenceValue = right;
        so.ApplyModifiedProperties();

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkSceneDirty(canvasObj.scene);
        Selection.activeGameObject = canvasObj;

        Debug.Log("<color=cyan>[WarningUISetupTool]</color> FrontWarning / LeftWarning / RightWarning を作成し、WarningDisplayに紐付けました。");
    }

    private static GameObject CreateWorldSpaceCanvas()
    {
        GameObject canvasObj = new GameObject(CanvasObjectName, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Undo.RegisterCreatedObjectUndo(canvasObj, "Create " + CanvasObjectName);

        Canvas canvas = canvasObj.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        RectTransform rt = canvasObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(457, 334); // GamePlayのHUD Canvasと同じサイズ感
        rt.localScale = Vector3.one * 0.01f;  // World SpaceのCanvasはピクセル単位が大きいので縮小
        rt.position = new Vector3(0, 1.5f, 2f);

        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            // Active Input HandlerがInput System Package (New)のみの設定のため、
            // 旧StandaloneInputModuleではなく新方式を使う。
            GameObject es = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule));
            Undo.RegisterCreatedObjectUndo(es, "Create EventSystem");
        }

        Debug.Log("<color=cyan>[WarningUISetupTool]</color> 'Canvas'が見つからなかったため、World Space Canvasを新規作成しました。");
        return canvasObj;
    }

    private static TextMeshProUGUI CreateOrGetWarningText(Transform parent, string objectName, string label, Vector2 anchoredPos)
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
        rt.sizeDelta = new Vector2(320, 60);
        rt.anchoredPosition = anchoredPos;
        rt.localScale = Vector3.one;

        TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
        if (tmp == null)
        {
            tmp = Undo.AddComponent<TextMeshProUGUI>(go);
        }
        tmp.text = label;
        tmp.fontSize = 36;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(1f, 0.15f, 0.15f);

        go.SetActive(false); // WarningDisplay.Start()のHideAllと合わせ、初期状態は非表示

        return tmp;
    }
}
