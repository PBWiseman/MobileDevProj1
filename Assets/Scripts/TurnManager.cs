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
                //Set this entity to always be rendered on top
                SpriteRenderer r = e.prefab.GetComponentInChildren<SpriteRenderer>();
                r.sortingOrder = 2;
                if (e.isPlayer)
                {
                    //Player turn
                    List<Entity> validTargets = entities.Where(e => !e.isPlayer && !e.isDead).ToList();
                    if (validTargets.Count > 0)
                    {
                        // Select a random target from the filtered list
                        //TODO: Change to player target selection
                        Entity target = validTargets[Random.Range(0, validTargets.Count)];
                        float startingPos = e.prefab.transform.position.x;
                        player.SetAnimState(1);
                        while (e.prefab.transform.position.x < startingPos + 1)
                        {
                            e.prefab.transform.position += new Vector3(0.1f, 0, 0);
                            yield return new WaitForSeconds(0.01f);
                        }
                        player.SetAnimState(0);
                        player.playAnimation("Attack1");
                        yield return new WaitForSeconds(0.25f);
                        target.TakeDamage(player.attack);
                        target.playAnimation("Hurt");
                        yield return new WaitForSeconds(0.25f);
                        player.SetAnimState(1);
                        while (e.prefab.transform.position.x > startingPos)
                        {
                            e.prefab.transform.position -= new Vector3(0.1f, 0, 0);
                            yield return new WaitForSeconds(0.01f);
                        }
                        player.SetAnimState(0);
                        if (target.isDead)
                        {
                            //Remove sprite
                            target.playAnimation("Death");
                            yield return new WaitForSeconds(1);
                            Enemy enemyTarget = (Enemy)target;
                            Destroy(enemyTarget.prefab);
                        }
                    }
                    else
                    {
                        Debug.Log("No valid targets available.");
                    }
                }
                else
                {
                    //Deal the entities attack damage to the player. If the player is dead remove them from the list
                    float startingPos = e.prefab.transform.position.x;
                    e.SetAnimState(1);
                    while (e.prefab.transform.position.x > startingPos - 1)
                    {
                        e.prefab.transform.position -= new Vector3(0.1f, 0, 0);
                        yield return new WaitForSeconds(0.01f);
                    }
                    e.SetAnimState(0);
                    e.playAnimation("Attack");
                    yield return new WaitForSeconds(0.4f);
                    player.TakeDamage(e.attack);
                    player.playAnimation("Hurt");
                    yield return new WaitForSeconds(0.25f);
                    e.SetAnimState(1);
                    while (e.prefab.transform.position.x < startingPos)
                    {
                        e.prefab.transform.position += new Vector3(0.1f, 0, 0);
                        yield return new WaitForSeconds(0.01f);
                    }
                    e.SetAnimState(0);
                    if (player.isDead)
                    {
                        player.playAnimation("Death");
                        //TODO: Remove sprite
                        Debug.Log(player.name + " has died");
                    }
                }
                r.sortingOrder = 1;
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
                if (currentState != FightStates.Continue)
                {
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
            e.GameSetup();
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
