using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    private AudioSource audioSource;
    private SoundManager soundManager;
    public bool isLoop = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = isLoop;
        audioSource.playOnAwake = false;

        // SoundManager のインスタンス取得
        soundManager = FindObjectOfType<SoundManager>();
        if (soundManager == null)
        {
            Debug.LogWarning("SoundManager がシーンに見つかりません。");
        }

        audioSource.volume = soundManager != null ? soundManager.BGMvalue : 0.5f;
        audioSource.Play();
    }

    void Update()
    {
        if (soundManager != null)
        {
            audioSource.volume = soundManager.BGMvalue;
        }
    }

    public void StopImmediately()
    {
        if (TryGetComponent<AudioSource>(out var audio))
        {
            audio.Stop();
        }
    }
}
