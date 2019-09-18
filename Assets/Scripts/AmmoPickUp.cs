using UnityEngine;
using control;
using Search;

public class AmmoPickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Location location = new Location((int)transform.position.x, (int)transform.position.y);
            ClickMove.pickUpPosition = location;
            Destroy(gameObject);
        }
    }
}
