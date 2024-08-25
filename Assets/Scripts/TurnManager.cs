using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public enum FightStates //So the checkForEnd function doesn't need to deal with the end game state
{
    Win,
    Lose,
    Continue
}


public class TurnManager : MonoBehaviour
{
    public FightStates currentState = FightStates.Continue;
    public static TurnManager instance;
    [SerializeField] private List<GameObject> spawnPoints;
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

    public IEnumerator MainTurnTracker(Entity player, int level, int fight) //This is a list of all entities in the fight sorted by initiative
    {
        List<Entity> entities = startFight(level, fight);
        player.fight_id = entities.Count; //Give the player a new fight id for each fight
        entities.Add(player);
        sortByInitiative(entities);
        int safety = 0;
        currentState = FightStates.Continue;
        yield return new WaitForSeconds(2); //Delay after spawn but before the first turn
        do
        {
            foreach (Entity e in entities)
            {
                if (e.isDead)
                {
                    continue;
                }
                if (e.isPlayer)
                {
                    playerTurn(e, entities);
                }
                else
                {
                    enemyTurn(e, player);
                }
                switch (checkForEnd(entities))
                {
                    case FightStates.Win:
                        yield return new WaitForSeconds(2); //Delay before the next fight in place of end turn stuff
                        currentState = FightStates.Win;
                        break;
                    case FightStates.Lose:
                        yield return new WaitForSeconds(2); //Delay before the next fight in place of end turn stuff
                        currentState = FightStates.Lose;
                        break;
                    case FightStates.Continue:
                        currentState = FightStates.Continue;
                        yield return new WaitForSeconds(1); //Delay between turns
                        //Fight continues.
                        break;
                }
            }
            safety++;
        } while (safety < 100 && currentState == FightStates.Continue);
        if (safety >= 100)
        {
            currentState = FightStates.Lose; //If you run for 100 turns without a win or loss then you lose. Can change this later if there is a valid case for that many turns
            Debug.LogWarning("TurnManager safety reached 100. Ending fight");
        }
        else
        {
            Debug.Log("Fight ended");
        }

    }


    private void playerTurn(Entity player, List<Entity> entities)
    {
        //Player turn
        //Deal the players attack damage to a randomly selected enemy that is not the player (will be changed to selection later)

        //TODO: Change to player target selection
        List<Entity> validTargets = entities.Where(e => !e.isPlayer && !e.isDead).ToList();

        // Check if there are any valid targets
        if (validTargets.Count > 0)
        {
            // Select a random target from the filtered list
            Entity target = validTargets[Random.Range(0, validTargets.Count)];
            target.TakeDamage(player.attack);
            if (target.isDead)
            {
                //Remove sprite
                Enemy enemyTarget = (Enemy)target;
                Destroy(enemyTarget.prefab);
            }
        }
        else
        {
            Debug.Log("No valid targets available.");
        }
    }

    private void enemyTurn(Entity e, Entity player)
    {
        //Deal the entities attack damage to the player. If the player is dead remove them from the list
        player.TakeDamage(e.attack);
        if (player.isDead)
        {
            //TODO: Remove sprite
            Debug.Log(player.name + " has died");
        }
    }

    /// <summary>
    /// Gets the enemies for a given level and fight number
    /// Also spawns the enemies at the spawn points
    /// </summary>
    /// <param name="level">The level to start the fight in</param>
    /// <param name="fight">The fight number to start</param>
    private List<Entity> startFight(int level, int fight)
    {
        List<Enemy> enemies = LevelManager.instance.getEntities(level, fight);
        List<Entity> entities = new List<Entity>();
        foreach (Enemy e in enemies)
        {
            if (spawnPoints.Count < e.fight_id + 1)
            {
                Debug.LogError("Not enough spawn points for all entities");
                break;
            }
            // Spawn the enemy at successive spawn points
            e.prefab = Instantiate(e.prefab, spawnPoints[e.fight_id].transform.position, Quaternion.identity);
            e.healthBar = e.prefab.GetComponentInChildren<Slider>();
            e.healthText = e.healthBar.GetComponentInChildren<TextMeshProUGUI>();
            e.TakeDamage(0); //Telling it to take 0 damage to update the health bar
            entities.Add(e);
        }
        return entities;
    }
    

    /// <summary>
    /// Sorts the entities by initiative
    /// </summary>
    /// <param name="entities">The list of entities to sort</param>
    private List<Entity> sortByInitiative(List<Entity> entities)
    {
        foreach (Entity e in entities)
        {
            e.RollInitiative();
        }
        entities.Sort((a, b) => a.initiative.CompareTo(b.initiative));
        return entities;
    }

    /// <summary>
    /// Checks if the fight has ended
    /// </summary>
    /// <param name="entities">The list of entities in the fight</param>
    /// <returns>Win, Lose, or Continue</returns>
    private FightStates checkForEnd(List<Entity> entities)
    {
        bool enemiesDead = true;

        foreach (Entity e in entities)
        {
            if (e.isPlayer && e.isDead) //If player is dead return as a loss right away
            {
                return FightStates.Lose;
            }
            if (!e.isPlayer && !e.isDead) //If any enemy is alive then the player hasn't won
            {
                enemiesDead = false;
            }
        }
        if (enemiesDead)
        {
            return FightStates.Win;
        }
        return FightStates.Continue;
    }
}
