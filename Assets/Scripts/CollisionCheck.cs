using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    [Tooltip("HPManagerを指定（指定しない場合は自動検索）")]
    public HPManager hpManager;

    private Rigidbody2D rb;
    private bool isGameOverHandled = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (hpManager == null)
        {
            hpManager = FindObjectOfType<HPManager>();
            if (hpManager == null)
            {
                Debug.LogError("HPManager が見つかりません。");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hpManager == null || isGameOverHandled) return;

        if (other.CompareTag("Ground") || other.CompareTag("Block")|| other.CompareTag("Spike"))
        {
            Debug.Log("ダメージ発生： " + other.name);
            hpManager.DecreaseHP(100);

            // HPが0になったらゲームオーバー演出
            if (hpManager.GetCurrentHP() <= 0)
            {
                HandleGameOver();
            }
        }
    }

    private void HandleGameOver()
    {
        isGameOverHandled = true;

        // 少し後ろに吹っ飛ばす（左後方に力を加える）
        if (rb != null)
        {
            rb.AddForce(new Vector2(-3f, 2f), ForceMode2D.Impulse);
        }
    }
}
