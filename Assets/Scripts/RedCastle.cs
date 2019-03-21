using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCastle : MonoBehaviour
{
    public GameObject soldier;
    public float castleHeight;
    public float castleDiameter;
    
    // Start is called before the first frame update
    void Start()
    {
        this.transform.localScale = new Vector3(castleDiameter, castleHeight, castleDiameter);
        this.transform.position = new Vector3(this.transform.position.x, castleHeight, this.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Soldier SpawnSoldier(Vector3 spawnLocation)
    {
        GameObject newSoldier = GameObject.Instantiate(soldier);
        newSoldier.transform.position = spawnLocation;
        newSoldier.GetComponent<TeamColors>().SetBlueTeam();
        return newSoldier.GetComponent<Soldier>();
    }
}
