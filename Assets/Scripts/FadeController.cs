using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class FadeController : MonoBehaviour
{
    [Header("最初からフェードインが完了しているか")]
    public bool isFadeInComplete = false;

    private CanvasGroup canvasGroup;
    private bool isFadeIn = false;
    private bool isFadeOut = false;

    void Awake() {
        this.canvasGroup = this.GetComponent<CanvasGroup>(); // CanvasGroupの取得
    }

    void Start()
    {
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroupがアタッチされていません");
            return;
        }

        // 最初からフェードインしていない場合だけフェードイン
        if(!isFadeInComplete)
        {
            UpdateFadeIn();
        }
        else
        {
            this.canvasGroup.alpha = 0.0f; // 透明
        }
        canvasGroup.blocksRaycasts = false; // 当たり判定をなくす
    }

    // フェードイン処理
    public void UpdateFadeIn()
    {
        this.canvasGroup.alpha = 1.0f; // 初期は透明
        this.canvasGroup.DOFade(0.0f, 0.5f); // 1秒かけてフェードアウト
        canvasGroup.blocksRaycasts = false; // 当たり判定をなくす
    }

    // フェードアウト処理
    public void UpdateFadeOut()
    {
        canvasGroup.blocksRaycasts = true;
        this.canvasGroup.alpha = 0f; // 初期は透明
        this.canvasGroup.DOFade(1.0f, 0.5f); // 1秒かけてフェードイン
    }

    // フェードアウト後にシーン遷移
    public void MoveNextScene(string nextScene){
        canvasGroup.blocksRaycasts = false;
        this.canvasGroup.DOFade(1.0f, 0.5f)
            .OnComplete(() => {
                SceneManager.LoadScene(nextScene);
            });
    }
}
