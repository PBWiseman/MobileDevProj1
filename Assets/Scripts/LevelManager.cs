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
    }

    private void loadLevelData()
    {
        string json = Resources.Load<TextAsset>("levels").text;
        levelData = JsonUtility.FromJson<LevelData>(json);
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

    public List<Entity> sortByInitiative(List<Entity> entities)
    {
        foreach (Entity e in entities)
        {
            e.RollInitiative();
        }
        entities.Sort((a, b) => a.initiative.CompareTo(b.initiative));
        return entities;
    }
}