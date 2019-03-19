using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    enum Command { Attacking, Defending, Gathering };

    public float soldierHeight;
    public float maxHP;
    public float aggroDist;
    public float dps;
    public float attackRange;
    public float moveSpeed;

    private Command currState = Command.Defending;
    private GameObject attackTarget;
    private GameObject gatherTarget;
    private Vector2 defendTarget;
    private float currHP;
    private float attackTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.localScale = new Vector3(this.transform.localScale.x, soldierHeight, this.transform.localScale.z);
        this.transform.position = new Vector3(this.transform.position.x, this.transform.localScale.y / 2.0f, this.transform.position.z);
        defendTarget = new Vector2(this.transform.position.x, this.transform.position.z);
        currHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer = Mathf.Max(attackTimer - Time.deltaTime, 0.0f);

        if (currState == Command.Attacking)
        {
            // TODO
        }
        else if (currState == Command.Defending)
        {
            Vector2 currPos = new Vector2(this.transform.position.x, this.transform.position.z);
            float distToTarget = (defendTarget - currPos).magnitude;
            if (distToTarget < moveSpeed) Move(defendTarget);
            else Move(currPos + (defendTarget - currPos).normalized * moveSpeed);
        }
        else if (currState == Command.Gathering)
        {
            // TODO
        }

        
    }

    void Move(Vector2 location)
    {
        this.transform.position = new Vector3(location.x, this.transform.position.y, location.y);
    }

    public void Defend(Vector3 location)
    {
        currState = Command.Defending;
        defendTarget = new Vector2(location.x, location.z);
    }
}
