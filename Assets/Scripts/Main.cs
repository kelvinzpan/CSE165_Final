using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject blueCastle;
    public GameObject redCastle;
    public GameObject playerController;

    // Start is called before the first frame update
    void Start()
    {
        blueCastle.GetComponent<TeamColors>().SetBlueTeam();
        redCastle.GetComponent<TeamColors>().SetRedTeam();

        // TESTING

        /* Soldier attack
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
    }

    // Update is called once per frame
    void Update()
    {
        /* Soldier movement
        playerController.GetComponent<Player>().DefendWithSelectedUnits(new Vector3(0.0f, 0.0f, 0.0f));
        */
    }
}
