using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform player;

    [Header("背景（遠・中・近景）各2枚ずつ配置）")]
    public Transform[] skyLayers;
    public Transform[] distantLayers;
    public Transform[] closeLayers;

    [Range(0f, 1f)]
    public float skySpeed = 0.1f;
    [Range(0f, 1f)]
    public float distantSpeed = 0.5f;
    [Range(0f, 1f)]
    public float closeSpeed = 0.8f;

    public float layerWidth = 180f; // 背景1枚分の幅

    private Vector3 lastPlayerPos;
    private bool isAutoScroll = false;

    void Start()
    {
        if (player != null)
        {
            lastPlayerPos = player.position;
        }
        else
        {
            isAutoScroll = true;
        }

        // ★ 2枚目の背景の位置を自動配置
        InitLayerPositions(skyLayers);
        InitLayerPositions(distantLayers);
        InitLayerPositions(closeLayers);
    }

    void InitLayerPositions(Transform[] layers)
    {
        if (layers == null || layers.Length < 2) return;

        Vector3 basePos = layers[0].position;
        layers[1].position = new Vector3(basePos.x + layerWidth, basePos.y, basePos.z);
    }

void Update()
{
    float deltaX;

    if (isAutoScroll)
    {
        deltaX = -5f * Time.deltaTime;
    }
    else
    {
        // ★ スクロール停止
        if (player.position.x >= 243.5f - 18 * 4)
        {
            deltaX = 0f;
        }
        else
        {
            deltaX = player.position.x - lastPlayerPos.x;
            lastPlayerPos = player.position;
        }
    }

    ScrollAndLoop(skyLayers, deltaX * skySpeed);
    ScrollAndLoop(distantLayers, deltaX * distantSpeed);
    ScrollAndLoop(closeLayers, deltaX * closeSpeed);
}


    void ScrollAndLoop(Transform[] layers, float moveX)
    {
        if (layers == null || layers.Length < 2) return;

        foreach (var layer in layers)
        {
            layer.position += new Vector3(moveX, 0f, 0f);
        }

        if (layers[0].position.x + layerWidth < Camera.main.transform.position.x)
        {
            Transform rightMost = layers[0].position.x > layers[1].position.x ? layers[0] : layers[1];
            Transform left = layers[0].position.x <= layers[1].position.x ? layers[0] : layers[1];

            left.position = new Vector3(
                rightMost.position.x + layerWidth,
                left.position.y,
                left.position.z
            );

            layers[0] = rightMost;
            layers[1] = left;
        }
    }
}
