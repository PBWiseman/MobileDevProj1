using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Entity
{
    public int id;
    public string name;
    public int speed;
    public int maxHealth;
    private int _currentHealth;
    public int currentHealth
    {
        get { return _currentHealth; }
        set
        {
            if (value > maxHealth)
            {
                _currentHealth = maxHealth;
            }
            else if (value < 0)
            {
                isDead = true;
                _currentHealth = 0;
            }
            else
            {
                _currentHealth = value;
            }
        }
    }
    public int attack;
    public int initiative;
    public int experienceReward; // Used for monsters
    private int _totalExperience; // Used for players
    public int totalExperience
    {
        get { return _totalExperience; }
        set
        {
            //Add the experience to the total experience and level up if needed
            _totalExperience += value;
            while (level < _totalExperience / 100) 
            {
                LevelUp();
            }
        }
    }
    public int level; // Used for players
    public bool isPlayer = false; //False by default, set to true when the entity is a player
    public bool isDead = false;

    public void Initialize()
    {
        currentHealth = maxHealth;
    }

    public void RollInitiative()
    {
        initiative = Random.Range(1, 20) + speed;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    private void LevelUp() //Temporary level up system.
    {
        level++;
        //TODO: Better system with player choice
        switch (Random.Range(0, 3))
        {
            case 0:
                maxHealth += 10;
                break;
            case 1:
                attack += 2;
                break;
            case 2:
                speed += 1;
                break;
        }
    }
}

[System.Serializable]
public class FightEntity
{
    public int id;
    public int count;
}

[System.Serializable]
public class Fight
{
    public int fight;
    public List<FightEntity> entities;
}

[System.Serializable]
public class Level
{
    public int level;
    public List<Fight> fights;
}

[System.Serializable]
public class LevelData
{
    public List<Entity> entities;
    public List<Level> levels;
}

[System.Serializable]
public class PlayerData
{
    public List<Entity> players;
}