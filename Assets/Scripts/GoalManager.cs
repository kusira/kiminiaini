using UnityEngine;

public class GoalManager : MonoBehaviour
{
    [Header("フェード制御")]
    [SerializeField] private FadeController fadeController;

    [Header("ゲームクリア表示")]
    [SerializeField] private GameObject gameClearObject;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

            if (other.CompareTag("Player"))
            {
                RetarnManager returnManager = FindObjectOfType<RetarnManager>();
            if (returnManager != null)
            {
                returnManager.gameCleared = true;
            }

            hasTriggered = true;

            // フェード開始
            fadeController?.UpdateFadeOut();

            // 無敵状態にする
            var hpManager = FindObjectOfType<HPManager>();
            if (hpManager != null)
            {
                hpManager.SetInvincible(true);
            }

            // 物理停止（重力や衝突を無効にする）
            var rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.simulated = false;
            }

            // プレイヤー操作を無効にする（任意）
            var player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SetControlEnabled(false);
            }

            // 2秒後にGameClearオブジェクトを表示
            Invoke(nameof(ShowGameClear), 2f);
        }
    }

    private void ShowGameClear()
    {
        if (gameClearObject != null)
        {
            gameClearObject.SetActive(true);
        }
    }
}
