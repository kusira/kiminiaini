using UnityEngine;

public class HPRecovery : MonoBehaviour
{
    // プレイヤーに触れたときに呼ばれる
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // プレイヤーの HPManager を取得
            HPManager hpManager = FindObjectOfType<HPManager>();
            if (hpManager != null)
            {
                hpManager.IncreaseHP(1); // HPを1回復
            }
            else
            {
                Debug.LogWarning("HPManager がシーン内に見つかりません！");
            }

            // 自身を削除
            Destroy(gameObject);
        }
    }
}
