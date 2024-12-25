using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private int ammo;
    public int skillCooldown;

    [SerializeField] private Slider xpSlider;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider skillSlider;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image[] arrows;

    public int currHealth;
    public int currAmmo;
    public int currXP;
    public int currLevel;
    public bool invincible;

    public float currCooldown;

    void Start()
    {
        currHealth = health;
        currAmmo = ammo;
        currXP = 0;
        currLevel = 1;
        currCooldown = 0;

        skillSlider.maxValue = skillCooldown;
        skillSlider.value = 0;

        healthSlider.maxValue = currHealth;
        healthSlider.value = currHealth;

        xpSlider.value = 0;
        xpSlider.maxValue = 100;

        invincible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            DecreaseHealth(1);
            DecreaseAmmo(1);
        }
        else if (Input.GetKeyDown(KeyCode.Equals))
        {
            IncreaseHealth(1);
            IncreaseAmmo(1);
        }
        currCooldown += Time.deltaTime;
        if (currCooldown > skillCooldown)
            currCooldown = skillCooldown;

        skillSlider.value = currCooldown;
    }

    public void IncreaseHealth(int amount)
    {
        if (currHealth + amount >= health)
            currHealth = health;
        else
            currHealth += amount;

        healthSlider.value = currHealth;
    }

    public void DecreaseHealth(int amount)
    {
        if (invincible)
            return;

        if (currHealth <= amount)
            currHealth = 0;
        else
            currHealth -= amount;

        healthSlider.value = currHealth;
        GetComponent<CharacterController>().PlayDamageSound();
    }

    public void IncreaseAmmo(int amount)
    {
        int initial = currAmmo;

        if (currAmmo + amount >= ammo)
            currAmmo = ammo;
        else
            currAmmo += amount;

        int final = currAmmo;

        for (int i = initial; i < final; i++)
            arrows[i].enabled = true;
    }

    public void DecreaseAmmo(int amount)
    {
        int initial = currAmmo;

        if (currAmmo <= amount)
            currAmmo = 0;
        else
            currAmmo -= amount;

        int final = currAmmo;

        for (int i = initial - 1; i >= final; i--)
            if (i >= 0)
                arrows[i].enabled = false;
    }

    public void IncreaseXP(int amount)
    {
        currXP += amount;
        if (currXP >= 100)
        {
            currXP -= 100;
            LevelUp();
        }

        xpSlider.value = currXP;
        xpText.text = "XP: " + currXP.ToString() + "/100";
    }

    public void LevelUp()
    {
        currLevel++;
        levelText.text = "Lvl: " + currLevel.ToString();
        skillCooldown--;
        currCooldown = skillCooldown;
        currHealth = health;
        currAmmo = 0;
    }
}
