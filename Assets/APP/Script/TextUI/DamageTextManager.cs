using UnityEngine;

/// <summary>
/// ダメージテキストの生成と管理を行うマネージャー
/// ジャストパリィテキストの生成と管理
/// シングルトンを使わず、直接参照で管理
/// </summary>
public class DamageTextManager : MonoBehaviour
{
    [Header("Prefab設定")]
    [SerializeField] private GameObject damageTextPrefab; 
    [SerializeField] private GameObject justParryTextPrefab;

    [Header("Canvas設定")]
    [SerializeField] private Canvas damageCanvas; // ダメージテキスト表示用Canvas

    void Awake()
    {
        // Canvasの自動検索(設定されていない場合)
        if (damageCanvas == null)
        {
            damageCanvas = GetComponentInChildren<Canvas>();
            if (damageCanvas == null)
            {
                Debug.LogError("[DamageTextManager] Canvasが見つかりません!");
            }
        }
    }

    /// <summary>
    /// ダメージテキストを生成して表示
    /// </summary>
    /// <param name="damage">表示するダメージ値</param>
    /// <param name="worldPosition">ワールド座標での表示位置</param>
    
    public void ShowDamage(int damage, Vector3 worldPosition)
    {
        if (damageTextPrefab == null)
        {
            Debug.LogError("[DamageTextManager] DamageText Prefabが設定されていません!");
            return;
        }
        if (damageCanvas == null)
        {
            Debug.LogError("[DamageTextManager] Canvasが設定されていません!");
            return;
        }

        // Prefabからダメージテキストを生成
        GameObject damageTextObj = Instantiate(damageTextPrefab, damageCanvas.transform);
        
        // DamageTextコンポーネントを取得して初期化
        DamageText damageText = damageTextObj.GetComponent<DamageText>();
        if (damageText != null)
        {
            damageText.Initialize(damage, worldPosition);
        }
        else
        {
            Debug.LogError("[DamageTextManager] DamageTextコンポーネントが見つかりません!");
            Destroy(damageTextObj);
        }
    }

    public void ShowJustParry(Vector3 worldPosition)
    {
        if (justParryTextPrefab == null)
        {
            Debug.LogError("[DamageTextManager] JustParryTextPrefabが設定されていません");
                return;
        }
        if (damageCanvas == null)
        {
            Debug.LogError("[DamageTextManager] Canvasが設定されていません!");
            return;
        }
        // Prefabからダメージテキストを生成
        GameObject justParryTextObj = Instantiate(justParryTextPrefab, damageCanvas.transform);

        // DamageTextコンポーネントを取得して初期化
        ParryText justParryText = justParryTextObj.GetComponent<ParryText>();
        if (justParryText != null)
        {
            justParryText.Initialize(worldPosition);
        }
        else
        {
            Debug.LogError("[DamageTextManager] ParryTextコンポーネントが見つかりません!");
            Destroy(justParryTextObj);
        }
    }

    /// <summary>
    /// 複数のダメージテキストを一度に表示(オプション)
    /// </summary>
    /// <param name="damages">ダメージ値の配列</param>
    /// <param name="worldPosition">基準となるワールド座標</param>
    public void ShowMultipleDamages(int[] damages, Vector3 worldPosition)
    {
        foreach (int damage in damages)
        {
            ShowDamage(damage, worldPosition);
        }
    }
}
