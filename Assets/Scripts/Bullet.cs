using UnityEngine;
using Control;
using Search;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!(collision.gameObject.tag == "Bullet"))
        {
            Location location = new Location((int)collision.transform.position.x, (int)collision.transform.position.y);
            ClickMove.obstaclePosition = location;
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        ClickMove.bulletSpawned = false;
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}