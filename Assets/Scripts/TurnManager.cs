using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Entity> startFight(int level, int fight)
    {
        List<Entity> entities = getEntities(level, fight);
        foreach (Entity e in entities)
        {
            e.Initialize();
        }
        //Add player to the list of entities when I make it
        entities = sortByInitiative(entities);
        return entities;
    }


    public void MainTurnTracker(List<Entity> entities) //This is a list of all entities in the fight sorted by initiative
    {
        foreach (Entity e in entities)
        {
            if (e.isPlayer)
            {
                //Player turn
            }
            else
            {
                //Enemy turn
            }
        }
    }
}
