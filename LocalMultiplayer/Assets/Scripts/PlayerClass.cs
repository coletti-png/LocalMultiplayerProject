using UnityEngine;

public class PlayerClass : MonoBehaviour
{
    public enum Class { None, Tank, Sprinter, Balanced } //Health & Speed
    public enum Weapon { None, SMG, Shotgun, Sniper }

    public Class currentClass = Class.None;
    public Weapon currentWeapon = Weapon.None;

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = this.GetComponent<PlayerStats>();
    }

    private void Update()
    {
        ClassSelection();
        WeaponSelection();
    }

    public void ClassSelection()
    {
        switch (currentClass)
        {
            case Class.Tank:
                playerStats.startingHealth = 250;
                playerStats.moveSpeed = 5f;
                break;

            case Class.Sprinter:
                playerStats.startingHealth = 95;
                playerStats.moveSpeed = 12f;
                break;

            case Class.Balanced:
                playerStats.startingHealth = 125;
                playerStats.moveSpeed = 7.5f;
                break;
        }
    }

    public void WeaponSelection()
    {
        switch (currentWeapon)
        {
            case Weapon.SMG:
                playerStats.damagePerHit = 10;
                playerStats.maxBulletsInMag = 30;
                playerStats.reloadTime = 8f;
                playerStats.shotCooldown = 0f;
                break;

            case Weapon.Shotgun:
                playerStats.damagePerHit = 120;
                playerStats.maxBulletsInMag = 5;
                playerStats.reloadTime = 3f;
                playerStats.shotCooldown = 1.25f;
                break;

            case Weapon.Sniper:
                playerStats.damagePerHit = 250;
                playerStats.maxBulletsInMag = 1;
                playerStats.reloadTime = 5f;
                playerStats.shotCooldown = 0;
                break;
        }
    }

}
