using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    public int level;
    public List<Fight> fights;
}

[System.Serializable]
public class Fight
{
    public int fight;
    public List<Entity> entities;
}

[System.Serializable]
public class Entity
{
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
public class LevelData
{
    public List<Level> levels;
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public LevelData levelData;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        loadLevelData();
        //Debug out the level data
        foreach (Level l in levelData.levels)
        {
            Debug.Log("Level: " + l.level);
            foreach (Fight f in l.fights)
            {
                startFight(l.level, f.fight);
                Debug.Log("Fight: " + f.fight);
                foreach (Entity e in f.entities)
                {
                    Debug.Log("Entity: " + e.name);
                    Debug.Log("Speed: " + e.speed);
                    Debug.Log("Max Health: " + e.maxHealth);
                    Debug.Log("Current Health: " + e.currentHealth);
                    Debug.Log("Attack: " + e.attack);
                }
            }
        }
    }

    private void loadLevelData()
    {
        string json = Resources.Load<TextAsset>("levels").text;
        levelData = JsonUtility.FromJson<LevelData>(json);
    }

    public void startFight(int level, int fight)
    {
        List<Entity> entities = getEntities(level, fight);
        foreach (Entity e in entities)
        {
            e.Initialize();
        }
        // Add player to the list of entities when I make it
        entities = sortByInitiative(entities);
    }

    public List<Entity> getEntities(int level, int fight)
    {
        List<Entity> entities = new List<Entity>();
        foreach (Level l in levelData.levels)
        {
            if (l.level == level)
            {
                foreach (Fight f in l.fights)
                {
                    if (f.fight == fight)
                    {
                        entities = f.entities;
                        return entities; // End the loops and return the list of entities if found
                    }
                }
            }
        }
        return entities; // Return an empty list if no entities are found
    }

    private List<Entity> sortByInitiative(List<Entity> entities)
    {
        foreach (Entity e in entities)
        {
            e.RollInitiative();
        }
        entities.Sort((a, b) => a.initiative.CompareTo(b.initiative));
        return entities;
    }
}
