using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    public GameObject resource;
    public int maxResources;
    public int initResources;
    public float spawnSpeed;
    public float maxValue;
    public float minValue;
    public float zSpawnRange;
    public float xSpawnRange;

    private int currResources;
    private float spawnTimer;

    private HashSet<GameObject> resources = new HashSet<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < initResources; i++) SpawnResource();
        currResources = initResources;
        spawnTimer = spawnSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer = Mathf.Max(spawnTimer - Time.deltaTime, 0.0f);

        if (spawnTimer == 0.0f && currResources < maxResources)
        {
            SpawnResource();
            spawnTimer = spawnSpeed;
        }
    }

    public void DespawnResource(GameObject resource)
    {
        resources.Remove(resource);
        Destroy(resource);
    }

    public GameObject SpawnResource()
    {
        GameObject newRes = Instantiate(resource);
        newRes.GetComponent<Resource>().SetController(this);

        float xSpawn = Random.Range(-xSpawnRange, xSpawnRange);
        float zSpawn = Random.Range(-zSpawnRange, zSpawnRange);
        newRes.transform.position = new Vector3(xSpawn, newRes.transform.position.y, zSpawn);

        float value = Mathf.Ceil(Random.Range(minValue, maxValue));
        newRes.GetComponent<Resource>().SetMaxValue(value);

        resources.Add(newRes);
        return newRes;
    }

    public HashSet<GameObject> GetResourceList()
    {
        return resources;
    }
}
