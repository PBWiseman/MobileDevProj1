using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemy : Entity
{
    public int monster_id; //The ID of the monster stats in the database
    public int experienceReward; // Used for monsters
    public string prefabDataPath; //The path to the prefab in the resources folder

    public Enemy(Enemy enemy, int copyNumber, int fight_id)
        : base(enemy.name + " " + copyNumber, enemy.speed, enemy.maxHealth, enemy.attack, enemy.currentHealth)
    {
        this.fight_id = fight_id;
        this.monster_id = enemy.monster_id;
        this.experienceReward = enemy.experienceReward;
        this.isPlayer = false;
        this.prefab = Resources.Load<GameObject>(enemy.prefabDataPath);
    }
}