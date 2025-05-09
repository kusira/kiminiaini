using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer), typeof(AudioSource))]
public class ThrowDrill : MonoBehaviour
{
    [Header("ドリル発射関連設定")]
    public Transform startPoint;
    public GameObject drillPrefab;
    public float initialSpeed = 10f;
    public float drillFireInterval = 1.0f;
    public int maxDrillNumber = 3;

    [Header("パラボラ線表示")]
    public float lineLength = 10f;
    public int maxResolution = 100;

    [Header("参照コンポーネント")]
    public Rigidbody2D playerRb;
    public PlayerController playerController;

    private LineRenderer lineRenderer;
    private AudioSource audioSource;
    private Vector2 gravity;
    private float lastFireTime = -Mathf.Infinity;
    private bool isControlEnabled = true;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        audioSource = GetComponent<AudioSource>();
        gravity = Physics2D.gravity;
    }

    void Update()
    {
        if (!isControlEnabled)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        DrawParabolaWithLength(lineLength);

        if (Input.GetMouseButtonDown(0))
        {
            TryLaunchDrill();
        }

        // 毎フレーム SEVolume を反映
        var smObj = GameObject.Find("SoundManager");
        if (smObj != null)
        {
            var sm = smObj.GetComponent<SoundManager>();
            if (sm != null && audioSource != null)
            {
                audioSource.volume = sm.SEvalue;
            }
        }
    }

    void TryLaunchDrill()
    {
        if (Time.time - lastFireTime < drillFireInterval) return;

        int existingDrills = GameObject.FindGameObjectsWithTag("Drill").Length;
        if (existingDrills >= maxDrillNumber) return;

        LaunchDrill();
        lastFireTime = Time.time;

        playerController?.PlayShotAnimation();

        // ★ 発射音を鳴らす
        if (audioSource != null) audioSource.Play();
    }

    void LaunchDrill()
    {
        Vector2 velocity = GetLaunchVelocity(true);
        GameObject drill = Instantiate(drillPrefab, startPoint.position, Quaternion.identity);

        Rigidbody2D rb = drill.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("ドリルに Rigidbody2D がありません！");
            return;
        }

        rb.velocity = velocity;
    }

    void DrawParabolaWithLength(float targetLength)
    {
        Vector2 velocity = GetLaunchVelocity(false);
        float t = 0f;
        float timeStep = 0.05f;
        float totalLength = 0f;

        List<Vector3> positions = new List<Vector3>();
        Vector2 prevPos = startPoint.position;
        positions.Add(prevPos);

        while (totalLength < targetLength && positions.Count < maxResolution)
        {
            t += timeStep;
            Vector2 currentPos = CalculatePositionAtTime(startPoint.position, velocity, t);
            totalLength += Vector2.Distance(prevPos, currentPos);
            positions.Add(currentPos);
            prevPos = currentPos;
        }

        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }

    Vector2 GetLaunchVelocity(bool addInertia)
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(Camera.main.transform.position.z - startPoint.position.z);
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        Vector2 rawDirection = ((Vector2)mouseWorldPos - (Vector2)startPoint.position).normalized;
        float angle = Vector2.SignedAngle(Vector2.right, rawDirection);
        float clampedAngle = Mathf.Clamp(angle, -10f, 60f);
        float rad = clampedAngle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        Vector2 launchVelocity = direction * initialSpeed;

        if (addInertia && playerRb != null)
        {
            launchVelocity.x += playerRb.velocity.x;
        }

        return launchVelocity;
    }

    Vector2 CalculatePositionAtTime(Vector2 origin, Vector2 velocity, float t)
    {
        return origin + velocity * t + 0.5f * gravity * t * t;
    }

    public void SetControlEnabled(bool enabled)
    {
        isControlEnabled = enabled;
    }

    public bool IsControlEnabled()
    {
        return isControlEnabled;
    }
}
