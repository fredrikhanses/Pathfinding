using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    private Vector3 offset;
    [Range(0.01f, 1.0f)]
    public float smoothFactor = 0.5f;
    public bool lookAtPlayer = false;

    private void Start()
    {
        offset = transform.position - player.position;
    }

    private void LateUpdate()
    {
        Vector3 newPos = player.position + offset;
        transform.position = Vector3.Slerp(transform.position, newPos, smoothFactor);
        if(lookAtPlayer)
        {
            transform.LookAt(player);
        }
    }
}