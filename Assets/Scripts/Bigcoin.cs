using System.Collections;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class Bigcoin : MonoBehaviour
{
    [Header("取得時に通知するコイン番号（GetCoinManagerに通知）")]
    [SerializeField] private int coinIndex = 0;

    private ScoreManager scoreManager;
    private GetCoinManager getCoinManager;
    private Animator animator;
    private AudioSource audioSource;
    private bool isCollected = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (animator == null)
        {
            Debug.LogWarning("Bigcoin に Animator がアタッチされていません。");
        }

        if (audioSource == null)
        {
            Debug.LogWarning("Bigcoin に AudioSource がアタッチされていません。");
        }

        // ScoreManager をシーンから探す
        GameObject scoreObj = GameObject.Find("Score");
        if (scoreObj != null)
        {
            scoreManager = scoreObj.GetComponent<ScoreManager>();
        }

        // GetCoinManager をシーンから探す
        GameObject coinManagerObj = GameObject.Find("GetCoin");
        if (coinManagerObj != null)
        {
            getCoinManager = coinManagerObj.GetComponent<GetCoinManager>();
        }

        if (scoreManager == null)
        {
            Debug.LogWarning("ScoreManager が見つかりません。スコア加算できません。");
        }

        if (getCoinManager == null)
        {
            Debug.LogWarning("GetCoinManager が見つかりません。コイン通知できません。");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player"))
        {
            isCollected = true;

            // スコア加算
            scoreManager?.AddScore(10000);

            // コインインデックスを通知
            getCoinManager?.ActivateCoin(coinIndex);

            // 効果音再生（SEvalue 反映）
            if (audioSource != null)
            {
                float seVolume = SoundManager.Instance != null ? SoundManager.Instance.SEvalue : 1f;
                audioSource.volume = seVolume;
                audioSource.Play();
            }

            // 上昇アニメーション
            transform.DOMoveY(transform.position.y + 1.0f, 0.15f)
                     .SetEase(Ease.OutQuad)
                     .OnComplete(() =>
                     {
                         animator?.SetTrigger("destroyBigcoin");
                         StartCoroutine(DestroyAfterDelay(0.15f));
                     });
        }
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
