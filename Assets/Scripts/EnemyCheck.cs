using UnityEngine;

/// <summary>
/// Player の子オブジェクトとしてアタッチし、敵との接触を検知する Trigger 用スクリプト
/// </summary>
public class EnemyCheck : MonoBehaviour
{
    public PlayerController player; // 親プレイヤーの参照

    // Trigger で Enemy に触れたら親プレイヤーに通知
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Spike"))
        {
            player?.TakeDamage();
        }
    }
}
