using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCastle : MonoBehaviour
{
    public GameObject soldier;
    public float castleHeight;
    public float castleDiameter;
    public float maxHP;

    public float spawnRange;
    public float spawnCooldown;

    private float currHP;
    private float spawnTimer = 0.0f;

    private bool useShittyAI = false;
    private bool justSpawned = false;
    private Soldier prevSoldier;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.localScale = new Vector3(castleDiameter, castleHeight, castleDiameter);
        this.transform.position = new Vector3(this.transform.position.x, castleHeight, this.transform.position.z);
        currHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer = Mathf.Min(spawnTimer + Time.deltaTime, spawnCooldown);

        if (useShittyAI)
        {
            if (justSpawned)
            {
                Vector3 range = new Vector3(Random.Range(-50.0f, 50.0f),
                                            0.0f,
                                            Random.Range(-20.0f, 20.0f));
                prevSoldier.Defend(range);
                justSpawned = false;
            }

            if (spawnTimer == spawnCooldown)
            {
                Vector3 range = new Vector3(Random.Range(-spawnRange, spawnRange),
                                            Random.Range(-spawnRange, spawnRange),
                                            Random.Range(-spawnRange, spawnRange));
                prevSoldier = SpawnSoldierInRange(this.transform.position + range);
                justSpawned = true;
            }
        }
    }

    public void UseShittyAI()
    {
        useShittyAI = true;
    }

    public Soldier SpawnSoldier(Vector3 spawnLocation)
    {
        GameObject newSoldier = GameObject.Instantiate(soldier);
        newSoldier.transform.position = spawnLocation;
        newSoldier.GetComponent<TeamColors>().SetRedTeam();
        return newSoldier.GetComponent<Soldier>();
    }

    public Soldier SpawnSoldierInRange(Vector3 spawnLocation)
    {
        if (spawnTimer >= spawnCooldown && isInSpawnRange(spawnLocation))
        {
            GameObject newSoldier = GameObject.Instantiate(soldier);
            newSoldier.transform.position = spawnLocation;
            newSoldier.GetComponent<TeamColors>().SetRedTeam();

            spawnTimer = 0.0f;
            return newSoldier.GetComponent<Soldier>();
        }
        else
        {
            return null;
        }
    }

    public void TakeDamage(float damage)
    {
        currHP = Mathf.Max(currHP - damage, 0.0f);

        // Get shorter with lower HP
        float newHeight = (castleHeight / 2.0f) * (1.0f + currHP / maxHP);
        this.transform.localScale = new Vector3(this.transform.localScale.x, newHeight, this.transform.localScale.z);

        if (currHP <= 0.0f)
        {
            Die();
        }
    }

    public void Die()
    {
        // Win the game 
        Debug.Log("Congrats, you won the game!");
    }

    public bool isInSpawnRange(Vector3 location)
    {
        float distFromCastle = (this.transform.position - location).magnitude;
        float spawnDist = spawnRange + castleDiameter / 2.0f;
        return (distFromCastle <= spawnDist);
    }
}
