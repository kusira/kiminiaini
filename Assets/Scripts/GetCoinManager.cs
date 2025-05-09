using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GetCoinManager : MonoBehaviour
{
    [Header("コイン画像")]
    public Sprite coinOnSprite;   // ON状態の画像（取得済み）
    public Sprite coinOffSprite;  // OFF状態の画像（未取得）

    [Header("コインスロット")]
    public Image[] baseImages;    // OFF画像（背景）
    public Image[] onImages;      // ON画像（取得済みで表示）

    public float animTime = 0.25f;
    private Vector3[] initialPositions;

    void Start()
    {
        InitUI();
    }

    void InitUI()
    {
        int count = Mathf.Min(baseImages.Length, onImages.Length);
        initialPositions = new Vector3[count];

        for (int i = 0; i < count; i++)
        {
            if (baseImages[i] != null)
                baseImages[i].sprite = coinOffSprite;

            if (onImages[i] != null)
            {
                onImages[i].sprite = coinOnSprite;
                onImages[i].gameObject.SetActive(false);
                onImages[i].color = Color.white;
                initialPositions[i] = onImages[i].rectTransform.localPosition;
            }
        }
    }

    /// <summary>
    /// 外部から指定インデックスのコインを取得状態にする
    /// </summary>
    /// <param name="index">表示させたいコインのインデックス</param>
    public void ActivateCoin(int index)
    {
        if (index < 0 || index >= onImages.Length) return;

        var img = onImages[index];
        if (img != null)
        {
            img.sprite = coinOnSprite;
            img.gameObject.SetActive(true);
            img.color = new Color(1, 1, 1, 1);
            img.rectTransform.localPosition = initialPositions[index];

            // スケール0から1へ（ぼよよんアニメーション）
            img.rectTransform.localScale = Vector3.zero;
            img.rectTransform.DOScale(Vector3.one, animTime).SetEase(Ease.OutBack);
        }
    }

    /// <summary>
    /// 全コイン状態をリセット（非表示）
    /// </summary>
    public void ResetAllCoins()
    {
        for (int i = 0; i < onImages.Length; i++)
        {
            if (onImages[i] != null)
            {
                onImages[i].gameObject.SetActive(false);
                onImages[i].rectTransform.localPosition = initialPositions[i];
                onImages[i].color = Color.white;
            }
        }
    }
}
