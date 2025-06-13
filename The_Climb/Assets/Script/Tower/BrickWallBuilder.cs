using UnityEngine;

public class BrickWallBuilder : MonoBehaviour
{
    [Header("�����K�ݒ�")]
    public GameObject brickPrefab;

    [Header("���̍\��")]
    public int bricksPerLayer = 60;        // 1�i�̃����K���i�ː��j
    public int heightLayers = 60;          // �c�̒i��
    public float radius = 10f;             // ���a
    public float verticalSpacing = 0.39f;  // �㉺�̊Ԋu
    public float baseHeight = 0f;         // ��ԉ��̃����K�̍����iY���W�j

    void Start()
    {
        if (brickPrefab == null)
        {
            Debug.LogError("brickPrefab ���ݒ肳��Ă��܂���I");
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

            // �����i�Ȃ�p�x�𔼕����炵�ă����K�ς݂ɂ���
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
