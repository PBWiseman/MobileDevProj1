using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum FightStates //So the checkForEnd function doesn't need to deal with the end game state
{
    Win,
    Lose,
    Continue
}


public class TurnManager : MonoBehaviour
{
    public FightStates currentState;
    public static TurnManager instance;
    public bool running = false;
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

    public FightStates MainTurnTracker(Entity player, int level, int fight) //This is a list of all entities in the fight sorted by initiative
    {
        List<Entity> entities = startFight(level, fight);
        player.fight_id = entities.Count; //Give the player a new fight id for each fight
        entities.Add(player);
        sortByInitiative(entities);
        //Debug all entities with initiative and health
        foreach (Entity e in entities)
        {
            Debug.Log(e.name + " has " + e.initiative + " initiative and " + e.currentHealth + " health");
        }
        StartCoroutine(mainTurns(player, entities));
        do
        {
            //wait for the fight to end
        } while (!running); //Will continue until the fight is over
        return currentState;
    }

    private IEnumerator mainTurns(Entity player, List<Entity> entities)
    {
        running = true;
        int safety = 0;
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
                        Debug.Log(player.name + " attacks " + target.name + " for " + player.attack + " damage");
                        Debug.Log(target.name + " has " + target.currentHealth + " health remaining");
                        if (target.isDead)
                        {
                            //TODO: Remove sprite
                            Debug.Log(target.name + " has died");
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
                    player.TakeDamage(e.attack);
                    Debug.Log(e.name + " attacks " + player.name + " for " + e.attack + " damage");
                    Debug.Log(player.name + " has " + player.currentHealth + " health remaining");
                    if (player.isDead)
                    {
                        //TODO: Remove sprite
                        Debug.Log(player.name + " has died");
                    }
                }
                switch (checkForEnd(entities))
                {
                    case FightStates.Win:
                        currentState = FightStates.Win;
                        break;
                    case FightStates.Lose:
                        currentState = FightStates.Lose;
                        break;
                    case FightStates.Continue:
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
            Debug.Log("TurnManager safety reached 100. Ending game");
        }
        else
        {
            Debug.Log("Fight ended");
        }
        running = false;
    }

    private List<Entity> startFight(int level, int fight)
    {
        List<Entity> entities = LevelManager.instance.getEntities(level, fight);
        return entities;
    }

    private List<Entity> sortByInitiative(List<Entity> entities)
    {
        foreach (Entity e in entities)
        {
            e.RollInitiative();
        }
        entities.Sort((a, b) => a.initiative.CompareTo(b.initiative));
        return entities;
    }

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
