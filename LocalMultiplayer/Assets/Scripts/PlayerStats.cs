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

    [Header("Script Grabs")]
    public PlayerClass playerClass;

    public void UpdateHealth()
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
        playerClass.OpenMenu();

        playerClass.ResetPlayerClass();

        //Reset Pos
        //Make Invicible while choosing?
    }
}
