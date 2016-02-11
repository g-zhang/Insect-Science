using UnityEngine;
using System.Collections;
using UnityStandardAssets.Cameras;

public enum Direction {
    Up,
    Down,
    Right,
    Left
}

public class SmallSwarm : MonoBehaviour {

    public Rigidbody rigid;
    public SphereCollider sc;
    public Transform scientistTrans;
    // Set in start
    public float moveSpeed;
    public float maxDistFromScientist;
    public float enemyDistractionTime;
    // Set dynamically
    public bool interacting = false;

	// Use this for initialization
	void Start () {
        // Get gameobject compononets
        rigid = GetComponent<Rigidbody>();
        sc = GetComponent<SphereCollider>();

        // Set variables
        moveSpeed = Swarm.S.moveSpeed;
        maxDistFromScientist = Swarm.S.maxDistFromScientist;
        enemyDistractionTime = Swarm.S.enemyDistractionTime;
        scientistTrans = Scientist.S.transform; // Used when camera switches back to the scientist
                                                // and to calculate the scientist's position
        // Ignore collisions with the scientist                       
        Physics.IgnoreCollision(sc, Scientist.S.GetComponent<CapsuleCollider>());
    }
	
    // Called once every frame
    void Update() {
        // Player trying to interact with something
        if (Input.GetKeyDown(KeyCode.E)) {
            // Will implement function later
            Interact();
        }
    }

	// Called every physics engine update
	void FixedUpdate () {
        // Don't do anything if interacting with something
        if (interacting) return;

        // Get velocity vector
        Vector3 vel = rigid.velocity;

        // Y-Axis movement
        bool up = Input.GetKey(KeyCode.W);
        bool down = Input.GetKey(KeyCode.S);

        if (up && !down && !tooFarAway(Direction.Up)) {
            vel.y = moveSpeed; // Going up
        }
        else if (!up && down && !tooFarAway(Direction.Down)) {
            vel.y = -moveSpeed; // Going down
        }
        else vel.y = 0f; // No y-axis movement

        // X-Axis movement
        bool right = Input.GetKey(KeyCode.D);
        bool left = Input.GetKey(KeyCode.A);

        if (right && !left && !tooFarAway(Direction.Right)) {
            vel.x = moveSpeed; // Going right
        }
        else if (!right && left && !tooFarAway(Direction.Left)) {
            vel.x = -moveSpeed; // Going left
        }
        else vel.x = 0f; // No x-axis movement

        // Set velocity
        rigid.velocity = vel.normalized * moveSpeed;
    }

    bool tooFarAway(Direction testDirection) {
        Vector3 curPos = transform.position;
        Vector3 scientistPos = scientistTrans.position;

        switch (testDirection) {
            case Direction.Up:      return scientistPos.y + maxDistFromScientist <= curPos.y; // Test if too far up
            case Direction.Down:    return scientistPos.y - maxDistFromScientist >= curPos.y; // Test if too far down
            case Direction.Right:   return scientistPos.x + maxDistFromScientist <= curPos.x; // Test if too far right
            case Direction.Left:    return scientistPos.x - maxDistFromScientist >= curPos.x; // Test if too far left
            default:
                Debug.Log("error: tooFarAway(Direction) received unrecognized test value");
                return true;
        }
    }

    void Interact() {

        // TODO: Interaction code between different objects

        // Camera control goes back to the scientist
        GameObject.Find("MultipurposeCameraRig").GetComponent<AutoCam>().m_Target = scientistTrans;
        Main.S.controlScientist = interacting = true;
    }
}
