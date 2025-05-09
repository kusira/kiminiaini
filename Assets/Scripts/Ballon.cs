using UnityEngine;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class BallonDestroy : MonoBehaviour
{
    [Header("吊り下げている紐オブジェクト")]
    public GameObject stringObject;

    [Header("バルーンに繋がれているレンガたち")]
    public GameObject[] brickBlocks;

    [Header("レンガの挙動設定")]
    public float brickForce = 3f;
    public float torqueAmount = 100f;
    public float brickDestroyDelay = 4f;

    [Header("効果音")]
    public AudioSource ballonSE;
    public AudioSource blockSE;

    private Animator animator;
    private bool isDestroying = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogWarning("Animator がアタッチされていません。");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDestroying) return;

        if (other.CompareTag("Drill"))
        {
            isDestroying = true;

            animator?.SetTrigger("ballonDestroy");

            // SE再生
            float seValue = SoundManager.Instance != null ? SoundManager.Instance.SEvalue : 1f;

            if (ballonSE != null)
            {
                ballonSE.volume = seValue;
                ballonSE.Play();
            }

            if (blockSE != null)
            {
                blockSE.volume = seValue;
                blockSE.Play();
            }

            // 紐を下に動かして削除
            if (stringObject != null)
            {
                stringObject.transform.DOLocalMoveY(-0.5f, 0.1f).SetRelative();
                Destroy(stringObject, 0.1f);
            }

            ExplodeBricks();
            StartCoroutine(DestroyAfterDelay(0.4f));
        }
    }

    void ExplodeBricks()
    {
        foreach (GameObject brick in brickBlocks)
        {
            if (brick == null) continue;

            var col = brick.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            var rb = brick.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = brick.AddComponent<Rigidbody2D>();
            }

            rb.gravityScale = 1.5f;

            float angle = Random.Range(70f, 110f) * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;

            rb.AddForce(direction * brickForce, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-torqueAmount, torqueAmount));

            Destroy(brick, brickDestroyDelay);
        }
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
