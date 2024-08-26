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
        CreateNewPlayer(); //TODO: Option to load or make new player
        startNewFight();
    }

    void Update()
    {
        if (GameOver)
        {
            return;
        }
        if (TurnManager.instance.currentState == FightStates.Win)
        {
            //TODO: Give player loot. Maybe health potions?
            player.totalExperience += LevelManager.instance.getFightXp(level, fight);
            //Check if the player has leveled up. Temp system until I decide on better xp values
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
                //End of the game?
                player.gameWon = true;
                Debug.Log("Player has won the game");
                SavePlayers();
                GameOver = true;
                return;
            }
            TurnManager.instance.currentState = FightStates.Continue;
            startNewFight();
        }
        else if (TurnManager.instance.currentState == FightStates.Lose)
        {
            Debug.Log("Player has died");
            SavePlayers();
            GameOver = true;
        }
    }

    private void CreateNewPlayer()
    {
        players = LoadPlayers();
        Player playerTemplate = LoadPlayerTemplate();
        //TODO: Customizable name
        string newName = "Bob";
        player = new Player(playerTemplate, players.Count, newName); //Set the player id to the next available id
        spawnPlayer(player);

        //Save back to the json file
        players.Add(player);
        SavePlayers();
    }

    private void spawnPlayer(Player player)
    {
        //Instantiate prefab from resources
        player.prefab = Instantiate(player.prefab, playerSpawnPoint.transform.position, Quaternion.identity);
        player.GameSetup();
    }

    private Player getPlayer(int player_id)
    {
        List<Player> players = LoadPlayers();
        Player player = players.Find(p => p.player_id == player_id);
        spawnPlayer(player);
        return player; //Note to self: This is the actual player entity from the list. It doesn't need to be saved back to it.
    }

    private void startNewFight()
    {
        player.currentLevel = level;
        player.currentFight = fight; //Set the player's current level and fight for saving
        SavePlayers();
        StartCoroutine(TurnManager.instance.MainTurnTracker(player, level, fight));
    }

    private List<Player> LoadPlayers()
    {
        string json;
        if (File.Exists(savePath))
        {
            json = File.ReadAllText(savePath);
            List<Player> players = JsonUtility.FromJson<PlayerData>(json).players;
        }
        else
        {
            players = new List<Player>();
        }
        return players;
    }

    private Player LoadPlayerTemplate()
    {
        string json = Resources.Load<TextAsset>("playerInfoTemplate").text;
        Player player = JsonUtility.FromJson<PlayerData>(json).players[0]; //There will only be one player in the template file
        return player;
    }

    private void SavePlayers()
    {
        PlayerData pd = new PlayerData();
        //Make a new list without some of the fields that don't need to be saved
        List<Player> playerSaves = new List<Player>();
        foreach (Player p in players)
        {
            Player player = new Player(p);
            player.prefab = null;
            player.healthBar = null;
            player.healthText = null;
            player.fight_id = -1;
            playerSaves.Add(player);
        }
        pd.players = playerSaves;
        string json = JsonUtility.ToJson(pd, true);
        File.WriteAllText(savePath, json);
    }
}
