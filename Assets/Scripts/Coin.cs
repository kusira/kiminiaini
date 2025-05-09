using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class Coin : MonoBehaviour
{
    private ScoreManager scoreManager;
    private Animator animator;
    private AudioSource audioSource;
    private bool isCollected = false; // 多重取得防止用

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (animator == null)
        {
            Debug.LogWarning("Coin に Animator がアタッチされていません。");
        }

        if (audioSource == null)
        {
            Debug.LogWarning("Coin に AudioSource がアタッチされていません。");
        }

        // ★ ScoreManager をシーン内の "Score" オブジェクトから取得
        GameObject scoreObj = GameObject.Find("Score");
        if (scoreObj != null)
        {
            scoreManager = scoreObj.GetComponent<ScoreManager>();
        }

        if (scoreManager == null)
        {
            Debug.LogWarning("ScoreManager が見つかりません。Coinのスコア加算ができません。");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return; // すでに取得済みなら無視

        if (other.CompareTag("Player") || other.CompareTag("Drill"))
        {
            if (scoreManager != null)
            {
                scoreManager.AddScore(1000); // 1000点加算
            }

            isCollected = true;

            // ★ Audio 再生（SEvalue 反映）
            if (audioSource != null)
            {
                audioSource.volume = SoundManager.Instance != null ? SoundManager.Instance.SEvalue : 1f;
                audioSource.Play();
            }

            // スケールを0.5倍にする（瞬時に）
            transform.localScale *= 0.5f;

            // アニメーション再生
            if (animator != null)
            {
                animator.SetTrigger("coinDestroy");
            }

            // 少し待ってから削除
            StartCoroutine(DestroyAfterDelay(0.2f));
        }
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
