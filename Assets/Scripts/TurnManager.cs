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

public enum Attacks
{
    Attack1,
    Attack2,
    Attack3,
    None
}

public class TurnManager : MonoBehaviour
{
    public FightStates currentState = FightStates.Continue;
    public static TurnManager instance;
    private Enemy selectedTarget;
    private Attacks selectedAttack;
    [SerializeField] private List<GameObject> spawnPoints;
    [SerializeField] private GameObject attackSelectionUI;
    [SerializeField] private GameObject targetSelectionUI;
    private List<Enemy> validTargets;
    
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
                    validTargets = entities.OfType<Enemy>().Where(e => !e.isPlayer && !e.isDead).ToList();
                    if (validTargets.Count > 0)
                    {
                        selectedTarget = null;
                        ShowTargetSelectionUI(validTargets);
                        yield return StartCoroutine(WaitForTargetSelection());
                        selectedAttack = Attacks.None;
                        ShowAttackSelectionUI();
                        yield return StartCoroutine(WaitForAttackSelection());
                        player.playAnimation(selectedAttack.ToString());
                        yield return Movement(e, 1);
                        yield return new WaitForSeconds(0.25f);
                        selectedTarget.TakeDamage(player.attack);
                        selectedTarget.toggleColor();
                        selectedTarget.toggleTarget();
                        yield return new WaitForSeconds(0.25f);
                        yield return Movement(e, -1);
                    }
                    else
                    {
                        Debug.Log("No valid targets available.");
                    }
                }
                else
                {
                    //Deal the entities attack damage to the player. If the player is dead remove them from the list
                    yield return Movement(e, -1);
                    e.playAnimation("Attack");
                    yield return new WaitForSeconds(0.4f);
                    player.TakeDamage(e.attack);
                    yield return new WaitForSeconds(0.25f);
                    yield return Movement(e, 1);
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

    private IEnumerator Movement(Entity e, float distance)
    {
        e.SetAnimState(1);
        float targetPos = e.prefab.transform.position.x + distance;
        float step;
        if (distance < 0)
        {
            step = -0.1f;
        }
        else
        {
            step = 0.1f;
        }

        while ((distance > 0 && e.prefab.transform.position.x < targetPos) ||
            (distance < 0 && e.prefab.transform.position.x > targetPos))
        {
            e.prefab.transform.position += new Vector3(step, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        e.SetAnimState(0);
    }

    /// <summary>
    /// Coroutine to handle the death of an entity
    /// </summary>
    /// <param name="e">The entity to kill</param>
    private IEnumerator EntityDeath(Entity e)
    {
        //yield return new WaitForSeconds(1);
        e.HideHealthBar();
        e.playAnimation("Death");
        yield return new WaitForSeconds(1);
        Destroy(e.prefab);
    }

    private IEnumerator WaitForAttackSelection()
    {
        while (selectedAttack == Attacks.None)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.25f);
    }

    private void ShowAttackSelectionUI()
    {
        attackSelectionUI.SetActive(true);
    }

    public void selectAttack(int attack)
    {
        selectedAttack = (Attacks)attack;
        attackSelectionUI.SetActive(false);
    }

    private IEnumerator WaitForTargetSelection()
    {
        while (selectedTarget == null)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.25f);
    }

    private void ShowTargetSelectionUI(List<Enemy> validTargets)
    {
        targetSelectionUI.SetActive(true);
        foreach (Enemy e in validTargets)
        {
            e.toggleTarget();
        }
    }

    public void selectTarget(int screenSlot)
    {
        //If there is a valid target enemy in the screen slot then select it. Otherwise do nothing.
        //It needs to look through a list of Entities for a screen slot but the screen slot is only in the enemy child class. It then needs to put the result into an enemy
        selectedTarget = validTargets.Find(e => e.screenSlot == screenSlot);
        if (selectedTarget != null)
        {
            targetSelectionUI.SetActive(false);
            foreach (Enemy e in validTargets)
            {
                if (e != selectedTarget)
                {
                    e.toggleTarget();
                }
                else
                {
                    e.toggleColor();
                }
            }
        }
    }

    /// <summary>
    /// Handles the menu selection of a target
    /// </summary>
    /// <param name="target">The target selected</param>
    private void OnTargetSelected(Enemy target)
    {
        selectedTarget = target;
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
            e.screenSlot = e.fight_id;
            e.GameSetup();
            //e.loadTargetRing();
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
