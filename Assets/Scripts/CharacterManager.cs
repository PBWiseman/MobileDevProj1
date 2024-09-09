using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;
    private string savePath => Application.persistentDataPath + "/playerInfo.json";
    private List<Player> players;
    public Player player;

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

    public void CreateNewPlayer(string name)
    {
        if (players == null)
        {
            players = LoadPlayers();
        }
        Player playerTemplate = LoadPlayerTemplate();
        player = new Player(playerTemplate, players.Count, name); //Set the player id to the next available id
        GameManager.instance.spawnPlayer(player);

        //Save back to the json file
        players.Add(player);
        SavePlayers();
    }
    
    public void LoadPlayer(int player_id)
    {
        if (players == null)
        {
            players = LoadPlayers();
        }
        Player player = players.Find(p => p.player_id == player_id);
        GameManager.instance.spawnPlayer(player);
    }

    public void DeletePlayer(int player_id)
    {
        if (players == null)
        {
            players = LoadPlayers();
        }
        Player player = players.Find(p => p.player_id == player_id);
        players.Remove(player);
        SavePlayers();
    }

    private List<Player> LoadPlayers()
    {
        string json;
        if (File.Exists(savePath))
        {
            json = File.ReadAllText(savePath);
            List<Player> players = JsonUtility.FromJson<PlayerData>(json).players;
            return players;
        }
        else
        {
            players = new List<Player>();
            return players;
        }
    }
    
    private Player LoadPlayerTemplate()
    {
        string json = Resources.Load<TextAsset>("playerInfoTemplate").text;
        Player player = JsonUtility.FromJson<PlayerData>(json).players[0]; //There will only be one player in the template file
        return player;
    }

    public void SavePlayers()
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
