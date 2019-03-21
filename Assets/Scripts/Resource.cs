using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public float maxSizeFactor;
    public float minSize;
    public float maxValue;

    private float currValue;
    private ResourceController controller;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<TeamColors>().SetYellowTeam();
        SetMaxValue(maxValue);
    }

    // Update is called once per frame
    void Update()
    {
        if (currValue == 0.0f)
        {
            controller.DespawnResource(this.gameObject);
        }
    }

    public void SetController(ResourceController controller)
    {
        this.controller = controller;
    }

    public void SetMaxValue(float value)
    {
        maxValue = value;
        this.transform.localScale = new Vector3(maxValue, maxValue, maxValue) * maxSizeFactor;
        this.transform.position = new Vector3(this.transform.position.x, maxValue * maxSizeFactor, this.transform.position.z);
        currValue = maxValue;
    }

    public float GetValue()
    {
        return currValue;
    }

    public float Gather(float gatherAmount)
    {
        float amountGathered = 0.0f;

        if (currValue >= gatherAmount)
        {
            currValue -= gatherAmount;
            amountGathered = gatherAmount;
        }
        else
        {
            amountGathered = currValue;
            currValue = 0.0f;
        }

        float newSize = minSize + (maxValue * maxSizeFactor - minSize) * (currValue / maxValue);
        this.transform.localScale = new Vector3(newSize, newSize, newSize);
        this.transform.position = new Vector3(this.transform.position.x, newSize, this.transform.position.z);

        return amountGathered;
    }
}
