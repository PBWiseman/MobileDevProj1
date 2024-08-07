using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public LevelData levelData;

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
                foreach (FightEntity fe in f.entities)
                {
                    Entity e = levelData.entities.Find(entity => entity.id == fe.id);
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
        //Add player to the list of entities when I make it
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
                        foreach (FightEntity fe in f.entities)
                        {
                            Entity e = levelData.entities.Find(entity => entity.id == fe.id);
                            for (int i = 0; i < fe.count; i++)
                            {
                                entities.Add(e);
                            }
                        }
                        return entities;
                    }
                }
            }
        }
        return entities; //Return an empty list if not found
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