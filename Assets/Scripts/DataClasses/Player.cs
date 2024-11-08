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
    public bool gameWon = false; //Flag to determine if the player has won the game

    public List<Attack> attacks;
    
    //Constructor
    public Player(Player player, int player_id, string name)
    : base(name, player.speed, player.maxHealth, player.attack, player.currentHealth, player.prefabDataPath)
    {
        this.player_id = player_id;
        this.totalExperience = player.totalExperience;
        this.level = player.level;
        this.isPlayer = true;
        this.prefab = Resources.Load<GameObject>(player.prefabDataPath);
        this.attacks = player.attacks;
    }

    //Copy constructor for saving to the file
    public Player(Player player) : base(player.name, player.speed, player.maxHealth, player.attack, player.currentHealth, player.prefabDataPath)
    {
        this.player_id = player.player_id;
        this.totalExperience = player.totalExperience;
        this.level = player.level;
        this.isPlayer = true;
        this.prefab = Resources.Load<GameObject>(player.prefabDataPath);
        this.currentLevel = player.currentLevel;
        this.currentFight = player.currentFight;
        this.prefabDataPath = player.prefabDataPath;
        this.isDead = player.isDead;
        this.gameWon = player.gameWon;
        this.attacks = player.attacks;
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
                TakeDamage(0); //Updates the health bar
                break;
            case 1:
                attack += Random.Range(3, 6);
                break;
            case 2:
                speed += Random.Range(3, 5);
                break;
        }
    }

    public void LoadPrefab()
    {
        prefab = Resources.Load<GameObject>(prefabDataPath);
    }

    public int attackDamage(int attack_id)
    {
        return attacks[attack_id].GetDamage() + attack;
    }
}