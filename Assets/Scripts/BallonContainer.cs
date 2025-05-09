using UnityEngine;

public class BalloonContainerSway : MonoBehaviour
{
    [Header("振動設定")]
    public float amplitude = 0.1f;     // 振幅（上下の幅）
    public float frequency = 1f;       // 周波数（揺れる速さ）

    private Vector3 startPosition;     // 初期位置

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        float offsetY = Mathf.Sin(Time.time * frequency * 2f * Mathf.PI) * amplitude;
        transform.localPosition = startPosition + new Vector3(0, offsetY, 0);
    }
}
