using System.Collections;
using UnityEngine;

/// <summary>
/// ドリルの制御スクリプト
/// ・進行方向への回転
/// ・地面/敵に当たるとアニメーションを再生して削除
/// ・一定以下の高さに落ちたら削除
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Drill : MonoBehaviour
{
    public float destroyYThreshold = -5f;     // Y座標がこの値以下になったら自動削除

    private Rigidbody2D rb;
    private Animator animator;
    private bool isDestroying = false;        // アニメーション中の二重削除防止

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogWarning("Animator がアタッチされていません。アニメーションが再生されません。");
        }
    }

    void Update()
    {
        // ドリルの進行方向に回転を合わせる
        if (rb.velocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        // 指定Y座標以下に落ちたら自動削除
        if (transform.position.y < destroyYThreshold && !isDestroying)
        {
            Destroy(gameObject);
        }
    }

void OnCollisionEnter2D(Collision2D collision)
{
    if (isDestroying) return;

    ContactPoint2D contact = collision.contacts[0];
    Vector2 normal = contact.normal;

    if (collision.collider.CompareTag("Bridge"))
    {
        // 下から衝突した場合（法線が下方向 ≒ Vector2.down）
        if (Vector2.Dot(normal, Vector2.down) > 0.7f)
        {
            // 下から衝突 → 無視
            return;
        }
    }

    if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Block") || 
        collision.collider.CompareTag("Bridge") || collision.collider.CompareTag("Spike"))
    {
        gameObject.tag = "Debris";

        
        // ★ レイヤーも変更（"Debris" レイヤーのインデックスを取得して設定）
        int debrisLayer = LayerMask.NameToLayer("Debris");
        if (debrisLayer != -1)
        {
            gameObject.layer = debrisLayer;
        }
        
        PlayDestroyAnimation();
    }
    else if (collision.collider.CompareTag("Enemy"))
    {
        var enemyDog = collision.collider.GetComponent<EnemyDog>();
        if (enemyDog != null) enemyDog.Die();

        var enemyBee = collision.collider.GetComponent<EnemyBee>();
        if (enemyBee != null) enemyBee.Die();

        var enemyCrow = collision.collider.GetComponent<EnemyCrow>();
        if (enemyCrow != null) enemyCrow.Die();

        PlayDestroyAnimation();
    }
}


    /// <summary>
    /// アニメーションを再生し、終わったらドリルを削除
    /// </summary>
    void PlayDestroyAnimation()
    {
        if (animator != null)
        {
            isDestroying = true;

            // ★ 速度を1/10にする（破壊時にゆっくり）
            rb.velocity = rb.velocity * 0.1f;

            animator.SetTrigger("DrillDestroy");

            // アニメーション終了後に削除
            StartCoroutine(DestroyAfterDelay(0.2f));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
