using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// ダメージテキストの個別制御
/// 生成、アニメーション、自動削除
/// </summary>
public class DamageText : MonoBehaviour
{
    [Header("コンポーネント")]
    private TextMeshProUGUI textMesh;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    [Header("アニメーション設定")]
    [SerializeField] private float moveSpeed = 50f;        // 上昇速度
    [SerializeField] private float lifetime = 1.5f;        // 表示時間
    [SerializeField] private float randomOffsetRange = 30f; // ランダム生成のオフセット範囲

    private Vector3 moveDirection;
    private float timer;

    void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        
        // CanvasGroupがなければ追加
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    /// <summary>
    /// ダメージテキストを初期化して表示
    /// </summary>
    /// <param name="damage">表示するダメージ値</param>
    /// <param name="worldPosition">ワールド座標での表示位置</param>
    public void Initialize(int damage, Vector3 worldPosition)
    {
        textMesh.text = damage.ToString();
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        
        // ランダム化
        float randomX = Random.Range(-randomOffsetRange, randomOffsetRange);
        float randomY = Random.Range(-randomOffsetRange * 0.5f, randomOffsetRange);
        screenPosition.x += randomX;
        screenPosition.y += randomY;
        
        rectTransform.position = screenPosition;

        //出現箇所にランダム性を持たせる
        moveDirection = Vector3.up + new Vector3(Random.Range(-0.2f, 0.2f), 0, 0);
        
        // 初期化
        canvasGroup.alpha = 1f;
        timer = 0f;
        StartCoroutine(AnimateText());
    }

    /// <summary>
    /// テキストのアニメーション処理
    /// </summary>
    IEnumerator AnimateText()
    {
        while (timer < lifetime)
        {
            timer += Time.deltaTime;

            rectTransform.position += moveDirection * moveSpeed * Time.deltaTime;
            if (timer > lifetime * 0.5f)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, (timer - lifetime * 0.5f) / (lifetime * 0.5f));
            }

            yield return null;
        }
        //終了後破棄
        Destroy(gameObject);
    }
}
