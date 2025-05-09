using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundSlider : MonoBehaviour
{
    public Slider seSlider;
    public Slider bgmSlider;

    public TextMeshProUGUI seText;
    public TextMeshProUGUI bgmText;

    void Start()
    {
        if (seSlider == null || bgmSlider == null)
        {
            Debug.LogWarning("SoundSlider: Sliderが未設定です。");
            return;
        }

        // 初期値を読み込み
        seSlider.value = SoundManager.Instance.SEvalue;
        bgmSlider.value = SoundManager.Instance.BGMvalue;
        UpdateText();

        // 値が変わったら反映
        seSlider.onValueChanged.AddListener((value) => {
            SoundManager.Instance.SetSE(value);
            UpdateText();
        });

        bgmSlider.onValueChanged.AddListener((value) => {
            SoundManager.Instance.SetBGM(value);
            UpdateText();
        });
    }

    private void UpdateText()
    {
        if (seSlider != null && seText != null)
            seText.text = $"{Mathf.RoundToInt(seSlider.value * 100)}";

        if (bgmSlider != null && bgmText != null)
            bgmText.text = $"{Mathf.RoundToInt(bgmSlider.value * 100)}";
    }
}
