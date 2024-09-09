using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private int level = 1;
    private int fight = 1;
    public static GameManager instance;
    public Player player;
    public List<Player> players;
    private bool GameOver = false; //TODO: Implement game over state
    private string savePath => Application.persistentDataPath + "/playerInfo.json";
    public GameObject playerSpawnPoint;

    void Awake()
    {
        Application.targetFrameRate = 60;
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = CharacterManager.instance.player;
        startNewFight();
    }

    void Update()
    {
        if (GameOver || TurnManager.instance.currentState == FightStates.Continue)
        {
            return;
        }
        if (TurnManager.instance.currentState == FightStates.Win)
        {
            //TODO: Give player loot. Maybe health potions?
            player.totalExperience += LevelManager.instance.getFightXp(level, fight);
            //Check if the player has leveled up
            while (player.totalExperience >= player.level * 100)
            {
                player.LevelUp();
            }
            Debug.Log($"{player.name} | lvl{player.level} | {player.totalExperience}xp");
            //Increase fight number unless it is the last fight in the level
            //In that case increase the level
            //If the last level is beaten, the player wins
            if (fight < LevelManager.instance.GetLevel(level).fights.Count)
            {
                fight++;
            }
            else if (level < LevelManager.instance.levelData.levels.Count)
            {
                level++;
                fight = 1;
            }
            else
            {
                //End of the game
                player.gameWon = true;
                Debug.Log("Player has won the game");
                CharacterManager.instance.SavePlayers();
                GameOver = true;
                return;
            }
            CharacterManager.instance.SavePlayers();
            TurnManager.instance.currentState = FightStates.Continue;
            startNewFight();
        }
        else if (TurnManager.instance.currentState == FightStates.Lose)
        {
            Debug.Log("Player has died");
            CharacterManager.instance.SavePlayers();
            GameOver = true;
        }
    }

    public void spawnPlayer(Player player)
    {
        //Instantiate prefab from resources
        player.prefab = Instantiate(player.prefab, playerSpawnPoint.transform.position, Quaternion.identity);
        player.GameSetup();
    }

    private void startNewFight()
    {
        player.currentLevel = level;
        player.currentFight = fight; //Set the player's current level and fight for saving
        CharacterManager.instance.SavePlayers();
        StartCoroutine(TurnManager.instance.MainTurnTracker(player, level, fight));
    }
}