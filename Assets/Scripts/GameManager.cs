using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int level = 1;
    private int fight = 1;
    public static GameManager instance;
    public Entity player;

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

    // Start is called before the first frame update
    void Start()
    {
        CreateNewPlayer();
        PlayGame();
        Debug.Log("Game Over");
    }
    //TODO: This overwrites the template and doesn't copy it. Fix this
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

    private Entity getPlayer(int id)
    {
        List<Entity> players = LoadPlayers();
        Entity player = players.Find(p => p.id == id);
        return player;
    }

    public void PlayGame()
    {
        do
        {
            FightStates fightResult = TurnManager.instance.MainTurnTracker(player, level, fight);
            if (fightResult == FightStates.Win)
            {
                //TODO: Give player loot. Maybe health potions?
                //Give player xp
                player.totalExperience += LevelManager.instance.getFightXp(level, fight);
                Debug.Log("Player has won the fight");
                Debug.Log("Player has " + player.totalExperience + " experience");
                Debug.Log("Player is level " + player.level);
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
                    Debug.Log("Player has won the game");
                    return;
                }
            }
            else //The method can only return win or lose, never continue
            {
                Debug.Log("Player has lost the fight");
                return;
            }
        } while (true); //While true because it will return when the game is over
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
