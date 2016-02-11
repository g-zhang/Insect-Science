using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.Cameras;

public class Swarm : MonoBehaviour {
    static public Swarm     S; // The Swarm Singleton

    public Transform        poi;                                            // Transform of the poi the swarm is going to follow
    public Vector3          offset = new Vector3(0f, 1f);                   // Offset so the swarm always above the poi
    public Vector3          crouchOffset = new Vector3(0f, 0.5f);           // Offset when crouching
    public float            easingU = 0.03f;                                // Used for linear interpolation
    public float            boxBuffer = 1.5f;                               // Box buffer around the poi
                                                                            // Swarm will only move if it's not inside the box buffer
    public Vector3          maxSwarmScale = new Vector3(0.8f, 0.8f, 0.8f);  // Vector used for scaling the size of the swarm
    public bool             regenInvoked = false;                           // Keeps track of whether the swarm is already regenerating

    // Variables set in the Inspector
    public GameObject       smallSwarmPrefab;               // Prefab for the small swarm
    public float            moveSpeed = 2f;                 // Movement speed of the small swarms 
    public float            maxDistFromScientist = 10f;     // Maximum distance the small swarm can move from the scientist
    public float            swarmRegenerationRate = 30f;    // How fast swarm charges are regenerated (in seconds)
    public float            enemyDistractionTime = 10f;     // Number of the seconds in which the small swarms distract enemies
    public bool ________________;
    // Dynamic variables
    public bool             crouch;         // If the scientist is crouching
    public int              maxCharges;     // Maximum number of swarm charges for this level
    public int              charges;        // Current number of charges the swarm has left
    public SphereCollider   soundTrigger;   // Sphere Collider used to determine if an enemy hears the flies or not

    // Set in Awake
    public Rigidbody        rigid;  // Rigidbody component of the swarm
    public SphereCollider   sc;     // Sphere collider of the swarm


	// Use this for initialization
	void Awake () {
        // Set the singleton
        S = this;

        // Get components
        rigid = GetComponent<Rigidbody>();
        rigid.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        sc = GetComponent<SphereCollider>();

        soundTrigger = transform.Find("Noise Trigger").GetComponent<SphereCollider>();

        // Don't want the scientist and swarm colliding
        Physics.IgnoreCollision(sc, Scientist.S.GetComponent<CapsuleCollider>());
	}

    void Update() {
        crouch = poi.gameObject.GetComponent<ThirdPersonCharacter>().m_Crouching;
        if(Input.GetKeyDown(KeyCode.Q)) {
            SpawnSubSwarm();
        }
        // Cancel invoke if at max charges
        if ( (maxCharges == charges) && regenInvoked) {
            CancelInvoke("RegenerateSwarm");
            regenInvoked = false;
        }
    }

    // Called every physics engine update
    void FixedUpdate() {

        // Adjust offset if crouching
        Vector3 offsetVec = (crouch) ? crouchOffset : offset;

        // Get the positions of the poi and the swarm
        Vector3 swarmPos = transform.position;
        Vector3 poiPos = poi.position + offsetVec;

        // Set new positions so that the swarm is not directly over the poi
        if (swarmPos.x >= poiPos.x + boxBuffer) {
            // To the right of the poi box buffer
            poiPos.x += boxBuffer;
        }
        else if (swarmPos.x <= poiPos.x - boxBuffer) {
            // To the left of the poi box buffer
            poiPos.x -= boxBuffer;
        }
        else {
            // Inside the box buffer
            poiPos = swarmPos;
        }

        // Linear interpolation
        Vector3 finalPos = (1 - easingU) * swarmPos + easingU * poiPos;

        // Set final position
        transform.position = finalPos;
    }

    void SpawnSubSwarm() {
        // If there's already a small swarm or no charges, ignore
        if (!Main.S.controlScientist || (charges <= 0) ) return;
        // No longer controlling scientist
        Main.S.controlScientist = false;
        // Deduct a charge and begin regeneration (if it hasn't already been invoked)
        charges -= 1;
        if (!regenInvoked) Invoke("RegenerateSwarm", swarmRegenerationRate);
        regenInvoked = true;

        AdjustSwarmSize(); // Adjust scale for big swarm (make it smaller)
        CreateSubSwarm();  // Create sub swarm

        // TODO: tuning adjustments
    }

    void AdjustSwarmSize() {
        transform.localScale = ((float)charges / (float)maxCharges) * maxSwarmScale;
        // TODO: tuning adjustments
    }

    void CreateSubSwarm() {
        GameObject go = Instantiate<GameObject>(smallSwarmPrefab);
        go.transform.localScale = (1f / (float)maxCharges) * maxSwarmScale;
        go.transform.position = transform.position;
        // TODO: tuning adjustments

        // Camera now follows the sub swarm
        GameObject.Find("MultipurposeCameraRig").GetComponent<AutoCam>().m_Target = go.transform;
    }

    void RegenerateSwarm() {
        // Increment charges and adjust size
        charges++;
        AdjustSwarmSize();
    }

}
