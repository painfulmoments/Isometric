using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class BackgroundFollow : MonoBehaviour
{
    [Header("跟随目标（建议为主角或摄像机）")]
    [SerializeField] private Transform follow;

    [Header("视差滚动控制")]
    public float scrollMultiplier = 0.2f;   // 控制背景滚动速度
    public bool followX = false;            // 是否跟随X轴
    public bool followY = false;            // 是否跟随Y轴

    private Material mat;
    private Vector3 lastFollowPosition;
    private Vector2 currentOffset;

    private void Start()
    {
        mat = GetComponent<Renderer>().material;
        if (follow == null)
        {
            follow = Camera.main.transform;
        }
        lastFollowPosition = follow.position;
    }

    private void LateUpdate()
    {
        Vector3 delta = follow.position - lastFollowPosition;

        // 只取 Z 轴方向的移动用于滚动背景
        currentOffset.x += delta.z * scrollMultiplier;

        if (followX)
            currentOffset.x += delta.x * scrollMultiplier;
        if (followY)
            currentOffset.y += delta.y * scrollMultiplier;

        mat.SetTextureOffset("_MainTex", currentOffset);

        lastFollowPosition = follow.position;
    }
}
