using UnityEngine;

public class Wiggle : MonoBehaviour
{
    public float minAngle = -30f;
    public float maxAngle = 30f;
    public float speedMultiplier = 200f;

    private void Update()
    {
        transform.localEulerAngles = new Vector3(0f, 0f, Mathf.PingPong(Time.time * speedMultiplier, maxAngle * 2f) + minAngle);
    }
}
