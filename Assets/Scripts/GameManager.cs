using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int level = 1;
    private int fight = 1;
    public static GameManager instance;
    public Entity player;
    // Start is called before the first frame update
    void Start()
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

    public void CreateNewPlayer()
    {
        //Load the player in id slot 0 from the playerInfo json file and then save it back as a new player with the next available id
        List<Entity> players = LoadPlayers();
        player = players[0]; //Loads from the template player in slot 0
        player.id = players.Count; //Set the player id to the next available id
        player.name = "Bob"; //TODO: Customizable name
        player.Initialize();
        //Save back to the json file
        players.Add(player);
        SavePlayers(players);
    }

    public void PlayGame()
    {
        FightStates fightResult = TurnManager.instance.MainTurnTracker(player, level, fight);
        if (fightResult == FightStates.Win)
        {
            //TODO: Give player loot. Maybe health potions?
            //Give player xp
            player.totalExperience += LevelManager.instance.getFightXp(level, fight);
        }
        else //The method can only return win or lose, never continue
        {
            //TODO:Player loses
        }
    }

    public List<Entity> LoadPlayers()
    {
        string json = Resources.Load<TextAsset>("playerInfo").text;
        List<Entity> players = JsonUtility.FromJson<PlayerData>(json).players;
        return players;
    }

    public void SavePlayers(List<Entity> players)
    {
        PlayerData pd = new PlayerData();
        pd.players = players;
        string json = JsonUtility.ToJson(pd);
        System.IO.File.WriteAllText(Application.dataPath + "/Resources/playerInfo.json", json);
    }
}
