using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlueCastle : MonoBehaviour
{
    public Image spawnRangeImage;
    public GameObject soldier;
    public float castleHeight;
    public float castleDiameter;
    public float spawnRange;
    public float spawnCooldown;

    private float spawnTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.localScale = new Vector3(castleDiameter, castleHeight, castleDiameter);
        this.transform.position = new Vector3(this.transform.position.x, castleHeight, this.transform.position.z);

        spawnRangeImage.transform.localScale = new Vector3(castleDiameter + spawnRange * 2, castleDiameter + spawnRange * 2, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer = Mathf.Min(spawnTimer + Time.deltaTime, spawnCooldown);
        spawnRangeImage.fillAmount = (spawnTimer / spawnCooldown);
    }

    public void SpawnSoldier(Vector3 spawnLocation)
    {
        if (spawnTimer >= spawnCooldown)
        {
            GameObject newSoldier = GameObject.Instantiate(soldier);
            newSoldier.transform.position = spawnLocation;
            newSoldier.GetComponent<TeamColors>().SetBlueTeam();

            spawnTimer = 0.0f;
        }
    }

    public void ShowSpawnRange()
    {
        spawnRangeImage.gameObject.SetActive(true);
    }

    public void HideSpawnRange()
    {
        spawnRangeImage.gameObject.SetActive(false);
    }
}
