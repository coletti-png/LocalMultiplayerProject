using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PlayerClass : MonoBehaviour
{
    public enum Class { None, Tank, Sprinter, Balanced } //Health & Speed
    public enum Weapon { None, SMG, Shotgun, Sniper } //Weapon Type
    public enum PlayerColor { None, Red, Yellow, Green, Blue, Purple, Pink, Black } //Cosmetic Color

    public Class currentClass = Class.None;
    public Weapon currentWeapon = Weapon.None;
    public PlayerColor currentColor = PlayerColor.None;

    private PlayerStats playerStats;
    public GameObject selectMenu;

    private void Start()
    {
        playerStats = this.GetComponent<PlayerStats>();
    }

    private void Update()
    {
        ClassSelection();
        WeaponSelection();
        ColorSelection();
    }

    public void ResetPlayerClass()
    {
        currentClass = Class.None;
        currentWeapon = Weapon.None;
        currentColor = PlayerColor.None;
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

    public void ApplyClass(Class selectedClass)
    {
        currentClass = selectedClass;
    }

    public void WeaponSelection()
    {
        switch (currentWeapon)
        {
            case Weapon.None:
                playerStats.damagePerHit = 0;
                playerStats.maxBulletsInMag = 0;
                playerStats.reloadTime = 0f;
                playerStats.shotCooldown = 0f;
                break;
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

    public void ApplyWeapon(Weapon selectedWeapon)
    {
        currentWeapon = selectedWeapon;
    }

    public void ColorSelection()
    {
        switch (currentColor)
        {
            case PlayerColor.None:
                playerStats.PlayerColor(0);
                break;
            case PlayerColor.Red:
                playerStats.PlayerColor(1);
                break;

            case PlayerColor.Yellow:
                playerStats.PlayerColor(2);
                break;

            case PlayerColor.Green:
                playerStats.PlayerColor(3);
                break;

            case PlayerColor.Blue:
                playerStats.PlayerColor(4);
                break;

            case PlayerColor.Purple:
                playerStats.PlayerColor(5);
                break;

            case PlayerColor.Pink:
                playerStats.PlayerColor(6);
                break;

            case PlayerColor.Black:
                playerStats.PlayerColor(7);
                break;
        }
    }

    public void ApplyColor(PlayerColor selectedColor)
    {
        currentColor = selectedColor;
    }

    // -----------------------
    // BUTTON WRAPPERS
    // -----------------------

    // Classes
    public void ApplyClassTank() => ApplyClass(Class.Tank);
    public void ApplyClassSprinter() => ApplyClass(Class.Sprinter);
    public void ApplyClassBalanced() => ApplyClass(Class.Balanced);

    // Weapons
    public void ApplyWeaponSMG() => ApplyWeapon(Weapon.SMG);
    public void ApplyWeaponShotgun() => ApplyWeapon(Weapon.Shotgun);
    public void ApplyWeaponSniper() => ApplyWeapon(Weapon.Sniper);

    // Colors
    public void ApplyColorRed() => ApplyColor(PlayerColor.Red);
    public void ApplyColorYellow() => ApplyColor(PlayerColor.Yellow);
    public void ApplyColorGreen() => ApplyColor(PlayerColor.Green);
    public void ApplyColorBlue() => ApplyColor(PlayerColor.Blue);
    public void ApplyColorPurple() => ApplyColor(PlayerColor.Purple);
    public void ApplyColorPink() => ApplyColor(PlayerColor.Pink);
    public void ApplyColorBlack() => ApplyColor(PlayerColor.Black);

    public void CloseMenu()
    {
        selectMenu.SetActive(false);
        this.GetComponent<EventSystem>().enabled = false;

        playerStats.UpdateHealth();
    }

    public void OpenMenu()
    {
        selectMenu.SetActive(true);
        this.GetComponent<EventSystem>().enabled = true;
    }
}
