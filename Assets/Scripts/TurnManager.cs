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
    public static TurnManager instance;
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
        int safety = 0; //Safety to prevent infinite loops
        List<Entity> entities = startFight(level, fight);
        player.fight_id = entities.Count; //Give the player a new fight id for each fight
        entities.Add(player);
        sortByInitiative(entities);
        //Debug all entities with initiative and health
        foreach (Entity e in entities)
        {
            Debug.Log(e.name + " has " + e.initiative + " initiative and " + e.currentHealth + " health");
        }
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
                    //Ugly rerolling code but it will be changed to player target later so I am fine with it
                    
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
                        return FightStates.Win;
                    case FightStates.Lose:
                        return FightStates.Lose;
                    case FightStates.Continue:
                        //Fight continues. Possibly have a delay here or something so it doesn't speed through?
                        break;
                }
            }
            safety++;
        } while (true && safety < 100); //When the fight is over the function will return so this can be a while true
        Debug.Log("TurnManager safety reached 100. Ending game");
        return FightStates.Lose;
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
