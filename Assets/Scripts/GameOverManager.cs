using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("プレイヤー関連")]
    public PlayerController playerController;
    public ThrowDrill throwDrill;

    [Header("BGM関連")]
    public BGMManager bgmManager;
    [Header("フェードコントローラ")]
    public FadeController fadeController;

    private bool isGameOver = false;
    private bool isGrounded = false;
    private float elapsed = 0f;
    private bool hasTriggeredSecond = false;
    private bool hasAddedForce = false;
    private bool hasShakenCamera = false;

    void Start()
    {
        // BGMManager が未アサインなら自動取得
        if (bgmManager == null)
        {
            bgmManager = FindObjectOfType<BGMManager>();
        }

    }

    void Update()
    {
        if (!isGameOver) return;

        elapsed += Time.unscaledDeltaTime;

        if (!hasTriggeredSecond && (elapsed >= 0.5f || isGrounded))
        {
            hasTriggeredSecond = true;
            playerController?.TriggerAnimation("damageReaction2");
        }

        if (!hasShakenCamera)
        {
            hasShakenCamera = true;
            ShakeCameraOnce();
        }
    }

    public void TriggerGameOver(bool hasAddedForce)
    {
        if (isGameOver) return;

        isGameOver = true;
        elapsed = 0f;
        isGrounded = false;
        hasTriggeredSecond = false;
        hasShakenCamera = false;

        Debug.Log("ゲームオーバー");

        playerController.gameObject.tag = "GameOver";
        Time.timeScale = 0.3f;

        playerController.SetControlEnabled(false);
        throwDrill?.SetControlEnabled(false);

        if (!hasAddedForce)
        {
            Rigidbody2D rb = playerController.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = new Vector2(-2, 1).normalized;
                rb.AddForce(direction * 8f, ForceMode2D.Impulse);
                this.hasAddedForce = true;
            }
        }

        playerController?.TriggerAnimation("damageReaction1");

        // 🔽 通常のBGMを即時停止
        if (bgmManager != null)
        {
            bgmManager.StopImmediately();
        }

        // 3秒後にシーン再読み込み
        StartCoroutine(RestartSceneWithFade());
    }

    public void SetGrounded(bool grounded)
    {
        isGrounded = grounded;
    }

    private void ShakeCameraOnce()
    {
        if (Camera.main != null)
        {
            Camera.main.transform.DOShakePosition(
                duration: 1.5f,
                strength: 0.2f,
                vibrato: 20,
                randomness: 90,
                snapping: false,
                fadeOut: true
            ).SetUpdate(true); // UnscaledTimeに対応
        }
    }

    private IEnumerator RestartSceneWithFade()
    {
        yield return new WaitForSecondsRealtime(3 * 0.3f);

        Time.timeScale = 1.0f;

        if (fadeController != null)
        {
            fadeController.UpdateFadeOut();
            yield return new WaitForSecondsRealtime(0.6f);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
