using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public float SEvalue = 0.5f;
    public float BGMvalue = 0.5f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // PlayerPrefsから音量設定を読み込む
        SEvalue = PlayerPrefs.GetFloat("SEvalue", 0.5f);
        BGMvalue = PlayerPrefs.GetFloat("BGMvalue", 0.5f);
    }

    public void SetSE(float value)
    {
        SEvalue = value;
        PlayerPrefs.SetFloat("SEvalue", SEvalue);
    }

    public void SetBGM(float value)
    {
        BGMvalue = value;
        PlayerPrefs.SetFloat("BGMvalue", BGMvalue);
    }
}
