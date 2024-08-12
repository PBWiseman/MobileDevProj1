using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    //The code for getting levels and fights was many nested foreach loops.
    //I asked GitHub Copilot if there was a way to make it more efficient and readable and it suggested using LINQ
    public List<Entity> getEntities(int level, int fight)
    {
        Level selectedLevel = GetLevel(level);
        if (selectedLevel == null)
        {
            return new List<Entity>(); //Return empty if null
        }

        Fight selectedFight = GetFight(selectedLevel, fight);
        if (selectedFight == null)
        {
            return new List<Entity>(); //Return empty if null
        }

        List<Entity> entities = new List<Entity>();
        foreach (FightEntity fe in selectedFight.entities)
        {
            Entity entity = levelData.entities.Find(e => e.id == fe.id); //Cross reference the entity ID to the entities list.
            if (entity != null)
            {
                entities.AddRange(Enumerable.Repeat(entity, fe.count)); //Add the amount of enemies in the fight
            }
        }

        return entities;
    }

    //Get the xp from a fight
    public int getFightXp(int level, int fight)
    {
        Level selectedLevel = GetLevel(level);
        if (selectedLevel == null)
        {
            return 0; //Return 0 if null
        }

        Fight selectedFight = GetFight(selectedLevel, fight);
        if (selectedFight == null)
        {
            return 0; //Return 0 if null
        }

        int xp = 0;
        foreach (FightEntity fe in selectedFight.entities)
        {
            Entity entity = levelData.entities.Find(e => e.id == fe.id); //Cross reference the entity ID to the entities list.
            if (entity != null)
            {
                xp += entity.experienceReward * fe.count;   //Add the xp times the amount of that enemy in the fight
            }
        }

        return xp;
    }

    private Level GetLevel(int level)
    {
        return levelData.levels.FirstOrDefault(l => l.level == level);
    }

    private Fight GetFight(Level level, int fight)
    {
        return level.fights.FirstOrDefault(f => f.fight == fight);
    }
}