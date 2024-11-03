using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemy : Entity
{
    public int monster_id; //The ID of the monster stats in the database
    public int experienceReward; // Used for monsters
    public int screenSlot; //The slot in the UI that the enemy is in
    public GameObject targetRing; //The target ring for the enemy
    SpriteRenderer sr;

    public Enemy(Enemy enemy, int copyNumber, int fight_id)
        : base(enemy.name + " " + copyNumber, enemy.speed, enemy.maxHealth, enemy.attack, enemy.currentHealth, enemy.prefabDataPath)
    {
        this.fight_id = fight_id;
        this.monster_id = enemy.monster_id;
        this.experienceReward = enemy.experienceReward;
        this.isPlayer = false;
        this.prefab = Resources.Load<GameObject>(enemy.prefabDataPath);
    }

    public void loadTargetRing()
    {
        if (targetRing == null)
        {
            targetRing = prefab.transform.Find("TargetAnchor/TargetCanvas").gameObject;
            sr = targetRing.GetComponent<SpriteRenderer>();
        }
    }

    public void toggleTarget()
    {
        if (targetRing == null)
        {
            targetRing = prefab.transform.Find("TargetAnchor/TargetCanvas").gameObject;
        }
        targetRing.SetActive(!targetRing.activeSelf);
    }

    public void toggleColor()
    {
        if (targetRing == null)
        {
            targetRing = prefab.transform.Find("TargetAnchor/TargetCanvas").gameObject;
        }
        if (sr == null)
        {
            sr = targetRing.GetComponentInChildren<SpriteRenderer>();
        }
        if (sr.color == Color.white)
        {
            sr.color = Color.red;
        }
        else
        {
            sr.color = Color.white;
        }
    }
}