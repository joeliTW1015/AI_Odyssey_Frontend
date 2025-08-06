using UnityEngine;
using UnityEngine.UI;

public class FishingGameManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] GameObject hook;
    [SerializeField] Transform fixedpoint;//魚竿固定線的點

    void Update()
    {
        if (lineRenderer == null || hook == null || fixedpoint == null) return;

        // 更新魚線的起點和終點
        lineRenderer.SetPosition(0, fixedpoint.position);
        lineRenderer.SetPosition(1, hook.transform.position);

        
    }
}
