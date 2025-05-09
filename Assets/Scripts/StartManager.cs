using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartManager : MonoBehaviour
{
    [Header("点滅させるテキスト")]
    [SerializeField] private TextMeshProUGUI startText;

    [Header("フェード制御")]
    [SerializeField] private FadeController fadeController;

    [Header("効果音")]
    [SerializeField] private AudioSource audioSource;

    private bool isStarting = false;
    private SoundManager soundManager;

    void Start()
    {
        Time.timeScale = 1.0f;
        // ★ 自動アサイン
        if (fadeController == null)
            fadeController = FindObjectOfType<FadeController>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        soundManager = FindObjectOfType<SoundManager>();
    }

    void Update()
    {
        if (!isStarting && Input.GetKeyDown(KeyCode.Space))
        {
            isStarting = true;

            // ★ Audio 再生（SEvalue 反映）
            if (audioSource != null && soundManager != null)
            {
                audioSource.volume = soundManager.SEvalue;
                audioSource.Play();
            }

            StartCoroutine(BeginGameSequence());
        }
    }

    IEnumerator BeginGameSequence()
    {
        // テキスト3回点滅
        for (int i = 0; i < 3; i++)
        {
            if (startText != null) startText.alpha = 0f;
            yield return new WaitForSeconds(0.1f);
            if (startText != null) startText.alpha = 1f;
            yield return new WaitForSeconds(0.1f);
        }

        // テキスト非表示
        if (startText != null) startText.gameObject.SetActive(false);

        // 少し待ってからフェードアウト開始
        yield return new WaitForSeconds(0.3f);

        if (fadeController != null)
        {
            fadeController.UpdateFadeOut(); // フェードアウト処理
        }

        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("GameScene");
    }
}
