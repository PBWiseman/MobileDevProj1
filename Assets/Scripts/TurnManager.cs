using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                    //Deal the players attack damage to a randomly selected enemy that is not the player (will be changed to selection later). If the entity is dead remove them from the list
                    //Ugly rerolling code but it will be changed to player target later so I am fine with it
                    
                    //TODO: Change to player target selection
                    Entity target = entities[Random.Range(0, entities.Count)];
                    while (target.isPlayer)
                    {
                        target = entities[Random.Range(0, entities.Count)];
                    }
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
                        System.Threading.Thread.Sleep(5000);
                        break;
                }
            }
        } while (true); //When the fight is over the function will return so this can be a while true
    }

    private List<Entity> startFight(int level, int fight)
    {
        List<Entity> entities = LevelManager.instance.getEntities(level, fight);
        foreach (Entity e in entities)
        {
            e.Initialize();
        }
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
        bool enemiesDead = false;

        foreach (Entity e in entities)
        {
            if (entity.isPlayer && entity.isDead) //If player is dead return as a loss right away
            {
                return FightStates.Lose;
            }
            if (!entity.isPlayer && !entity.isDead) //If any enemy is alive then the player hasn't won
            {
                allEnemiesAreDead = false;
            }
        }
        if (allEnemiesAreDead)
        {
            return FightStates.Win;
        }
        return FightStates.Continue;
    }
}
