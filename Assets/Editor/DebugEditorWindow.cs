using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

enum DebugPage
{
    PlayerScene,
    Item,
    Enemy
}

public class DebugEditorWindow : EditorWindow
{
    DebugPage currentPage = DebugPage.PlayerScene;

    PlayerDamage player;

    int hpInput;
    int sceneIndex;
    Vector2 scrollPosition; 
    Dictionary<EquipmentData1, int> equipmentInputs = new Dictionary<EquipmentData1, int>(); // 装備の入力値を保持


    [MenuItem("Tools/Debug Window")]
    static void Open()
    {
        GetWindow<DebugEditorWindow>("Debug Tool");
    }

    void OnGUI()
    {
        DrawHeader();
        
        switch(currentPage)
        {
            case DebugPage.PlayerScene:
                DrawPlayerSection();
                GUILayout.Space(10);
                DrawSceneSection();
                GUILayout.Space(10);
                break;
            case DebugPage.Item:
                DrawItemHeldAmount();
                GUILayout.Space(10);
                break;
            case DebugPage.Enemy:
                DrawEnemySection();
                GUILayout.Space(10);
                break;

        }
        
    }

    void DrawHeader()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        if(GUILayout.Toggle(
            currentPage == DebugPage.PlayerScene,
            "Player / Scene",
            EditorStyles.toolbarButton))
        {
            currentPage = DebugPage.PlayerScene;
        }
        if(GUILayout.Toggle(
            currentPage == DebugPage.Item,
            "ItemAmount",
            EditorStyles.toolbarButton))
        {
            currentPage = DebugPage.Item;
        }
        if (GUILayout.Toggle(
            currentPage == DebugPage.Enemy,
            "Enemy",
            EditorStyles.toolbarButton))
        {
            currentPage = DebugPage.Enemy;
        }
        EditorGUILayout.EndHorizontal();
    }

    void OnInspectorUpdate()
    {
        //PlayMode中は再描画してリアルタイム更新
        if (EditorApplication.isPlaying)
        {
            Repaint();
        }
    }

    //Player設定
    void DrawPlayerSection()
    {
        GUILayout.Label("Player", EditorStyles.boldLabel);

        //PlayModeでないと警告を表示
        if (!EditorApplication.isPlaying)
        {
            EditorGUILayout.HelpBox("Play Modeで実行してください", MessageType.Info);
            return;
        }

        player = FindObjectOfType<PlayerDamage>();
        if (player == null)
        {
            EditorGUILayout.HelpBox("Playerがいない", MessageType.Warning);
            return;
        }

        //HP調整
        EditorGUILayout.LabelField("現在のHP", $"{player.HP} / {player.MAXHP}");
        hpInput = EditorGUILayout.IntField("HP設定", hpInput);
        if (GUILayout.Button("HP適用"))
        {
            Undo.RecordObject(player, "Change HP");
            player.SetHP(hpInput);
            EditorUtility.SetDirty(player);
        }
    }
    //SceneSelect
    void DrawSceneSection()
    {
        GUILayout.Label("Scene", EditorStyles.boldLabel);

        int count = SceneManager.sceneCountInBuildSettings;
        string[] sceneNames = new string[count];

        for (int i = 0; i < count; i++)
        {
            sceneNames[i] = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
        }
        sceneIndex = EditorGUILayout.Popup("Scene", sceneIndex, sceneNames);

        
        if(GUILayout.Button("シーン切り替え"))
        {

            if (EditorApplication.isPlaying)
            {
                SceneManager.LoadScene(sceneIndex);
            }
            else
            {
                EditorSceneManager.OpenScene(
                    SceneUtility.GetScenePathByBuildIndex(sceneIndex));
            }
                
        }
    }

    void DrawItemHeldAmount()
    {
        GUILayout.Label("装備インベントリ",EditorStyles.boldLabel);
        if (!EditorApplication.isPlaying)
        {
            EditorGUILayout.HelpBox("PlayModeで実行してください", MessageType.Info);
            return;
        }
        //InventoryManagerの取得
        var inventoryManager = InventoryManager.Instance;
        if(!inventoryManager)
        {
            EditorGUILayout.HelpBox("InventoryManagerが見つかりません", MessageType.Warning);
            return;
        }

        //装備データベースの取得
        var equipmentDatabase = FindObjectOfType<Soubikanri>()?.GetComponent<Soubikanri>();
        if(!equipmentDatabase)
        {
            EditorGUILayout.HelpBox("Soubikanriが見つかりません", MessageType.Warning);
            return;
        }

       
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(400));
        
        //Dictionaryのコピーを作成してループ（ループ中の変更を許可）
        var equipmentList = inventoryManager.EquipmentCounts.ToList();
        
        foreach (var kvp in equipmentList)
        {
            EquipmentData1 equipment = kvp.Key;
            int currentCount = inventoryManager.EquipmentCounts[equipment]; // 最新の値を取得

            // 入力値が未設定の場合は現在の所持数で初期化
            if (!equipmentInputs.ContainsKey(equipment))
            {
                equipmentInputs[equipment] = currentCount;
            }

            EditorGUILayout.BeginHorizontal();
            //アイテム名
            EditorGUILayout.LabelField(equipment.GetItemname(), GUILayout.Width(150));
            //現在の所持数
            EditorGUILayout.LabelField($"現在:{currentCount}", GUILayout.Width(80));
            //所持数入力フィールド（保持された値を使用）
            equipmentInputs[equipment] = EditorGUILayout.IntField(equipmentInputs[equipment], GUILayout.Width(60));

            if (GUILayout.Button("適用", GUILayout.Width(80)))
            {
                int newCount = equipmentInputs[equipment];
                if (newCount >= 0 && newCount <= equipment.GetItemlimit())
                {
                    inventoryManager.EquipmentCounts[equipment] = newCount;
                    var soubikanri = FindObjectOfType<Soubikanri>();
                    if (soubikanri != null)
                    {
                        soubikanri.UpdateUI();
                    }
                    Debug.Log($"{equipment.GetItemname()}の所持数を{newCount}に変更しました");
                }
                else
                {
                    Debug.LogWarning($"所持数は0～{equipment.GetItemlimit()}の範囲で設定してください");
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    void DrawEnemySection()
    {
        GUILayout.Label("Enemy", EditorStyles.boldLabel);
        if (!EditorApplication.isPlaying)
        {
            EditorGUILayout.HelpBox("PlayModeで実行してください", MessageType.Info);
            return;
        }
        var enemies = FindObjectsOfType<Enemystate1>();
        if (enemies.Length == 0)
        {
            EditorGUILayout.HelpBox("Enemyが見つかりません", MessageType.Warning);
            return;
        }
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        foreach (var enemy in enemies)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            // 敵の名前を表示
            EditorGUILayout.LabelField(enemy.name, GUILayout.Width(150));
            // 状態によって色を変える（Editor画面）
            Color stateColor = Color.white;
            switch (enemy.CurrentState)
            {
                case EnemyState.Idle: stateColor = Color.gray; break;
                case EnemyState.Chase: stateColor = Color.yellow; break;
                case EnemyState.Attack: stateColor = Color.red; break;
                case EnemyState.Cooldown: stateColor = Color.cyan; break;
            }
            // 色付きのラベル
            GUI.contentColor = stateColor;
            EditorGUILayout.LabelField($"[{enemy.CurrentState}]", EditorStyles.boldLabel, GUILayout.Width(100));
            GUI.contentColor = Color.white;
            // Selectボタン
            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                Selection.activeGameObject = enemy.gameObject;
                EditorGUIUtility.PingObject(enemy.gameObject);
            }
            // killボタン
            if (GUILayout.Button("Kill", GUILayout.Width(60)))
            {
                enemy.GetComponent<CharaDamage>()?.Damage(9999);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }
}