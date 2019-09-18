using UnityEngine;

public class TimedDelete : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 2);
    }
}