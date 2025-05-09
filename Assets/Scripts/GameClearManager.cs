using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using unityroom.Api;

public class GameClearManager : MonoBehaviour
{
    [Header("GameClear 親オブジェクト（子に1文字ずつ）")]
    public Transform gameClearLetterGroup;

    [Header("スコア表示テキスト")]
    public TextMeshProUGUI scoreDisplayText;

    [Header("スコア取得元スクリプト")]
    public ScoreManager scoreManager;

    [Header("スペース表示テキスト")]
    public TextMeshProUGUI spaceKeyPromptText;

    [Header("アニメーション設定")]
    public float letterBounceHeight = 30f;
    public float letterBounceDuration = 0.3f;
    public float letterBounceInterval = 0.1f;
    public float scoreFadeDuration = 0.5f;
    public float scoreStartOffsetY = 30f;
    public float spaceTextDelay = 0.5f;
    public float spaceTextStartOffsetY = 30f;
    public float spaceTextFadeDuration = 0.5f;

    private bool isRestartEnabled = false;
    private int currentScore;

    void Start()
    {
        currentScore = scoreManager.GetScore();
        UnityroomApiClient.Instance.SendScore(1, currentScore, ScoreboardWriteMode.HighScoreDesc);

        // 初期状態のセットアップ
        scoreDisplayText.text = "";
        scoreDisplayText.alpha = 0f;

        if (spaceKeyPromptText != null)
        {
            spaceKeyPromptText.text = "- Press Space to Start -";
            spaceKeyPromptText.alpha = 0f;
        }

        StartCoroutine(PlayGameClearSequence());
    }

    void Update()
    {
        if (isRestartEnabled && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private IEnumerator PlayGameClearSequence()
    {
        int letterCount = gameClearLetterGroup.childCount;

        for (int i = 0; i < letterCount; i++)
        {
            Transform letter = gameClearLetterGroup.GetChild(i);
            Vector3 originalPos = letter.localPosition;

            letter.DOLocalMoveY(originalPos.y + letterBounceHeight, letterBounceDuration / 2)
                  .SetEase(Ease.OutQuad)
                  .OnComplete(() =>
                  {
                      letter.DOLocalMoveY(originalPos.y, letterBounceDuration / 2).SetEase(Ease.InQuad);
                  });

            yield return new WaitForSeconds(letterBounceInterval);
        }

        yield return new WaitForSeconds(0.5f);
        ShowScore();
    }

    private void ShowScore()
    {
        if (scoreManager != null && scoreDisplayText != null)
        {
            scoreDisplayText.text = currentScore.ToString("D7");

            Vector3 originalPos = scoreDisplayText.rectTransform.localPosition;
            scoreDisplayText.rectTransform.localPosition = originalPos - new Vector3(0, scoreStartOffsetY, 0);
            scoreDisplayText.alpha = 0f;

            scoreDisplayText.rectTransform.DOLocalMoveY(originalPos.y, scoreFadeDuration).SetEase(Ease.OutQuad);
            scoreDisplayText.DOFade(1f, scoreFadeDuration);

            // 次の段階へ：スペースキーの案内表示
            StartCoroutine(ShowSpacePrompt());
        }
    }

    private IEnumerator ShowSpacePrompt()
    {
        yield return new WaitForSeconds(spaceTextDelay);

        if (spaceKeyPromptText != null)
        {
            Vector3 originalPos = spaceKeyPromptText.rectTransform.localPosition;
            spaceKeyPromptText.rectTransform.localPosition = originalPos - new Vector3(0, spaceTextStartOffsetY, 0);
            spaceKeyPromptText.alpha = 0f;

            spaceKeyPromptText.rectTransform.DOLocalMoveY(originalPos.y, spaceTextFadeDuration).SetEase(Ease.OutCubic);
            spaceKeyPromptText.DOFade(1f, spaceTextFadeDuration);
        }

        isRestartEnabled = true;
    }
}
