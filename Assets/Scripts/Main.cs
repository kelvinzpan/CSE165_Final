using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject blueCastle;
    public GameObject redCastle;
    public GameObject playerController;
    public GameObject resourceController;
    public int runTestNum;

    private bool runOnce = true;
    private GameObject test;

    // Start is called before the first frame update
    void Start()
    {
        blueCastle.GetComponent<TeamColors>().SetBlueTeam();
        redCastle.GetComponent<TeamColors>().SetRedTeam();

        // TESTING

        /* Soldier defense (auto-attack) */
        if (runTestNum == 1)
        {
            redCastle.GetComponent<RedCastle>().SpawnSoldier(new Vector3(0.0f, 0.0f, 0.0f));
            blueCastle.GetComponent<BlueCastle>().SpawnSoldier(new Vector3(14.0f, 0.0f, 0.0f));
        }
        /* Soldier movement */
        else if (runTestNum == 2)
        {
            for (int i = 0; i < 17; i++)
            {
                Soldier soldier = blueCastle.GetComponent<BlueCastle>().SpawnSoldier(new Vector3(i * 5.0f, 0.0f, 0.0f));
                playerController.GetComponent<Player>().selectUnit(soldier.gameObject);
            }
        }
        /* Soldier attack */
        else if (runTestNum == 3)
        {
            test = redCastle.GetComponent<RedCastle>().SpawnSoldier(new Vector3(0.0f, 0.0f, 0.0f)).gameObject;
            for (int i = 0; i < 3; i++)
            {
                Soldier soldier = blueCastle.GetComponent<BlueCastle>().SpawnSoldier(new Vector3(50.0f, 0.0f, i * 5.0f));
                playerController.GetComponent<Player>().selectUnit(soldier.gameObject);
            }
        }
        /* Soldier gather */
        else if (runTestNum == 4)
        {
            for (int i = 0; i < 3; i++)
            {
                Soldier soldier = blueCastle.GetComponent<BlueCastle>().SpawnSoldier(new Vector3(50.0f, 0.0f, i * 5.0f));
                playerController.GetComponent<Player>().selectUnit(soldier.gameObject);
            }
        }
        /* Meteor attack */
        else if (runTestNum == 5)
        {
            for (int i = -20; i < 20; i++)
            {
                redCastle.GetComponent<RedCastle>().SpawnSoldier(new Vector3(-2.0f, 0.0f, i * 5.0f));
            }
        }
        /* Attack base */
        else if (runTestNum == 6)
        {
            for (int i = 0; i < 3; i++)
            {
                Soldier soldier = blueCastle.GetComponent<BlueCastle>().SpawnSoldier(new Vector3(50.0f, 0.0f, i * 5.0f));
                playerController.GetComponent<Player>().selectUnit(soldier.gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (runOnce)
        {
            runOnce = false;

            // TESTING

            /* Soldier movement */
            if (runTestNum == 2)
            {
                playerController.GetComponent<Player>().DefendWithSelectedUnits(new Vector3(0.0f, 0.0f, 0.0f));
            }
            /* Soldier attack */
            else if (runTestNum == 3)
            {
                playerController.GetComponent<Player>().AttackWithSelectedUnits(test);
            }
            /* Soldier gather */
            else if (runTestNum == 4)
            {
                playerController.GetComponent<Player>().GatherWithSelectedUnits(resourceController.GetComponent<ResourceController>().SpawnResource());
            }
            /* Meteor attack */
            else if (runTestNum == 5)
            {
                blueCastle.GetComponent<BlueCastle>().SummonMeteor(new Vector2(0.0f, -50.0f), 0.0f);
                blueCastle.GetComponent<BlueCastle>().SummonMeteor(new Vector2(0.0f, 50.0f), 1.0f);
            }
            /* Attack base */
            else if (runTestNum == 6)
            {
                playerController.GetComponent<Player>().AttackWithSelectedUnits(redCastle);
            }
        }

        if (runTestNum == 0)
        {
            redCastle.GetComponent<RedCastle>().UseShittyAI();
        }
    }
}
