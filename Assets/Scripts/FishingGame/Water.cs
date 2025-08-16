using UnityEngine;
using UnityEngine.U2D;

public class Water : MonoBehaviour
{
    SpriteShapeController shape;
    public Transform[] controlPoints;
    int pointNum = 7;

    void Awake()
    {
        shape = GetComponent<SpriteShapeController>();
    }

    void Start()
    {
        UpdateSpline();
    }

    void Update()
    {
        UpdateSpline();
    }

    void UpdateSpline()
    {
        if (shape == null || controlPoints == null) return;

        var spline = shape.spline;

        for (int i = 0; i < controlPoints.Length; i++)
        {
            if (i >= pointNum) break;

            Vector3 pos = controlPoints[i].position;
            spline.SetPosition(i, new Vector3(pos.x, pos.y, 0));
            spline.SetTangentMode(i, ShapeTangentMode.Continuous);

            // 👉 設定左右手把方向與長度（相對位置）
            Vector3 leftTangent = -Vector3.left * 2f;
            Vector3 rightTangent = -Vector3.right * 2f;

            spline.SetLeftTangent(i, leftTangent);
            spline.SetRightTangent(i, rightTangent);
        }
    }
}
