using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player : Entity 
{
    public int player_id; //Unique ID for the player
    public int currentLevel; //The level the player is currently on
    public int currentFight; //The fight the player is currently in
    public int totalExperience; // Used for players
    public int level; // Used for players

    public Player(Player player, int player_id, string name)
    : base(name, player.speed, player.maxHealth, player.attack, player.currentHealth)
    {
        this.player_id = player_id;
        this.totalExperience = player.totalExperience;
        this.level = player.level;
        this.isPlayer = true;
    }

    public void LevelUp() //Temporary level up system.
    {
        level++;
        //TODO: Better system with player choice
        switch (Random.Range(0, 3))
        {
            case 0:
                int healthIncrease = Random.Range(20, 40);
                maxHealth += healthIncrease;
                currentHealth += healthIncrease;
                break;
            case 1:
                attack += Random.Range(5, 10);
                break;
            case 2:
                speed += Random.Range(3, 5);
                break;
        }
    }
}