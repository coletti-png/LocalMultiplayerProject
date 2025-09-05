using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public int currentHealth;
    public int startingHealth;

    [Header("Movement")]
    public float moveSpeed;

    [Header("Weapon")]
    public int damagePerHit;
    public float reloadTime;
    public float bulletsInMag;
    public int maxBulletsInMag;
    public float shotCooldown;

    [Header("Color")]
    public Material[] playerColors;

    void Start()
    {
        currentHealth = startingHealth;
    }

    public void PlayerColor(int colorNum)
    {
        this.GetComponent<Renderer>().material = playerColors[colorNum];
    }

    public void TakeDamage(int damageTaken)
    {
        currentHealth -= damageTaken;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        //Add Death/Respanwing logic here
        Debug.Log("Player Died");
    }
}
