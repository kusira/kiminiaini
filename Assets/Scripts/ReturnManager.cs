using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RetarnManager : MonoBehaviour
{
    [Header("フェード制御コンポーネント")]
    [SerializeField] private FadeController fadeController;

    [Header("ゲームクリア中は入力を無効化")]
    public bool gameCleared = false;

    private bool isRestarting = false;

    void Start()
    {
        if (fadeController == null)
        {
            fadeController = FindObjectOfType<FadeController>();
            if (fadeController == null)
            {
                Debug.LogError("FadeController がシーン内に見つかりませんでした。");
            }
        }
    }

    void Update()
    {
        if (isRestarting || gameCleared) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(RestartWithFade());
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(ReturnToTitleWithFade());
        }
    }

    private IEnumerator RestartWithFade()
    {
        isRestarting = true;

        if (fadeController != null)
        {
            fadeController.UpdateFadeOut();
        }

        yield return new WaitForSeconds(1.0f);

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    private IEnumerator ReturnToTitleWithFade()
    {
        isRestarting = true;

        if (fadeController != null)
        {
            fadeController.UpdateFadeOut();
        }

        yield return new WaitForSeconds(1.0f);

        SceneManager.LoadScene("TitleScene");
    }
}
