using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Entity
{
    public int fight_id; //Unique ID for the entity in the fight
    public string name; //Name of the entity
    public int speed; //Speed of the entity. Added to the initiative roll
    private int _maxHealth; //Max health of the entity
    public int maxHealth
    {
        get { return _maxHealth; }
        set
        {
            if (value > _maxHealth)  //If max health increases, increase current health by the difference
            {
                currentHealth += (value - _maxHealth);
            }
            _maxHealth = value;
        }
    }
    private int _currentHealth; //Current health of the entity
    public int currentHealth
    {
        get { return _currentHealth; }
        set
        {
            if (value > maxHealth) //Can't go higher than max health
            {
                _currentHealth = maxHealth;
            }
            else if (value < 0) //Sets entity to dead if health goes below 0
            {
                isDead = true;
                _currentHealth = 0;
            }
            else //Sets health to the provided value
            {
                _currentHealth = value;
            }
        }
    }
    public int attack; //Attack power of the entity
    public int initiative; //Stores result of the initiative roll + speed for the entity
    public bool isPlayer; //Flag to determine if the entity is a player or enemy
    public bool isDead = false; //Flag to determine if the entity is dead

    public void RollInitiative()
    {
        initiative = Random.Range(1, 20) + speed;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    public Entity(string name, int speed, int maxHealth, int attack, int currentHealth)
    {
        this.name = name;
        this.speed = speed;
        this.maxHealth = maxHealth;
        this.attack = attack;
        this.currentHealth = currentHealth;
        this.isDead = false;
    }
}
