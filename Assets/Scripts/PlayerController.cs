using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // -------------------- パラメータ設定 --------------------

    public float jumpForce = 7f;                     // ジャンプ力（上方向の力）
    public float moveSpeed = 3f;                     // 横方向への移動速度
    public GameObject groundCheck;                   // 接地判定用の子オブジェクト（地面に触れているか確認）
    public HPManager hpManager;                // HPの減少などを管理する外部スクリプト

    public float invincibleDuration = 2f;            // ダメージ後の無敵時間（秒）
    public float flashInterval = 0.1f;               // 無敵時間中の点滅間隔
    public float shotDuration = 0.2f;                // playerShot アニメーションの長さ（秒）

    // -------------------- 内部状態 --------------------

    private Rigidbody2D rb;                          // 2D物理用の Rigidbody
    private SpriteRenderer sr;                       // スプライトの描画コンポーネント
    private Animator animator;                       // アニメーション制御用

    private int jumpCount = 0;                       // 現在のジャンプ回数
    public int maxJumpCount = 1;                     // 最大ジャンプ回数
    
    private bool isGrounded = false;                 // 現在地面に接しているかどうか
    private bool isInvincible = false;               // 無敵状態中かどうか
    private bool isControlEnabled = true;            // 移動・ジャンプ有効フラグ
    private string currentState = "";                // 現在のアニメーションステート名
    private bool isShooting = false;                 // playerShot アニメーション中かどうか
    private float shotElapsed = 0f;                  // playerShot 再生経過時間

    // -------------------- 初期化処理 --------------------

    void Start()
    {
        Time.timeScale = 1.0f;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // エラー確認（Inspectorの設定ミス防止）
        if (groundCheck == null) Debug.LogError("groundCheck が設定されていません！");
        if (sr == null) Debug.LogError("SpriteRenderer が見つかりません！");
        if (animator == null) Debug.LogError("Animator が見つかりません！");
    }

    // -------------------- 毎フレーム処理（入力など） --------------------

    void Update()
    {
        if (!isControlEnabled) return;

        HandleAnimation(); // 状態に応じたアニメーション処理

        // スペースキーでジャンプ（多段ジャンプ対応）
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumpCount)
        {
            // ジャンプ直前に垂直速度をリセット（連打で跳ねないように）
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpCount++;
        }
    }

    // -------------------- 毎固定フレーム処理（物理演算） --------------------

    void FixedUpdate()
    {
        if (!isControlEnabled) return;
        // プレイヤーは常に右方向に移動
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }

    // -------------------- 外部から地面との接触を通知 --------------------

public void SetGrounded(bool grounded)
{
    isGrounded = grounded;

    if (grounded)
    {
        jumpCount = 0; // 地面に接地 → ジャンプ回数をリセット
    }
    else
    {
        // 空中に出た直後にジャンプカウントが0なら1にしておく（不正ジャンプ防止）
        if (jumpCount == 0)
        {
            jumpCount = 1;
        }
    }
}


    // -------------------- 外部から強制ジャンプさせる --------------------

    public void ForceJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f); // Y速度リセット
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        jumpCount = 1; // 空中状態になるのでジャンプ回数は1に
    }

    // -------------------- ダメージ処理 --------------------

    public void TakeDamage()
    {
        if (isInvincible) return; // 無敵中なら無視

        // HP 減少処理
        hpManager?.DecreaseHP(1);

        // 点滅＆無敵時間スタート
        StartCoroutine(FlashInvincibility());
    }

    // -------------------- 無敵時間中の点滅処理 --------------------

    private IEnumerator FlashInvincibility()
    {
        isInvincible = true;
        float elapsed = 0f;

        while (elapsed < invincibleDuration)
        {
            sr.enabled = false; // 非表示
            yield return new WaitForSeconds(flashInterval / 2f);

            sr.enabled = true;  // 表示
            yield return new WaitForSeconds(flashInterval / 2f);

            elapsed += flashInterval;
        }

        sr.enabled = true; // 最終的に表示状態に戻す
        isInvincible = false;
    }

    // -------------------- アニメーション制御 --------------------

    /// <summary>
    /// 他と重複しないようにアニメーション状態を切り替える
    /// </summary>
    private void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        if (!string.IsNullOrEmpty(currentState))
        {
            animator.ResetTrigger(currentState);
        }
        animator.SetTrigger(newState);        // 新しいトリガーを発火
        currentState = newState;              // 状態を記録
    }

    /// <summary>
    /// 状態に応じてアニメーションを切り替える
    /// </summary>
    private void HandleAnimation()
    {
        // playerShot 中なら他アニメーションを再生しない
        if (isShooting)
        {
            shotElapsed += Time.deltaTime;

            // 一定時間 + 着地状態 なら終了
            if (shotElapsed >= shotDuration && isGrounded)
            {
                isShooting = false;
            }
            else
            {
                return; // 他の状態に遷移しない
            }
        }

        // 空中状態ならジャンプ or フォール
        if (!isGrounded)
        {
            if (rb.velocity.y > 0.1f)
            {
                ChangeAnimationState("playerJump");
            }
            else if (rb.velocity.y < -0.1f)
            {
                ChangeAnimationState("playerFall");
            }
        }
        else
        {
            ChangeAnimationState("playerRun");
        }
    }

    // -------------------- 外部からショットアニメーションを再生 --------------------

    public void PlayShotAnimation()
    {
        isShooting = true;
        shotElapsed = 0f;
        ChangeAnimationState("playerShot");
    }

    // -------------------- 移動・ジャンプの入力受付を切り替える --------------------
    public void SetControlEnabled(bool enabled)
    {
        isControlEnabled = enabled;
    }


    // -------------------- 外部からアニメーショントリガーを与える --------------------
    public void TriggerAnimation(string triggerName)
    {
        if (animator != null && !string.IsNullOrEmpty(triggerName))
        {
            animator.SetTrigger(triggerName);
        }
    }

}
