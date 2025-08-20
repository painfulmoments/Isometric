using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class FallingPlatform : MonoBehaviour
{
    public float fallDelay = 0.5f;
    public float destroyY = -5f;

    private Rigidbody rb;
    private bool hasFallen = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // 初始静止
    }

    void Update()
    {
        if (hasFallen && transform.position.y <= destroyY)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !hasFallen)
        {
            hasFallen = true;
            Invoke(nameof(StartFalling), fallDelay);
        }
    }

    private void StartFalling()
    {
        rb.isKinematic = false;   // 开启重力
        rb.useGravity = true;
    }
}
