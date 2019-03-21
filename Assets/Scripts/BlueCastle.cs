using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlueCastle : MonoBehaviour
{
    public Canvas baseUI;

    private GameObject baseMenu;
    private bool isRendered;
    private int currUnitIndex;

    public Image spawnRangeImage;
    public GameObject soldier;
    public GameObject[] unitRoster;

    public float castleHeight;
    public float castleDiameter;
    public float spawnRange;
    public float spawnCooldown;

    private float spawnTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        baseMenu = baseUI.transform.GetChild(0).gameObject;
        baseMenu.GetComponent<CanvasGroup>().alpha = 0.0f;
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
        if (spawnTimer >= spawnCooldown && isInSpawnRange(spawnLocation))
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

    public bool isInSpawnRange(Vector3 location)
    {
        float distFromCastle = (this.transform.position - location).magnitude;
        float spawnDist = spawnRange + castleDiameter / 2.0f;
        return (distFromCastle <= spawnDist);
    }

    public void toggleBaseMenu() {
        if(isRendered) {
            baseMenu.GetComponent<CanvasGroup>().alpha = 0.0f;
        } else {
            baseMenu.GetComponent<CanvasGroup>().alpha = 1.0f;
        }
        isRendered = !isRendered;
    }

    public void changeMenuLookAt(GameObject objToLookAt) {
        baseUI.transform.rotation = Quaternion.LookRotation(transform.position - objToLookAt.transform.position);
        //baseUI.transform.LookAt(objToLookAt.transform);
    }

    public void toggleCurrentUnit() {
        // 3 units just keep looping thru
        currUnitIndex = currUnitIndex + 1 % 3;
        soldier = unitRoster[currUnitIndex];
        baseMenu.transform.GetChild(currUnitIndex).GetComponent<Image>().color = Color.green;
    }
}
