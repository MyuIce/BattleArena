using UnityEngine;
using UnityEditor;
using System.IO;

public static class EquipmentCsvImport
{
    private const string CsvPath =
        "Assets/APP/Data/BattleArenaData.csv";
    private const string EquipmentRootFolder =
        "Assets/APP/Script/04_ScriptableObject/Itemdata/EquipmentData/";

    [MenuItem("Tools/Import Equipment CSV")]
    public static void Import()
    {
        if (!File.Exists(CsvPath))
        {
            Debug.Log("CSV not found");
            return;
        }
        string[] lines = File.ReadAllLines(CsvPath);

        int updateCount = 0;
        int errorCount = 0;

        //lines[0]はヘッダー行
        for(int i=1;i<lines.Length;i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            string[] cols = lines[i].Split(',');

            try
            {
                int id = int.Parse(cols[0]);
                string type = cols[2];
                
                string assetPath = $"{EquipmentRootFolder}{type}Data/{id}_{type}.asset";

                EquipmentData1 data =
                    AssetDatabase.LoadAssetAtPath<EquipmentData1>(assetPath);

                if(data == null)
                {
                    Debug.LogError(
                        $"対応するSOが存在しません ID:{id} Type:{type}"
                    );
                    errorCount++;
                    continue;
                }

                //上書き処理
                data.GetType()
                    .GetField("Equipmentname",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(data, cols[1]);

                data.GetType()
                    .GetField("Equipmentexplanation",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(data, cols[9]);

                data.GetType()
                    .GetField("Itemlimit",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(data, int.Parse(cols[8]));

                var status = data.GetItemStatus();
                status.ATK = int.Parse(cols[3]);
                status.DEF = int.Parse(cols[4]);
                status.AGI = int.Parse(cols[5]);
                status.INT = int.Parse(cols[6]);
                status.RES = int.Parse(cols[7]);

                EditorUtility.SetDirty(data);
                updateCount++;
            }
            catch
            {
                Debug.LogError($"CSV失敗 行:{i + 1}");
                errorCount++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Import完了");
    }
}
