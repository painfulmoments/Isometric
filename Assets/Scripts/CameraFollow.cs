using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随设置")]
    public Transform target;
    public float smoothSpeed = 0.125f;

    [Header("侧视角设置")]
    public float sideDistance = 10f;     // 相机距离玩家的横向距离（X轴）
    public float heightOffset = 3f;      // 相机高度偏移（Y轴）
    public float forwardOffset = 0f;     // 相机前后偏移（Z轴）

    [Header("视角设置")]
    public bool lookAtPlayer = true;     // 是否始终看向玩家
    public Vector3 fixedRotation = new Vector3(0, 90, 0); // 固定旋转角度（如果不看向玩家）

    void FixedUpdate()
    {
        if (target == null) return;

        // 计算侧视角位置：玩家位置 + 侧面偏移
        Vector3 desiredPosition = new Vector3(
          target.position.x + sideDistance,  // X轴正方向（右侧）
          target.position.y + heightOffset,  // Y轴偏移（高度）
          target.position.z + forwardOffset  // Z轴跟随玩家
      );

        // 平滑移动到目标位置
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // 设置相机朝向
        if (lookAtPlayer)
        {
            transform.LookAt(target);
        }
        else
        {
            transform.rotation = Quaternion.Euler(fixedRotation);
        }
    }

    // 在Scene视图中显示相机位置预览
    private void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Vector3 cameraPos = new Vector3(
                target.position.x - sideDistance,
                target.position.y + heightOffset,
                target.position.z + forwardOffset
            );

            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(cameraPos, Vector3.one);

            // 画一条线显示相机到玩家的连线
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(cameraPos, target.position);
        }
    }
}