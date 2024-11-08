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
    public List<Sprite> backgrounds;
    public GameObject background;
    public GameObject[] attackLabels;

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
    }

    void Update()
    {
        if (GameOver || TurnManager.instance.currentState == FightStates.Continue)
        {
            return;
        }
        if (TurnManager.instance.currentState == FightStates.Win)
        {
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
                //Set the background
                background.GetComponent<SpriteRenderer>().sprite = backgrounds[level - 1];
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
            StartNewFight();
        }
        else if (TurnManager.instance.currentState == FightStates.Lose)
        {
            Debug.Log("Player has died");
            CharacterManager.instance.SavePlayers();
            GameOver = true;
        }
    }

    public void spawnPlayer(Player inputPlayer)
    {
        //Instantiate prefab from resources
        //Debug all fields to make sure they are set correctly
        inputPlayer.LoadPrefab();
        inputPlayer.prefab = Instantiate(inputPlayer.prefab, playerSpawnPoint.transform.position, Quaternion.identity);
        inputPlayer.GameSetup();
        player = inputPlayer;
        if (player.currentLevel != 0)
        {
            level = player.currentLevel;
            fight = player.currentFight;
        }
        background.GetComponent<SpriteRenderer>().sprite = backgrounds[level - 1];
        StartNewFight();
    }

    public void StartNewFight()
    {
        player.currentLevel = level;
        player.currentFight = fight; //Set the player's current level and fight for saving
        setAttackLabels();
        CharacterManager.instance.SavePlayers();
        StartCoroutine(TurnManager.instance.MainTurnTracker(player, level, fight));
    }

    public void setAttackLabels()
    {
        for (int i = 0; i < player.attacks.Count; i++)
        {
            attackLabels[i].GetComponent<TextMeshProUGUI>().text = player.attacks[i].name + "    " + (player.attacks[i].minDamage + player.attack) + "-" + (player.attacks[i].maxDamage + player.attack) + " dmg";
        }
    }
}