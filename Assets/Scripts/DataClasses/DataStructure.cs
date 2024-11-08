using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Note: Entity has been moved to its own file due to its size

[System.Serializable]
public class FightEntity
{
    public int monster_id;
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
    public List<Enemy> enemies;
    public List<Level> levels;
}

[System.Serializable]
public class PlayerData
{
    public List<Player> players;
}

[System.Serializable]
public class Attack
{
    public int id;
    public string animation;
    public string name;
    public int minDamage;
    public int maxDamage;

    public Attack(int id, string name, int minDamage, int maxDamage)
    {
        this.id = id;
        this.name = name;
        this.minDamage = minDamage;
        this.maxDamage = maxDamage;
    }

    public int GetDamage()
    {
        //Add 1 to maxDamage to make it inclusive
        return Random.Range(minDamage, maxDamage+1);
    }
}