using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // プレイヤーの Transform をアサイン

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 camPos = transform.position;
        float targetX = Mathf.Min(Mathf.Max(0f, player.position.x + 7.5f), 243.5f);
        transform.position = new Vector3(targetX, camPos.y, camPos.z);
    }
}
