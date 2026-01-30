using UnityEditor;
using UnityEngine;

public class MemoWindow : EditorWindow
{
    // メモ欄のタイトル
    private string[] memoTitles = { "開発(アセット)メモ", "スクリプトメモ", "アイデアメモ"};

    // 各メモの内容
    private string[] memoTexts;

    // 保存キー
    private const string MemoKeyPrefix = "DevMemo_Text_";

    // スクロール位置
    private Vector2 scrollPos;

    [MenuItem("Tools/Developer Memo")]
    public static void Open()
    {
        GetWindow<MemoWindow>("Developer Memo");
    }

    private void OnEnable()
    {
        memoTexts = new string[memoTitles.Length];

        // 各メモの保存内容を読み込み
        for (int i = 0; i < memoTitles.Length; i++)
        {
            memoTexts[i] = EditorPrefs.GetString(MemoKeyPrefix + i, "");
        }
    }

    private void OnGUI()
    {
        GUILayout.Label(" 開発用メモ帳", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("複数のカテゴリに分けてメモできます。Unityを再起動しても保持されます。", MessageType.Info);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        // 各メモ欄を順に表示
        for (int i = 0; i < memoTitles.Length; i++)
        {
            EditorGUILayout.Space(20);
            GUILayout.Label(memoTitles[i], EditorStyles.boldLabel);

            memoTexts[i] = EditorGUILayout.TextArea(memoTexts[i], GUILayout.Height(100));

            if (GUILayout.Button(" " + memoTitles[i] + " を保存"))
            {
                EditorPrefs.SetString(MemoKeyPrefix + i, memoTexts[i]);
                Debug.Log($"[{memoTitles[i]}] を保存しました。");
            }

            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("全メモを保存"))
        {
            for (int i = 0; i < memoTitles.Length; i++)
            {
                EditorPrefs.SetString(MemoKeyPrefix + i, memoTexts[i]);
            }
            Debug.Log("全メモを保存しました。");
        }
    }
}
