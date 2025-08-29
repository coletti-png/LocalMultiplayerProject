using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    [SerializeField] private float lifetime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerStats>(out var stats))
        {
            stats.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
