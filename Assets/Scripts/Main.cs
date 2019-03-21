using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject blueCastle;
    public GameObject redCastle;
    public GameObject playerController;
    public GameObject resourceController;

    private bool runOnce = true;
    private GameObject test;

    // Start is called before the first frame update
    void Start()
    {
        blueCastle.GetComponent<TeamColors>().SetBlueTeam();
        redCastle.GetComponent<TeamColors>().SetRedTeam();

        // TESTING

        /* Soldier defense (auto-attack)
        redCastle.GetComponent<RedCastle>().SpawnSoldier(new Vector3(0.0f, 0.0f, 0.0f));
        blueCastle.GetComponent<BlueCastle>().SpawnSoldier(new Vector3(14.0f, 0.0f, 0.0f));
        */

        /* Soldier movement
        for (int i = 0; i < 17; i++)
        {
            Soldier soldier = blueCastle.GetComponent<BlueCastle>().SpawnSoldier(new Vector3(i * 5.0f, 0.0f, 0.0f));
            playerController.GetComponent<Player>().selectUnit(soldier.gameObject);
        }
        */

        /* Soldier attack 
        test = redCastle.GetComponent<RedCastle>().SpawnSoldier(new Vector3(0.0f, 0.0f, 0.0f)).gameObject;
        for (int i = 0; i < 3; i++)
        {
            Soldier soldier = blueCastle.GetComponent<BlueCastle>().SpawnSoldier(new Vector3(50.0f, 0.0f, i * 5.0f));
            playerController.GetComponent<Player>().selectUnit(soldier.gameObject);
        }
        */

        /* Soldier gather 
        for (int i = 0; i < 3; i++)
        {
            Soldier soldier = blueCastle.GetComponent<BlueCastle>().SpawnSoldier(new Vector3(50.0f, 0.0f, i * 5.0f));
            playerController.GetComponent<Player>().selectUnit(soldier.gameObject);
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        if (runOnce)
        {
            runOnce = false;

            // TESTING

            /* Soldier movement
            playerController.GetComponent<Player>().DefendWithSelectedUnits(new Vector3(0.0f, 0.0f, 0.0f));
            */

            /* Soldier attack 
            playerController.GetComponent<Player>().AttackWithSelectedUnits(test);
            */

            /* Soldier gather 
            playerController.GetComponent<Player>().GatherWithSelectedUnits(resourceController.GetComponent<ResourceController>().SpawnResource());
            */
        }
    }
}
