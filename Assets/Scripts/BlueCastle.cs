using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlueCastle : MonoBehaviour
{
    public Canvas baseUI;

    private GameObject baseMenu;
    private GameObject fireBallSlider;
    private bool isRendered;
    private int currUnitIndex = -1;

    public Image spawnRangeImage;
    public GameObject soldier;
    public GameObject[] unitRoster;

    public float castleHeight;
    public float castleDiameter;
    public float maxHP;
    public float maxResource;

    public float spawnRange;
    public float spawnCooldown;

    private float currHP;
    private float currResource = 0.0f;
    private float spawnTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        baseMenu = GameObject.Find("Base Menu");
        fireBallSlider = GameObject.Find("Base Menu/Slider");

        baseMenu.GetComponent<CanvasGroup>().alpha = 0.0f;
        this.transform.localScale = new Vector3(castleDiameter, castleHeight, castleDiameter);
        this.transform.position = new Vector3(this.transform.position.x, castleHeight, this.transform.position.z);
        currHP = maxHP;

        spawnRangeImage.transform.localScale = new Vector3(castleDiameter + spawnRange * 2, castleDiameter + spawnRange * 2, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer = Mathf.Min(spawnTimer + Time.deltaTime, spawnCooldown);
        spawnRangeImage.fillAmount = (spawnTimer / spawnCooldown);
    }

    public Soldier SpawnSoldierInRange(Vector3 spawnLocation)
    {
        if (spawnTimer >= spawnCooldown && isInSpawnRange(spawnLocation))
        {
            GameObject newSoldier = GameObject.Instantiate(soldier);
            newSoldier.transform.position = spawnLocation;
            newSoldier.GetComponent<TeamColors>().SetBlueTeam();

            spawnTimer = 0.0f;
            return newSoldier.GetComponent<Soldier>();
        }
        else
        {
            return null;
        }
    }

    public Soldier SpawnSoldier(Vector3 spawnLocation)
    {
        GameObject newSoldier = GameObject.Instantiate(soldier);
        newSoldier.transform.position = spawnLocation;
        newSoldier.GetComponent<TeamColors>().SetBlueTeam();
        return newSoldier.GetComponent<Soldier>();
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

    public void AddResource(float resource)
    {
        currResource = Mathf.Min(currResource + resource, maxResource);
    }

    public void Die()
    {
        // Lose the game
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

    public void toggleCurrentUnit() {
        // First time no unit is select4ed
        if(currUnitIndex != -1) {
            baseMenu.transform.GetChild(currUnitIndex).GetComponent<Image>().color = new Color(50.0f, 255.0f, 0.0f);
        }
        currUnitIndex = (currUnitIndex + 1) % 3;
        soldier = unitRoster[currUnitIndex];
        baseMenu.transform.GetChild(currUnitIndex).GetComponent<Image>().color = Color.green;
    }

    public void changeSliderValue(float value) {
        fireBallSlider.GetComponent<Slider>().value += value;
    }
}
