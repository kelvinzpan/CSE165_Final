using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    enum Command { Attacking, Defending, Gathering, Fleeing };

    public GameObject aggroBox;
    public float soldierHeight;
    public float maxHP;
    public float aggroDist;
    public float attackDamage;
    public float attackRange;
    public float attackSpeed;
    public float moveSpeed;

    private Command currState = Command.Defending;
    private GameObject attackTarget;
    private GameObject gatherTarget;
    private Vector2 defendTarget;
    private GameObject currAggroBox;
    private GameObject defendAggroTarget;
    private Vector2 fleeTarget;
    private float currHP;
    private float attackTimer = 0.0f;

    const string LAYER_CASTLE = "Castle";
    const string LAYER_SOLDIER = "Soldier";
    const string LAYER_FLOOR = "Floor";

    // Start is called before the first frame update
    void Start()
    {
        this.transform.localScale = new Vector3(this.transform.localScale.x, soldierHeight, this.transform.localScale.z);
        this.transform.position = new Vector3(this.transform.position.x, this.transform.localScale.y / 2.0f, this.transform.position.z);

        Defend(this.transform.position);
        currHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer = Mathf.Max(attackTimer - Time.deltaTime, 0.0f);

        if (currState == Command.Attacking)
        {
            if (attackTarget && !isSameTeam(attackTarget))
            {
                Engage(attackTarget);
            }
            else
            {
                Defend(this.transform.position);
            }
        }
        else if (currState == Command.Defending)
        {
            if (defendAggroTarget)
            {
                if (isInAggroRange(defendAggroTarget))
                {
                    Engage(defendAggroTarget);
                }
                else
                {
                    defendAggroTarget = null;
                }
            }
            else
            {
                MoveTowards(defendTarget);

                List<Collider> colliders = currAggroBox.GetComponent<OverlapBox>().GetColliders();
                foreach (Collider collider in colliders)
                {
                    if (collider)
                    {
                        GameObject currObj = collider.gameObject;

                        if (!isSameTeam(currObj))
                        {
                            defendAggroTarget = currObj;
                            break;
                        }
                    }
                }
            }
        }
        else if (currState == Command.Gathering)
        {
            // TODO
        }
        else if (currState == Command.Fleeing)
        {
            if (isAtLocation(fleeTarget))
            {
                Defend(this.transform.position);
            }
            else
            {
                MoveTowards(fleeTarget);
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currHP = Mathf.Max(currHP - damageAmount, 0.0f);

        // Get shorter with lower HP
        float newSoldierHeight = (soldierHeight / 2.0f) * (1.0f + currHP / maxHP);
        this.transform.localScale = new Vector3(this.transform.localScale.x, newSoldierHeight, this.transform.localScale.z);

        if (currHP <= 0.0f)
        {
            Die();
        }
    }

    public void Attack(GameObject target)
    {
        ClearCommandState();
        currState = Command.Attacking;
        attackTarget = target;
    }

    public void Defend(Vector3 location)
    {
        ClearCommandState();
        currState = Command.Defending;
        defendTarget = new Vector2(location.x, location.z);
        currAggroBox = Instantiate(aggroBox);
        currAggroBox.transform.localScale = new Vector3(aggroDist, currAggroBox.transform.localScale.y, aggroDist);
        currAggroBox.transform.position = new Vector3(defendTarget.x, 0.0f, defendTarget.y);
    }

    public void Flee(Vector3 location)
    {
        ClearCommandState();
        currState = Command.Fleeing;
        fleeTarget = new Vector2(location.x, location.z);
    }

    private void ClearCommandState()
    {
        // Attack state
        if (attackTarget) attackTarget = null;

        // Defend state
        if (currAggroBox) Destroy(currAggroBox);
        if (defendAggroTarget) defendAggroTarget = null;

        // Gathering state

        // Flee state
        
    }

    private void MoveTowards(Vector2 location)
    {
        Vector2 currPos = new Vector2(this.transform.position.x, this.transform.position.z);
        float distToLocation = (location - currPos).magnitude;

        if (distToLocation < moveSpeed)
        {
            this.transform.position = new Vector3(location.x, this.transform.position.y, location.y);
        }
        else
        {
            Vector2 newPos = currPos + (location - currPos).normalized * moveSpeed;
            this.transform.position = new Vector3(newPos.x, this.transform.position.y, newPos.y);
        }
    }

    private void Engage(GameObject target)
    {
        if (isInAttackRange(target))
        {
            if (attackTimer <= 0.0f)
            {
                // TODO Play attack animation

                if (target.layer == LayerMask.NameToLayer(LAYER_SOLDIER))
                {
                    target.GetComponent<Soldier>().TakeDamage(attackDamage);
                }
                else if (target.layer == LayerMask.NameToLayer(LAYER_CASTLE))
                {
                    // TODO Attack base
                }

                attackTimer = attackSpeed;
            }
        }
        else
        {
            Vector2 targetPos = new Vector2(target.transform.position.x, target.transform.position.z);
            MoveTowards(targetPos);
        }
    }

    private void Die()
    {
        Destroy(this.gameObject);
    }

    private bool isInAggroRange(GameObject target)
    {
        Vector2 targetPos = new Vector2(target.transform.position.x, target.transform.position.z);
        return (targetPos - defendTarget).magnitude <= aggroDist;
    }

    private bool isInAttackRange(GameObject target)
    {
        Vector2 targetPos = new Vector2(target.transform.position.x, target.transform.position.z);
        Vector2 currPos = new Vector2(this.transform.position.x, this.transform.position.z);
        return (targetPos - currPos).magnitude <= attackRange;
    }

    private bool isAtLocation(Vector2 location)
    {
        Vector2 currPos = new Vector2(this.transform.position.x, this.transform.position.z);
        return currPos == location;
    }

    private bool isSameTeam(GameObject target)
    {
        return this.GetComponent<TeamColors>().IsSameTeam(target.GetComponent<TeamColors>());
    }
}
