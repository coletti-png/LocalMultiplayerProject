using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    private bool canDealDamage = true;
    [SerializeField] private float lifetime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerStats>(out var stats) && canDealDamage)
        {
            stats.TakeDamage(damage);
            canDealDamage = false;
            Debug.Log("Hit");
            Debug.Log(damage);
        }

        if (collision.gameObject.tag != "Bullet")
        {
            Destroy(gameObject);
        }
    }
}
