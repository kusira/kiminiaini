using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(AudioSource))]
public class EnemyCrow : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stopDistance = 20f;
    [SerializeField] private float minXSpeed = 0f;
    [SerializeField, Range(0f, 1f)] private float homingStrength = 0.1f;

    [Header("やられ設定")]
    [SerializeField] private Sprite deadSprite;

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
        rb.gravityScale = 0f;

        if (col == null)
        {
            Debug.LogWarning("EnemyCrow に Collider2D が設定されていません！");
        }

        var playerObj = GameObject.Find("Player");
        if (playerObj != null) playerTransform = playerObj.transform;

        var scoreObj = GameObject.Find("Score");
        if (scoreObj != null) scoreManager = scoreObj.GetComponent<ScoreManager>();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        if (playerTransform != null)
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);

            if (distance < stopDistance)
            {
                Vector2 direction = (playerTransform.position - transform.position).normalized;
                Vector2 targetVelocity = direction * moveSpeed;

                if (Mathf.Abs(targetVelocity.x) < minXSpeed)
                {
                    targetVelocity.x = minXSpeed * Mathf.Sign(targetVelocity.x);
                }

                rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, homingStrength);
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
    }

    void Update()
    {
        if (isDead)
        {
            transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        }
        else if (playerTransform != null)
        {
            Vector2 direction = rb.velocity;
            if (direction.sqrMagnitude > 0.01f)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        scoreManager?.AddScore(300);

        if (deadSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = deadSprite;
        }

        if (audioSource != null)
        {
            audioSource.volume = SoundManager.Instance != null ? SoundManager.Instance.SEvalue : 1f;
            audioSource.Play();
        }

        rb.gravityScale = 1.5f;
        rb.velocity = new Vector2(0f, -1f);

        if (col != null)
        {
            col.enabled = false;
        }

        StartCoroutine(DestroyAfterSeconds(2f));
    }

    private IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
