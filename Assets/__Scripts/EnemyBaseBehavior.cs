using UnityEngine;
using System.Collections;
using System;

public class EnemyBaseBehavior : MonoBehaviour {
    public enum EnemyState { dead = 0, normal, sleeping, alert, attacking };
    public enum AttackTarget { none = 0, scientist, swarm };

    //Properties
    public float MaxHP = 10;
    public float DefenseMult = 1.0f;
    public float MovementSpeed = .5f;
    public float RotationSpeed = .25f;
    public GameObject[] patrolPath;
    public int patrolIndex = 0;
    public float minArriveDistance = 0.0f;
    public float sightRange = 5f;
    public float sightAngle = 45f;
    public float visionPeriphal = 5f;

    //enemy status
    public bool __Status_____________;
    public EnemyState currState = EnemyState.normal;
    public AttackTarget currTarget = AttackTarget.none;
    public Vector3 currTargetPos = Vector3.zero;
    public float currHP;
    public float currWaitTime = 0f;

    NavMeshAgent navagn;
    protected Rigidbody body;
    Vector3 nextPoint;
    float nextWaitTime = 0f;
    protected Vector3 visionVector = Vector3.zero;
    protected Vector3 visionPos = Vector3.zero;

	// Use this for initialization
	void Start () {
        currHP = MaxHP;
        body = GetComponent<Rigidbody>();
        navagn = GetComponent<NavMeshAgent>();

        BaseClassStart();

        if (patrolPath.Length > 0)
        {
            SetNext(patrolPath[patrolIndex]);
        }
	}
	
	// Update is called once per frame
	void Update () {
        //gather enviroment data
        getVisionVals();
        Debug.DrawRay(visionPos, visionVector.normalized * sightRange, Color.red);
        BaseClassUpdate();

        Awareness();

        //state machine logic
        if(currState == EnemyState.normal)
        {
            if (patrolPath.Length > 0)
            {
                Patrol();
            } else
            {
                navagn.updateRotation = false;
                navagn.Stop();
            }
            if(currTarget != AttackTarget.none)
            {
                currState = EnemyState.attacking;
            }
        } else if(currState == EnemyState.attacking)
        {
            navagn.Stop();
            navagn.updateRotation = false;
            Attack();
            if (currTarget == AttackTarget.none)
            {
                currState = EnemyState.normal;
            }
        }
	}

    //move to functions are using unity navmesh agent pathfinding
    public void SetNext(GameObject point)
    {
        SetNext(point.transform.position);
        nextWaitTime = point.GetComponent<PatrolPoint>().waitTime;
    }

    public void SetNext(Vector3 point)
    {
        nextPoint = point;
    }

    //returns true if arrived at point
    public bool ArrivedAt(Vector3 point)
    {
        //ignore z and y axis
        point.z = body.transform.position.z;
        point.y = body.transform.position.y;
        return Vector3.Distance(body.transform.position, point) <= minArriveDistance;
    }
    public bool ArrivedAt(GameObject point)
    {
        return ArrivedAt(point.transform.position);
    }

    public void Patrol()
    {
        if(currWaitTime > 0)
        {
            navagn.updateRotation = false;
            navagn.Stop();     

            currWaitTime -= Time.deltaTime;
        }
        else
        {
            if (ArrivedAt(nextPoint))
            {
                currWaitTime = nextWaitTime;
                patrolIndex = (patrolIndex + 1) % patrolPath.Length;
                SetNext(patrolPath[patrolIndex]);

                navagn.updateRotation = false;
                navagn.Stop();
            } else
            {
                Vector3 targetDir = new Vector3(nextPoint.x, body.transform.position.y, body.transform.position.z) - transform.position;
                if (Vector3.Angle(body.transform.forward, targetDir) > 10f)
                {
                    body.transform.rotation = Quaternion.LookRotation(Vector3.Slerp(body.transform.forward, targetDir, RotationSpeed));
                }
                else
                {
                    navagn.Resume();
                    navagn.updateRotation = true;
                    navagn.destination = nextPoint;
                }
            }
        }
    }

    /* DAMAGE TAKEN AND COLLISION */
    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Swarm")
        {
            TakeHit();
            Invoke("TakeHit", 1f);
        }
        else {
            // ... other collider logic if you need it
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.tag == "Swarm")
        {
            CancelInvoke("TakeHit");
        }
        else {
            // ... other collider logic if you need it
        }
    }

    void TakeHit()
    {
        currHP -= Swarm.S.attackPower;
        if (currHP <= 0)
        {
            // ... die
        }
    }

    //returns true if swarm is in range
    public bool SwarmInRange(float range)
    {
        try
        {
            return (Vector3.Distance(Swarm.S.transform.position, visionPos) <= range);
        } catch(NullReferenceException)
        {
            return false;
        }
        
    }
    
    public bool SwarmInSight(float range, float visionAngle)
    {
        try
        {
            Vector3 targetDir = Swarm.S.transform.position - visionPos;
            Vector3 targetDirX = new Vector3(targetDir.x, 0, 0);
            //Debug.DrawRay(body.transform.position, targetDir, Color.blue);
            //Debug.DrawRay(body.transform.position, targetDirX, Color.cyan);
            return SwarmInRange(range) && (Vector3.Angle(targetDir, visionVector) <= visionAngle)
                                       && (Vector3.Angle(targetDirX, visionVector) <= visionPeriphal);
        } catch(NullReferenceException)
        {
            return false;
        }
    }

    public Vector3 getScientistCenterPos()
    {
        return Scientist.S.transform.position + new Vector3(0f, 1f, 0f);
    }

    public bool ScientistInRange(float range)
    {
        try
        {
            return (Vector3.Distance(getScientistCenterPos(), visionPos) <= range);
        }
        catch (NullReferenceException)
        {
            print("Warning, scientist doesn't exist in this scene!");
            return false;
        }
    }

    public bool ScientistInSight(float range, float visionAngle)
    {
        try
        {
            Vector3 targetDir = getScientistCenterPos() - visionPos;
            Vector3 targetDirX = new Vector3(targetDir.x, 0, 0);
            //Debug.DrawRay(visionPos, targetDir, Color.blue);
            //Debug.DrawRay(visionPos, targetDirX, Color.cyan);
            return ScientistInRange(range) && (Vector3.Angle(targetDir, visionVector) <= visionAngle)
                                       && (Vector3.Angle(targetDirX, visionVector) <= visionPeriphal);
        }
        catch (NullReferenceException)
        {
            print("Warning, scientist doesn't exist in this scene!");
            return false;
        }
    }

    /* VIRTUAL FUNCTIONS */

    //this function decides how the enemy detects the player, either scientist or swarm
    //sets 
    public virtual void Awareness()
    {
        if (ScientistInSight(sightRange, sightAngle))
        {
            currTarget = AttackTarget.scientist;
            currTargetPos = getScientistCenterPos();
            Debug.DrawRay(Scientist.S.transform.position, Vector3.up * 3f, Color.green);
        }
        else if(SwarmInSight(sightRange, sightAngle))
        {
            currTarget = AttackTarget.swarm;
            currTargetPos = Swarm.S.transform.position;
            Debug.DrawRay(Swarm.S.transform.position, Vector3.up * 3f, Color.green);
        }
        else
        {
            currTarget = AttackTarget.none;
            currTargetPos = Vector3.zero;
        }
    }

    public virtual void Attack()
    {
        //override this function for each enemy
    }

    public virtual void BaseClassStart()
    {
        //override this function if something needs to be added to start
    }

    public virtual void BaseClassUpdate()
    {
        //override this function if something needs to be in update
    }


    //function return the vector of the enemies line of sight
    public virtual void getVisionVals()
    {
        visionVector = body.transform.forward;
        visionPos = body.transform.position;
    }
}
