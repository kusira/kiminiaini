using UnityEngine;

/// <summary>
/// プレイヤーの足元に付ける接触検知用スクリプト
/// </summary>
public class GroundCheck : MonoBehaviour
{
    public PlayerController playerController; // 親プレイヤーの参照
    public GameOverManager gameOverManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 地面・ブロック・橋に接触
        if (other.CompareTag("Ground") || other.CompareTag("Block") || other.CompareTag("Bridge") || other.CompareTag("Spike"))
        {
            playerController?.SetGrounded(true);
            gameOverManager?.SetGrounded(true);
        }

        // 敵に接触した場合（上からなら倒す、それ以外はダメージ）
        if (other.CompareTag("Enemy"))
        {
            if (transform.position.y > other.bounds.center.y + 0.2f)
            {
                var enemy = other.GetComponent<EnemyDog>();
                if (enemy != null)
                {
                    Debug.Log("上から踏んで敵を倒した");
                    enemy.Die();
                    playerController?.ForceJump();
                }

                var bee = other.GetComponent<EnemyBee>();
                if (bee != null)
                {
                    Debug.Log("上から踏んで飛行敵を倒した");
                    bee.Die();
                    playerController?.ForceJump();
                }

                var crow = other.GetComponent<EnemyCrow>();
                if (crow != null)
                {
                    Debug.Log("上から踏んで飛行敵を倒した");
                    crow.Die();
                    playerController?.ForceJump();
                }
            }
            else
            {
                Debug.Log("踏めなかった（横 or 下）");
                playerController?.TakeDamage();
            }
        }

        // Void に接触したら即ダメージ
        if (other.CompareTag("Void"))
        {
            Debug.Log("Void に落下 → 即ダメージ");
            playerController?.hpManager?.DecreaseHP(100); // ← 体力を 100 減らす
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground") || other.CompareTag("Block") || other.CompareTag("Bridge") || other.CompareTag("Spike"))
        {
            playerController?.SetGrounded(false);
        }
    }
}
