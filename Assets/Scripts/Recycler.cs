using UnityEngine;

public class Recycler: MonoBehaviour
{
    public ObstacleFactory factory;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == 9)
        {
            factory.Recycle(collision.transform);
        }
    }
}
