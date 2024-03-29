﻿using UnityEngine;
using System.Collections;
using System;
using UnityStandardAssets.Characters.ThirdPerson;

public class EnemyBaseBehavior : MonoBehaviour {
    public enum EnemyState { dead = 0, normal, sleeping, alert, attacking, swarmed, investigate };
    public enum AttackTarget { none = 0, scientist, swarm };

    //Properties
    public float MaxHP = 10;
    public float MovementSpeed = .5f;
    public float RotationSpeed = .25f;
    public GameObject[] patrolPath;
    public int patrolIndex = 0;
    public float minArriveDistance = 0.0f;
    public float sightRange = 5f;
    public float sightAngle = 45f;
    public float visionPeriphal = 5f;
    public float attackingStayTime = 2f; //time the guard will stay there after seeing the player
    public LineRenderer sightLineObj;
    public Material sightLineNormalMat;
    public Material sightLineInteractedMat;
    public Material sightLineAttackMat;

    [Header("Swarmed State Settings")]
    public float RunawayDistance = 10f;
    public float RunawayTime = 10f;
    public float InteractPopupTime = .1f;
    public bool ________________;
    public float currPopupTime = 0f;
    public bool isInteractable = false;

    [Header("Light Switch Settings")]
    public bool EnableLight = false;
    public float sightDampen = .5f; //multplier to the sightRange when lights are off
    public GameObject Light;
    public GameObject[] investigatePath;
    public int ipathIdx = 0;

    //enemy status
    [Header("Status")]
    public EnemyState currState = EnemyState.normal;
    public AttackTarget currTarget = AttackTarget.none;
    public Vector3 currTargetPos = Vector3.zero;
    public float currHP;
    public float currWaitTime = 0f;
    public Vector3 currRotation = Vector3.zero;
    public float currRunawayTime = 0f;

    NavMeshAgent navagn;
    protected Rigidbody body;
    protected Collider coll;
    Vector3 nextPoint;
    float nextWaitTime = 0f;
    Vector3 nextPointRotation = Vector3.zero;
    protected Vector3 visionVector = Vector3.zero;
    public Vector3 visionPos = Vector3.zero;
	SmallSwarm hoverSwarm;

    RigidbodyConstraints normalBody = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    RigidbodyConstraints enableZBody = RigidbodyConstraints.FreezeRotation;
    int sightLayerMask;

    // Use this for initialization
    void Start () {
		sightLayerMask = LayerMask.GetMask("Default", "Scientist", "Ground");

        currHP = MaxHP;
        body = GetComponent<Rigidbody>();
        navagn = GetComponent<NavMeshAgent>();
        coll = GetComponent<Collider>();

        BaseClassStart();

        if (patrolPath.Length > 0)
        {
            SetNext(patrolPath[patrolIndex]);
        }
    }

    void DrawSightLine()
    {
        float mult = 1f;
        if (EnableLight && !Light.GetComponent<RoomLight>().turnedOn)
        {
            mult = sightDampen;
        }
        float distance = sightRange * mult;
        Ray visionray = new Ray(visionPos, visionVector);
        RaycastHit hit;
        if (Physics.Raycast(visionray, out hit, sightRange, sightLayerMask, QueryTriggerInteraction.Ignore))
        {
            float tdistance = Vector3.Distance(hit.point, visionPos);
            if(tdistance < distance)
            {
                distance = tdistance; 
            }
        }

        sightLineObj.transform.localScale = new Vector3(sightLineObj.transform.localScale.x,
                                        sightLineObj.transform.localScale.y,
                                        distance);
    }
	
	// Update is called once per frame
	void Update () {
        //gather enviroment data
        getVisionVals();
        //Debug.DrawRay(visionPos, visionVector.normalized * sightRange, Color.red);
        DrawSightLine();

        BaseClassUpdate();

        Awareness();
        //OnInteractUpdate();

        //state machine logic
        if (currState == EnemyState.normal)
        {
            sightLineObj.GetComponent<Renderer>().sharedMaterial = sightLineNormalMat;
            if (patrolPath.Length > 0)
            {
                Patrol(patrolPath);
            } else
            {
                navagn.updateRotation = false;
                navagn.Stop();
            }
            if(EnableLight && !Light.GetComponent<RoomLight>().turnedOn)
            {
                currState = EnemyState.investigate;
                ipathIdx = 0;
                SetNext(investigatePath[ipathIdx]);
                navagn.destination = nextPoint;
                currWaitTime = 0;
            }
            if(currTarget != AttackTarget.none && currState == EnemyState.normal)
            {
                currState = EnemyState.attacking;
            }
        } else if(currState == EnemyState.attacking)
        {
            sightLineObj.GetComponent<Renderer>().sharedMaterial = sightLineAttackMat;
            navagn.Stop();
            navagn.updateRotation = false;
            Attack();
            if (currTarget == AttackTarget.none)
            {
                currState = EnemyState.normal;
            }
        } else if(currState == EnemyState.swarmed)
        {
            sightLineObj.GetComponent<Renderer>().sharedMaterial = sightLineInteractedMat;
            Patrol(patrolPath);
            if (currTarget != AttackTarget.none)
            {
                currState = EnemyState.attacking;
                SetNext(patrolPath[patrolIndex]);
                navagn.destination = nextPoint;
                currWaitTime = 0;
            }
        } else if(currState == EnemyState.investigate)
        {
            sightLineObj.GetComponent<Renderer>().sharedMaterial = sightLineInteractedMat;
            if (currTarget != AttackTarget.none)
            {
                currState = EnemyState.attacking;
            }
            if (Investigate(investigatePath, 1))
            {
                print("Guard: Investigation Completed!");
                currState = EnemyState.normal;
            }
        }
	}

    //move to functions are using unity navmesh agent pathfinding
    public void SetNext(GameObject point)
    {
        PatrolPoint npoint = point.GetComponent<PatrolPoint>();
        SetNext(point.transform.position);
        nextWaitTime = npoint.waitTime;
        if(npoint.directions.Length > 0)
        {
            nextPointRotation = npoint.directions[0];
        } else
        {
            nextPointRotation = Vector3.zero;
        }
    }

    public void SetNext(Vector3 point)
    {
        nextPoint = point;
    }

    //returns true if arrived at point
    public bool ArrivedAt(Vector3 point, bool axis=true)
    {
        //ignore z and y axis
        if(axis)
        {
            point.z = body.transform.position.z;
        }
        point.y = body.transform.position.y;
        return Vector3.Distance(body.transform.position, point) <= minArriveDistance;
    }
    public bool ArrivedAt(GameObject point, bool axis = true)
    {
        return ArrivedAt(point.transform.position, axis);
    }

    public void Patrol(GameObject[] Path)
    {
        if(currWaitTime > 0)
        {
            navagn.updateRotation = false;
            navagn.Stop();

            //ROTATE
            //print(body.transform.right);
            if(currRotation != Vector3.zero)
            {
                body.transform.rotation = Quaternion.LookRotation(Vector3.Slerp(body.transform.forward, currRotation, RotationSpeed));
            }     

            currWaitTime -= Time.deltaTime;
        }
        else
        {
            Debug.DrawRay(nextPoint, Vector3.up * 5f, Color.blue);


            body.constraints = normalBody;
            body.transform.position = new Vector3(body.transform.position.x, body.transform.position.y, nextPoint.z);

            if (ArrivedAt(nextPoint))
            {
                currWaitTime = nextWaitTime;
                currRotation = nextPointRotation;
                patrolIndex = (patrolIndex + 1) % Path.Length;
                SetNext(Path[patrolIndex]);

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

    //returns true after path is completed
    public bool Investigate(GameObject[] Path, int waitIdx)
    {
        if (currWaitTime > 0)
        {
            navagn.updateRotation = false;
            navagn.Stop();

            if (ipathIdx == waitIdx && !Light.GetComponent<RoomLight>().turnedOn)
            {

            } else
            {
                currWaitTime -= Time.deltaTime;
            } 
        }
        else
        {
            body.constraints = enableZBody;
            Debug.DrawRay(nextPoint, Vector3.up * 5f, Color.yellow);
            if (ArrivedAt(nextPoint, false))
            {
                navagn.updateRotation = false;
                navagn.Stop();

                currWaitTime = nextWaitTime;
                if(ipathIdx + 1 >= Path.Length)
                {
                    ipathIdx = 0;
                    return true;
                } else if(ipathIdx == waitIdx && !Light.GetComponent<RoomLight>().turnedOn)
                {
                    ipathIdx = waitIdx;
                } else
                {
                    ipathIdx = (ipathIdx + 1) % Path.Length;
                    SetNext(Path[ipathIdx]);
                }
            }
            else
            {
                Vector3 targetDir = new Vector3(nextPoint.x, body.transform.position.y, nextPoint.z) - transform.position;
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
        return false;
    }

    void Runaway()
    {
        Vector3 runawayPos = body.transform.position + new Vector3(body.transform.forward.x, 0, 0) * RunawayDistance;
        SetNext(runawayPos);
        nextWaitTime = RunawayTime;
        nextPointRotation = Vector3.zero;
        currWaitTime = 0f;
        navagn.destination = nextPoint;
	}

	public bool swarmed {
		get { return currState == EnemyState.swarmed; }
		set {
			if (value) {
                // start running away
                
                Runaway();
                currRunawayTime = RunawayTime;
                currState = EnemyState.swarmed;
            }
			else {
                // stop running away
                currState = EnemyState.normal;
                SetNext(patrolPath[patrolIndex]);
                navagn.destination = nextPoint;
                currWaitTime = 0;
            }
		}
	}

	///* Swarm interaction */
	//void OnTriggerEnter(Collider other)
 //   {
 //       Debug.DrawRay(other.gameObject.transform.position, Vector3.up * 5f, Color.green);
 //       print(other.gameObject.tag);
 //       if (other.gameObject.tag == "SmallSwarm")
	//	{
	//		Main.S.ShowInteractPopup(gameObject, "Press E to Swarm Guard");
	//		isInteractable = true;
 //           currPopupTime = InteractPopupTime;
	//		hoverSwarm = other.GetComponentInParent<SmallSwarm>();
 //       }
 //   }

 //   void OnTriggerStay(Collider other)
 //   {
 //       if (other.gameObject.tag == "SmallSwarm")
 //       {
 //           if (Main.S.interact && currState != EnemyState.swarmed)
 //           {
 //               Runaway();
 //               currRunawayTime = RunawayTime;
 //               currState = EnemyState.swarmed;
 //               hoverSwarm.Interact(gameObject);
 //               hoverSwarm = null;
 //           }
 //       }
 //   }

 //   void OnTriggerExit(Collider other)
 //   {
 //       if (other.gameObject.tag == "SmallSwarm")
 //       {
 //           Main.S.HideInteractPopup(gameObject);
 //           hoverSwarm = null;
 //           isInteractable = false;
 //       }
 //   }

   // void OnInteractUpdate()
   // {
   //     if(currPopupTime > 0)
   //     {
   //         float interact = Input.GetAxis("Interact");
   //         if (interact > 0 && currState != EnemyState.swarmed)
   //         {
   //             Runaway();
   //             currRunawayTime = RunawayTime;
   //             currState = EnemyState.swarmed;
			//	hoverSwarm.Interact(gameObject);
			//	hoverSwarm = null;
   //         }

   //         currPopupTime -= Time.deltaTime;
   //     } else
   //     {
   //         Main.S.HideInteractPopup(gameObject);
			//hoverSwarm = null;
   //     }
   // }

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
            bool lineOfSight = false;
            Ray visionray = new Ray(visionPos, targetDir);
            RaycastHit hit;
            if(Physics.Raycast(visionray, out hit, sightRange, sightLayerMask, QueryTriggerInteraction.Ignore))
            {
                if(hit.transform.tag == "Swarm")
                {
                    lineOfSight = true;
                }
            }
            //Debug.DrawRay(body.transform.position, targetDir, Color.blue);
            //Debug.DrawRay(body.transform.position, targetDirX, Color.cyan);
            return SwarmInRange(range) && (Vector3.Angle(targetDir, visionVector) <= visionAngle)
                                       && (Vector3.Angle(targetDirX, visionVector) <= visionPeriphal)
                                       && lineOfSight;
        } catch(NullReferenceException)
        {
            return false;
        }
    }

    public Vector3 getScientistCenterPos()
    {   if(Scientist.S.GetComponent<ThirdPersonCharacter>().m_Crouching)
        {
            return Scientist.S.transform.position + new Vector3(0f, .5f, 0f);
        } else
        {
            return Scientist.S.transform.position + new Vector3(0f, 1f, 0f);
        }  
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
            bool lineOfSight = false;
            Ray visionray = new Ray(visionPos, targetDir);
            RaycastHit hit;
            if (Physics.Raycast(visionray, out hit, sightRange, sightLayerMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.tag == "Player")
                {
                    lineOfSight = true;
                }
            }
            //Debug.DrawRay(visionPos, targetDir, Color.blue);
            //Debug.DrawRay(visionPos, targetDirX, Color.cyan);
            return ScientistInRange(range) && (Vector3.Angle(targetDir, visionVector) <= visionAngle)
                                       && (Vector3.Angle(targetDirX, visionVector) <= visionPeriphal)
                                       && lineOfSight;
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
        float mult = 1f;
        if(EnableLight && !Light.GetComponent<RoomLight>().turnedOn)
        {
            mult = sightDampen;
        }
        if (ScientistInSight(sightRange * mult, sightAngle))
        {
            currTarget = AttackTarget.scientist;
            currTargetPos = getScientistCenterPos();
            Debug.DrawRay(Scientist.S.transform.position, Vector3.up * 3f, Color.green);
        }
        else if(SwarmInSight(sightRange * mult, sightAngle))
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
