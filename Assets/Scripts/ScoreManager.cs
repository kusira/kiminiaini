using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("スコア表示用テキスト")]
    public TextMeshProUGUI scoreText;
    private int currentScore = 0; 

    void Start()
    {
        UpdateScoreText();
    }

    /// <summary>
    /// スコアを加算する（外部から呼び出し）
    /// </summary>
    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreText();
    }

    /// <summary>
    /// 現在のスコアを取得（参照用）
    /// </summary>
    public int GetScore()
    {
        return currentScore;
    }

    /// <summary>
    /// スコアの表示を更新
    /// </summary>
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString("D7");
        }
    }
}
