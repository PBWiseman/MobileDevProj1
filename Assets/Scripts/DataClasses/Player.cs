using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player : Entity 
{
    public int player_id; //Unique ID for the player
    public int currentLevel; //The level the player is currently on
    public int currentFight; //The fight the player is currently in
    private int _totalExperience;
    public int totalExperience
    {
        get { return _totalExperience; }
        set
        {
            //Add the experience to the total experience and level up if needed
            _totalExperience += value;
            while (level < _totalExperience / 100) 
            {
                LevelUp();
            }
        }
    }
    public int level; // Used for players

    public Player(Entity entity, int player_id, string name)
    : base(name, entity.speed, entity.maxHealth, entity.attack, entity.currentHealth)
    {
        this.player_id = player_id;
        this.totalExperience = entity.totalExperience;
        this.level = entity.level;
        this.isPlayer = true;
    }

    private void LevelUp() //Temporary level up system.
    {
        level++;
        //TODO: Better system with player choice
        switch (Random.Range(0, 3))
        {
            case 0:
                int healthIncrease = Random.Range(5, 10);
                maxHealth += healthIncrease;
                currentHealth += healthIncrease;
                break;
            case 1:
                attack += Random.Range(1, 3);
                break;
            case 2:
                speed += Random.Range(1, 2);
                break;
        }
    }
}