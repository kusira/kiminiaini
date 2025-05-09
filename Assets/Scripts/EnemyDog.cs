using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyDog : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stopDistance = 20f;

    [Header("やられ設定")]
    [SerializeField] private Sprite deadSprite;

    private Transform playerTransform;
    private ScoreManager scoreManager;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private AudioSource audioSource;

    private bool isDead = false;
    private float rotateSpeed = 360f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();

        if (col == null)
        {
            Debug.LogWarning("EnemyDog に Collider2D が設定されていません！");
        }

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
                rb.velocity = new Vector2(0f, rb.velocity.y);
                return;
            }
        }

        rb.velocity = new Vector2(transform.right.x * moveSpeed * -1, rb.velocity.y);
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

        rb.velocity = new Vector2(0, -1f);
        rb.gravityScale = 1.5f;

        if (col != null)
        {
            col.enabled = false;
        }

        // ★ AudioSource 再生（音量をSEvalueで調整）
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
