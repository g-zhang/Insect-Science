using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;

public enum SwarmState {
    Follow,         // The player is controlling the scientist and not attacking
    Move,           // The player is controlling the swarm
    Attack,         // The swarm is sent to attack an enemy
    Task            // The swarm is sent to complete a task
}

public enum SwarmType {
    HouseFly,       // First level flies
    BlackFly,       // Second level flies
    DeerFly,        // Third level flies
    HorseFly        // Fourth level flies
}

public class Swarm : MonoBehaviour {
    static public Swarm     S;                                      // The Swarm Singleton

    public Transform        poi;                                    // Transform of the poi the swarm is going to follow
    public Vector3          offset = new Vector3(0f, 1f);           // Offset so the swarm always above the poi
    public Vector3          crouchOffset = new Vector3(0f, 0.5f);   // Offset when crouching
    public float            easingU = 0.03f;                        // Used for linear interpolation
    public float            boxBuffer = 1.5f;                       // Box buffer around the poi
                                                                    // Buffer used so that the swarm is not directly over the poi

    public SwarmState       state;                              // The current state of the swarm
    public bool             crouch;                             // If the scientist is crouching
    public float            moveSpeedMultiplier = 2f;           // Used to calculate swarm movement speed
    public float            distFromScientistMultiplier = 5f;   // Used to calculate the maximum distance the swarm can move from the scientist.
    public float            swarmHealthMult = 15f;              // Used to calculate the health of the swarm (i.e. the number of flies)

    public int              maxHealth;              // Maximum health of the swarm. Also indicates maximum number of flies in swarm.
    public int              health;                 // Current health of the swarm. Also indicates current number of flies in swarm.
    public float            swarmRegenerationRate;  // How fast flies (health) are regenerated
    public SwarmType        type;                   // Type of flies in swarm
    public float            moveSpeed;              // Movement speed of the swarm
    public float            maxDistFromScientist;   // Maximum distance the flies can move from the scientist
    public float            attackPower;            // How much damage the swarm does
    public SphereCollider   soundTrigger;           // Sphere Collider used to determine if an enemy hears the flies or not

    public bool             canAttackAndMove;   // Powerup allows swarm to attack and move at the same time
    public bool             canCoverLights;     // Powerup allows swarm to cover lights and darken an area
    public bool             hasVenom;           // Powerup allows swarm to slow enemies when attacking them
    public bool             canEatWood;         // Powerup allows swarm to eat through wood

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

        // Default state of the swarm
        state = SwarmState.Follow;

	}

    void Start() {
        // Load values based on skill level
        moveSpeed = (float)Persistent.S.GetSkillLevel(SkillId.MovementSpeed) + moveSpeedMultiplier;
        maxDistFromScientist = (float)Persistent.S.GetSkillLevel(SkillId.MoveDistance) *
            distFromScientistMultiplier + distFromScientistMultiplier;
        maxHealth = (int)(Persistent.S.GetSkillLevel(SkillId.SwarmSize) * swarmHealthMult) + (int)swarmHealthMult;
        health = maxHealth;
        swarmRegenerationRate = (float)Persistent.S.GetSkillLevel(SkillId.SwarmRegen) + 1f;
        attackPower = (float)Persistent.S.GetSkillLevel(SkillId.Biting) + 1f;

        // Begin regenerating flies
        Invoke("regenerateFlies", 1f / swarmRegenerationRate);

        // Ignore collisions between scientist and swarm
        Physics.IgnoreCollision(sc, poi.transform.gameObject.GetComponent<CapsuleCollider>());
    }

    void Update() {
        crouch = poi.gameObject.GetComponent<ThirdPersonCharacter>().m_Crouching;
    }
    void FixedUpdate () {

        // Different movements based on state
        switch (state) {
            // If the swarm is following the scientist
            case SwarmState.Follow:
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
                break;
            
            // If the player has manual control of the swarm
            case SwarmState.Move:
                // Get velocity vector
                Vector3 vel = rigid.velocity;
                // Poi position
                poiPos = poi.position;

                // Y-Axis movement
                bool up = Input.GetKey(KeyCode.W);
                bool down = Input.GetKey(KeyCode.S);

                if (up && !down && !tooFarAway(1)) {
                    vel.y = moveSpeed; // Going up
                }
                else if (!up && down && !tooFarAway(2)) {
                    vel.y = -moveSpeed; // Going down
                }
                else vel.y = 0f; // No y-axis movement

                // X-Axis movement
                bool right = Input.GetKey(KeyCode.D);
                bool left = Input.GetKey(KeyCode.A);

                if (right && !left && !tooFarAway(3)) {
                    vel.x = moveSpeed; // Going right
                }
                else if (!right && left && !tooFarAway(4)) {
                    vel.x = -moveSpeed; // Going left
                }
                else vel.x = 0f; // No x-axis movement

                // Set velocity
                rigid.velocity = vel.normalized * moveSpeed;
                break;

            case SwarmState.Attack:
                break;
        }
	}

    bool tooFarAway(int testDirection) {
        Vector3 poiPos = poi.position;
        Vector3 curPos = transform.position;

        switch (testDirection) {
            case 1: return poiPos.y + maxDistFromScientist <= curPos.y; // Test if too far up
            case 2: return poiPos.y - maxDistFromScientist >= curPos.y; // Test if too far down
            case 3: return poiPos.x + maxDistFromScientist <= curPos.x; // Test if too far right
            case 4: return poiPos.x - maxDistFromScientist >= curPos.x; // Test if too far left
            default:
                Debug.Log("error: tooFarAway(int) received unrecognized test value");
                return true;
        }
    }

    void regenerateFlies() {
        health = (health < maxHealth) ? health + 1 : health;
		Main.S.flies = health;
    }

    /*
    FOR GORDON
    void OnTriggerEnter(Collider coll) {
        if (coll.gameObject.tag == "Swarm") {
            TakeHit();
            Invoke("TakeHit", 1f);
        }
        else {
            // ... other collider logic if you need it
        }
    }

    void OnTriggerExit(Collider coll) {
        if (coll.gameObject.tag == "Swarm") {
            CancelInvoke("TakeHit");
        }
        else {
            // ... other collider logic if you need it
        }
    }

    void TakeHit() {
        health -= Swarm.S.attackPower;
        if (health <= 0) {
            // ... die
        }
    }

    */
}
