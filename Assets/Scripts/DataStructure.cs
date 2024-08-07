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
    public int totalExperience; // Used for players
    public int level; // Used for players

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