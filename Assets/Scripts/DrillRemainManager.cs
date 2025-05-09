using UnityEngine;

public class DrillLeftManager : MonoBehaviour
{
    [Tooltip("ドリルスロットの表示オブジェクト（最大3）")]
    public GameObject[] drillSlots; // 0: 左側 → 2: 右側（後ろから消したい）

    void Update()
    {
        // 現在のドリル数（"Drill" タグ付きオブジェクトの数をカウント）
        int activeDrillCount = GameObject.FindGameObjectsWithTag("Drill").Length;

        // 各スロットの表示制御
        for (int i = 0; i < drillSlots.Length; i++)
        {
            int reverseIndex = drillSlots.Length - 1 - i;

            if (drillSlots[reverseIndex] != null)
            {
                // ドリル数に応じて後ろから消す
                drillSlots[reverseIndex].SetActive(i + 1 > activeDrillCount);
            }
        }
    }
}
