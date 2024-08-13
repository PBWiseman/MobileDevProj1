using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemy : Entity
{
    public int monster_id; //The ID of the monster stats in the database
    public int experienceReward; // Used for monsters

    public Enemy(Entity entity, int copyNumber, int fight_id)
        : base(entity.name + " " + copyNumber, entity.speed, entity.maxHealth, entity.attack, entity.currentHealth)
    {
        this.fight_id = fight_id;
        this.monster_id = entity.monster_id;
        this.experienceReward = entity.experienceReward;
        this.isPlayer = false;
    }
}