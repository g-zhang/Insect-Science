using UnityEngine;
using System.Collections;

public class EnemyBaseBehavior : MonoBehaviour {
    public enum EnemyState { dead = 0, normal, sleeping, alert, attacking }

    //Properties
    public int MaxHP = 10;
    public float DefenseMult = 1.0f;
    public float MovementSpeed = .5f;
    public float RotationSpeed = .25f;
    public GameObject[] patrolPath;
    public int patrolIndex = 0;
    public float minArriveDistance = 0.0f;
    public float sightRange = 5f;
    public float sightAngle = 45f;

    //enemy status
    public bool __Status_____________;
    public EnemyState currState = EnemyState.normal;
    public int currHP;
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
        getVisionVals();
        Debug.DrawRay(visionPos, visionVector.normalized * sightRange, Color.red);
        BaseClassUpdate();

        Debug.DrawRay(Swarm.S.transform.position, Vector3.up * 3f, Color.green);

        if(currState == EnemyState.normal)
        {
            if (patrolPath.Length > 0)
            {
                Patrol();
            }
            if (SwarmInSight(sightRange, sightAngle))
            {
                currState = EnemyState.attacking;
            }
        } else if(currState == EnemyState.attacking)
        {
            //navagn.enabled = false;
            navagn.Stop();
            Attack();
            if (!SwarmInSight(sightRange, sightAngle))
            {
                currState = EnemyState.normal;
            }
        }
	}


    //returns true if scientist is in range
    public bool SwarmInRange(float range)
    {
        return (Vector3.Distance(Swarm.S.transform.position, visionPos) <= range);
    }

    public bool SwarmInSight(float range, float visionAngle)
    {
        Vector3 targetDir = Swarm.S.transform.position - visionPos;
        Vector3 targetDirX = new Vector3(targetDir.x, 0, 0);
        //Debug.DrawRay(body.transform.position, targetDir, Color.blue);
        //Debug.DrawRay(body.transform.position, targetDirX, Color.cyan);
        return SwarmInRange(range) && (Vector3.Angle(targetDir, visionVector) <= visionAngle)
                                   && (Vector3.Angle(targetDirX, visionVector) <= 5f);
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
            
            //navagn.enabled = false;

            currWaitTime -= Time.deltaTime;
        }
        else
        {
            navagn.Resume();
            navagn.updateRotation = true;
            //navagn.enabled = true;
            if (ArrivedAt(nextPoint))
            {
                currWaitTime = nextWaitTime;
                patrolIndex = (patrolIndex + 1) % patrolPath.Length;
                SetNext(patrolPath[patrolIndex]);
            } else
            {
                Vector3 targetDir = new Vector3(nextPoint.x, body.transform.position.y, body.transform.position.z) - transform.position;
                if (Vector3.Angle(body.transform.forward, targetDir) > 10f)
                {
                    body.transform.rotation = Quaternion.LookRotation(Vector3.Slerp(body.transform.forward, targetDir, RotationSpeed));
                }
                else
                {
                    navagn.destination = nextPoint;
                }
            }
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
