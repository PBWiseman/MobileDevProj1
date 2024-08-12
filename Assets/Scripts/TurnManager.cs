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

    public FightStates MainTurnTracker(Entity player, int level, int fight) //This is a list of all entities in the fight sorted by initiative
    {
        List<Entity> entities = startFight(level, fight);
        entities.Add(player);
        sortByInitiative(entities);
        do
        {
            foreach (Entity e in entities)
            {
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
                    if (target.isDead)
                    {
                        entities.Remove(target);
                    }
                }
                else
                {
                    //Deal the entities attack damage to the player. If the player is dead remove them from the list
                    player.TakeDamage(e.attack);
                    if (player.isDead)
                    {
                        entities.Remove(player);
                    }
                }
                switch (checkForEnd(entities))
                {
                    case FightStates.Win:
                        return FightStates.Win;
                    case FightStates.Lose:
                        return FightStates.Lose;
                    case FightStates.Continue:
                        //Fight continues
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
        //If the player is dead than start the end game process
        //If all enemies are dead then return true
        //else return false
        if (entities.FindAll(entity => entity.isPlayer).Count == 0)
        {
            //Player is dead
            return FightStates.Lose;
        }
        if (entities.FindAll(entity => !entity.isPlayer).Count == 0)
        {
            //All enemies are dead
            return FightStates.Win;
        }
        return FightStates.Continue;
    }
}
