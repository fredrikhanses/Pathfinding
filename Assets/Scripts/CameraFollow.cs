using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 offset;
    public Transform player;

    void Start()
    {
        offset = transform.position - player.position;
    }

    void LateUpdate()
    {
        transform.position = player.position + offset;
        transform.LookAt(player);
    }
}
