using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class Entity
{
    public int fight_id; //Unique ID for the entity in the fight
    public string name; //Name of the entity
    public int speed; //Speed of the entity. Added to the initiative roll
    public int maxHealth; //Max health of the entity
    public int currentHealth; //Current health of the entity
    public int attack;
    public int initiative; //Stores result of the initiative roll + speed for the entity
    public bool isPlayer; //Flag to determine if the entity is a player or enemy
    public bool isDead = false; //Flag to determine if the entity is dead
    public GameObject prefab; //The prefab to spawn in the fight
    public Slider healthBar; //The health bar for the entity
    public TextMeshProUGUI healthText; //The health text for the entity
    public string prefabDataPath; //The path to the prefab in the resources folder
    private Animator animator; //The animator for the entity

    public void RollInitiative()
    {
        initiative = Random.Range(1, 20) + speed;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            isDead = true;
            currentHealth = 0;
        }
        if (healthBar != null)
        {
            //Convert the health to a float between 0 and 1
            healthBar.value = (float)currentHealth / maxHealth;
        }
        else
        {
            Debug.LogError("No health bar found");
        }
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}";
        }
        else
        {
            Debug.LogError("No health text found");
        }
    }

    public Entity(string name, int speed, int maxHealth, int attack, int currentHealth)
    {
        this.name = name;
        this.speed = speed;
        this.maxHealth = maxHealth;
        this.currentHealth = currentHealth;
        this.attack = attack;
        this.isDead = false;
        this.prefab = Resources.Load<GameObject>(prefabDataPath);
    }

    public void playAnimation(string animation)
    {
        if (animator == null)
        {
            try
            {
                animator = prefab.GetComponentInChildren<Animator>();
            }
            catch 
            {
                Debug.LogError("No animator found");
                return;
            }
        }
        //Trigger the animation
        animator.SetTrigger(animation);
    }

    public void SetAnimState(int state)
    {
        if (animator == null)
        {
            try
            {
                animator = prefab.GetComponentInChildren<Animator>();
            }
            catch
            {
                Debug.LogError("No animator found");
                return;
            }
        }
        //Set the animation state int
        animator.SetInteger("AnimState", state);
    }

    public void GameSetup()
    {
        if (healthBar == null)
        {
            healthBar = prefab.GetComponentInChildren<Slider>();
        }
        if (healthText == null)
        {
            healthText = healthBar.GetComponentInChildren<TextMeshProUGUI>();
        }
        if (animator == null)
        {
            animator = prefab.GetComponentInChildren<Animator>();
        }
        TakeDamage(0); //Telling it to take 0 damage to update the health bar
    }
}
