using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject blueCastle;
    public GameObject redCastle;

    // Start is called before the first frame update
    void Start()
    {
        blueCastle.GetComponent<TeamColors>().SetBlueTeam();
        redCastle.GetComponent<TeamColors>().SetRedTeam();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
