using UnityEngine;

public class BrickWallBuilder : MonoBehaviour
{
    [Header("レンガ設定")]
    public GameObject brickPrefab;

    [Header("塔の構造")]
    public int bricksPerLayer = 60;        // 1段のレンガ数（戸数）
    public int heightLayers = 60;          // 縦の段数
    public float radius = 10f;             // 半径
    public float verticalSpacing = 0.39f;  // 上下の間隔
    public float baseHeight = 0f;         // 一番下のレンガの高さ（Y座標）

    void Start()
    {
        if (brickPrefab == null)
        {
            Debug.LogError("brickPrefab が設定されていません！");
            return;
        }

        BuildWall();
    }

    void BuildWall()
    {
        float angleStep = 360f / bricksPerLayer;

        for (int y = 0; y < heightLayers; y++)
        {
            float currentHeight = baseHeight + y * verticalSpacing;

            // 偶数段なら角度を半分ずらしてレンガ積みにする
            float angleOffset = (y % 2 == 1) ? angleStep / 2f : 0f;

            for (int i = 0; i < bricksPerLayer; i++)
            {
                float angle = (i * angleStep + angleOffset) * Mathf.Deg2Rad;

                Vector3 position = new Vector3(
                    Mathf.Cos(angle) * radius,
                    currentHeight,
                    Mathf.Sin(angle) * radius
                );

                Quaternion rotation = Quaternion.Euler(0, -Mathf.Rad2Deg * angle, 0);

                Instantiate(brickPrefab, position, rotation, transform);
            }
        }
    }
}
