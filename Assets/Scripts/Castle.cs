using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour
{
    public GameObject soldier;

    private float height;
    private float diameter;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetCastleSize(float height, float diameter)
    {
        this.height = height;
        this.diameter = diameter;

        this.transform.localScale = new Vector3(diameter, height, diameter);
        this.transform.position = new Vector3(this.transform.position.x, height, this.transform.position.z);
    }

    public Soldier SpawnSoldier(Vector3 spawnLocation)
    {
        GameObject newSoldier = GameObject.Instantiate(soldier);
        newSoldier.transform.position = spawnLocation;
        if (this.GetComponent<TeamColors>().IsBlueTeam()) newSoldier.GetComponent<TeamColors>().SetBlueTeam();
        else newSoldier.GetComponent<TeamColors>().SetRedTeam();
        return newSoldier.GetComponent<Soldier>();
    }
}
