using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public float gatherAmount;

    private Command currState = Command.Defending;
    private GameObject attackTarget;
    private Vector2 defendTarget;
    private GameObject currAggroBox;
    private GameObject defendAggroTarget;
    private GameObject gatherTarget;
    private GameObject gatherBase;
    private float amountGathered = 0.0f;
    private Vector2 fleeTarget;
    private float currHP;
    private float attackTimer = 0.0f;

    public AudioClip spawn;
    public AudioClip defend;
    public AudioClip defend2;
    public AudioClip fight1;
    public AudioClip fight2;
    public AudioClip die;
    public AudioClip attack;
    public AudioClip movement;
    
    public int soundCount = 8;
    private AudioClip[] allClips;
    private AudioSource[] allSources;

    public Image hpCircle;

    const string LAYER_CASTLE = "Castle";
    const string LAYER_SOLDIER = "Soldier";
    const string LAYER_FLOOR = "Floor";

    void Awake() {
        AudioClip[] clips = { spawn, defend, defend2, fight1, fight2, die, attack, movement};
        allClips = clips;
        attachAudioSource();
        allSources = gameObject.GetComponents<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        allSources[0].Play();
        this.transform.localScale = new Vector3(this.transform.localScale.x, soldierHeight, this.transform.localScale.z);
        this.transform.position = new Vector3(this.transform.position.x, this.transform.localScale.y / 2.0f, this.transform.position.z);

        Defend(this.transform.position);
        currHP = maxHP;
        hpCircle.gameObject.SetActive(false);
    }

    void attachAudioSource() {
        for(int i = 0; i < soundCount; i++) {

            AudioSource audio = gameObject.AddComponent<AudioSource>() as AudioSource;
            audio.clip = allClips[i];
            audio.spatialize = true;
            audio.spatializePostEffects = true;
            //makes it 3d
            audio.spatialBlend = 1.0f;
            audio.rolloffMode = AudioRolloffMode.Linear;
            audio.minDistance = 0.03f;
            audio.maxDistance = 100.0f;
            if(i == 0) {
                audio.playOnAwake = true;
                audio.priority = (int)Random.Range(50.0f, 200.0f);
            } else {
                audio.playOnAwake = false;
            }

        }
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
            if (amountGathered > 0.0f)
            {
                if (isInGatherRange(gatherBase))
                {
                    gatherBase.GetComponent<BlueCastle>().AddResource(amountGathered);
                    amountGathered = 0.0f;
                }
                else
                {
                    Vector2 gatherPos = new Vector2(gatherBase.transform.position.x, gatherBase.transform.position.z);
                    MoveTowards(gatherPos);
                }
            }
            else
            {
                if (gatherTarget)
                {
                    if (isInGatherRange(gatherTarget))
                    {
                        amountGathered = gatherTarget.GetComponent<Resource>().Gather(gatherAmount);
                    }
                    else
                    {
                        Vector2 gatherPos = new Vector2(gatherTarget.transform.position.x, gatherTarget.transform.position.z);
                        MoveTowards(gatherPos);
                    }
                }
                else
                {
                    Defend(this.transform.position);
                }
            }
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

        hpCircle.gameObject.SetActive(true);
        hpCircle.fillAmount = currHP / maxHP;

        if (currHP <= 0.0f)
        {
            Die();
        }
    }

    public void Attack(GameObject target)
    {
        allSources[2].Play();
        ClearCommandState();
        currState = Command.Attacking;
        attackTarget = target;
    }

    public void Defend(Vector3 location)
    {
        if(!allSources[0].isPlaying) {
            int random = (int)Random.Range(1.0f, 2.99f);
            allSources[random].Play();
        }
            
        ClearCommandState();
        currState = Command.Defending;
        defendTarget = new Vector2(location.x, location.z);
        currAggroBox = Instantiate(aggroBox);
        currAggroBox.transform.localScale = new Vector3(aggroDist, currAggroBox.transform.localScale.y, aggroDist);
        currAggroBox.transform.position = new Vector3(defendTarget.x, 0.0f, defendTarget.y);
    }

    public void Gather(GameObject target, GameObject castle)
    {
        ClearCommandState();
        currState = Command.Gathering;
        gatherTarget = target;
        gatherBase = castle;
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

        // Clear fight sounds
        allSources[3].Stop();
        allSources[4].Stop();
        // Gathering state
        if (gatherTarget) gatherTarget = null;
        if (gatherBase) gatherBase = null;

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
                int indx = (int)Random.Range(3.0f, 4.99f);
                if(!allSources[indx].isPlaying) {
                    allSources[indx].Play();
                }
                // TODO Play attack animation

                if (target.layer == LayerMask.NameToLayer(LAYER_SOLDIER))
                {
                    target.GetComponent<Soldier>().TakeDamage(attackDamage);
                }
                else if (target.layer == LayerMask.NameToLayer(LAYER_CASTLE))
                {
                    if (target.GetComponent<TeamColors>().IsBlueTeam()) target.GetComponent<BlueCastle>().TakeDamage(attackDamage);
                    else target.GetComponent<RedCastle>().TakeDamage(attackDamage);
                }

                attackTimer = attackSpeed;
            }
        }
        else
        {
            allSources[3].Stop();
            allSources[4].Stop();
            Vector2 targetPos = new Vector2(target.transform.position.x, target.transform.position.z);
            MoveTowards(targetPos);
        }
    }

    private void Die()
    {
        allSources[3].Stop();
        allSources[4].Stop();
        allSources[5].Play();
        Destroy(currAggroBox);
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

    private bool isInGatherRange(GameObject target)
    {
        Vector2 targetPos = new Vector2(target.transform.position.x, target.transform.position.z);
        Vector2 currPos = new Vector2(this.transform.position.x, this.transform.position.z);
        float gatherRadius = target.transform.localScale.x / 2;
        return (targetPos - currPos).magnitude - gatherRadius <= attackRange;
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
