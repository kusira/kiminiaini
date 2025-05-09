using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBee : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 0f;
    public float stopDistance = 10f;

    [Header("やられ設定")]
    public Sprite deadSprite;

    private Transform playerTransform;
    private ScoreManager scoreManager;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private AudioSource audioSource;

    private bool isDead = false;
    private float rotateSpeed = 180f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();

        if (col == null)
        {
            Debug.LogWarning("EnemyBee に Collider2D が設定されていません！");
        }

        rb.gravityScale = 0f;

        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null) playerTransform = playerObj.transform;

        GameObject scoreObj = GameObject.Find("Score");
        if (scoreObj != null) scoreManager = scoreObj.GetComponent<ScoreManager>();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        if (playerTransform != null)
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);
            if (distance >= stopDistance)
            {
                rb.velocity = Vector2.zero;
                return;
            }
        }

        rb.velocity = new Vector2(moveSpeed, 0f);
    }

    void Update()
    {
        if (isDead)
        {
            transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        scoreManager?.AddScore(200);

        if (deadSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = deadSprite;
        }

        rb.gravityScale = 1.5f;
        rb.velocity = new Vector2(0f, -1f);

        if (col != null)
        {
            col.enabled = false;
        }

        // ★ SE再生（SoundManagerのSEvalueを反映）
        if (audioSource != null && SoundManager.Instance != null)
        {
            audioSource.volume = SoundManager.Instance.SEvalue;
            audioSource.Play();
        }

        StartCoroutine(DestroyAfterSeconds(2f));
    }

    private IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
