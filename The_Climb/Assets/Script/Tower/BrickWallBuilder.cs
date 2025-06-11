using UnityEngine;

public class BrickWallBuilder : MonoBehaviour
{
    [Header("ƒŒƒ“ƒKİ’è")]
    public GameObject brickPrefab;

    [Header("“ƒ‚Ì\‘¢")]
    public int bricksPerLayer = 6;        // 1’i‚ÌƒŒƒ“ƒK”iŒË”j
    public int heightLayers = 5;          // c‚Ì’i”
    public float radius = 5f;             // ”¼Œa
    public float verticalSpacing = 0.5f;  // ã‰º‚ÌŠÔŠu

    void Start()
    {
        if (brickPrefab == null)
        {
            Debug.LogError("brickPrefab ‚ªİ’è‚³‚ê‚Ä‚¢‚Ü‚¹‚ñI");
            return;
        }

        BuildWall();
    }

    void BuildWall()
    {
        float angleStep = 360f / bricksPerLayer;

        for (int y = 0; y < heightLayers; y++)
        {
            float currentHeight = y * verticalSpacing;

            // ‹ô”’i‚È‚çŠp“x‚ğ”¼•ª‚¸‚ç‚µ‚ÄƒŒƒ“ƒKÏ‚İ‚É‚·‚é
            float angleOffset = (y % 2 == 1) ? angleStep / 2f : 0f;

            for (int i = 0; i < bricksPerLayer; i++)
            {
                float angle = (i * angleStep + angleOffset) * Mathf.Deg2Rad;

                Vector3 position = new Vector3(
                    Mathf.Cos(angle) * radius,
                    currentHeight,
                    Mathf.Sin(angle) * radius
                );

                // ŠOŒü‚«‚ÉŒü‚¯‚é‚½‚ß‚É -angleStep*i ‚Å‚È‚­A-Mathf.Rad2Deg * angle ‚É‚·‚é
                Quaternion rotation = Quaternion.Euler(0, -Mathf.Rad2Deg * angle, 0);

                Instantiate(brickPrefab, position, rotation, transform);
            }
        }
    }
}
