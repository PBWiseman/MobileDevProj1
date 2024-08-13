using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public LevelData levelData;

    void Awake()
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
            Enemy enemyTemplate = levelData.enemies.Find(e => e.monster_id == fe.monster_id); // Cross reference the entity ID to the enemy list.
            if (enemyTemplate != null)
            {
                for (int i = 0; i < fe.count; i++)
                {
                    Enemy newEnemy = new Enemy(enemyTemplate, i + 1, entities.Count); //Passes through the enemy template, the count of the enemy, and a unique fight id
                    entities.Add(newEnemy);
                }
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

    public Level GetLevel(int level)
    {
        return levelData.levels.FirstOrDefault(l => l.level == level);
    }

    public Fight GetFight(Level level, int fight)
    {
        return level.fights.FirstOrDefault(f => f.fight == fight);
    }
}