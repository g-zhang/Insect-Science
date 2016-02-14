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
	public float moveAccel;
	public float maxDistFromScientist;
	public float enemyDistractionTime;
	// Set dynamically
	public bool interacting = false;

	GameObject target;          // Object this swarm is attacking/disabling.
	int secondsRemaining = 10;  // Seconds remaining until this swarm dies (used after swarm starts attacking).
	int enemyLayer;
	Vector3 targetOffset;

	// Use this for initialization
	void Start() {
		enemyLayer = LayerMask.NameToLayer("Enemies");
		// Get gameobject compononets
		rigid = GetComponent<Rigidbody>();
		sc = transform.Find("Collider").GetComponent<SphereCollider>();

		// Set variables
		moveSpeed = 2 * Swarm.S.moveSpeed;
		moveAccel = moveSpeed * 2;
		maxDistFromScientist = Swarm.S.maxDistFromScientist;
		enemyDistractionTime = Swarm.S.enemyDistractionTime;
		scientistTrans = Scientist.S.transform; // Used when camera switches back to the scientist
												// and to calculate the scientist's position
												// Ignore collisions with the scientist                       
		Physics.IgnoreCollision(sc, Scientist.S.GetComponent<CapsuleCollider>());
		Physics.IgnoreCollision(sc, Swarm.S.GetComponent<SphereCollider>());
	}

	// Called every physics engine update
	void FixedUpdate() {
		if (target != null) {
			transform.position = target.transform.position + targetOffset;
		}
		if (Main.S.split && target == null) {
			Main.S.ignoreSplit = true;
			Destroy(gameObject);
			// Camera control goes back to the scientist
			GameObject.Find("MultipurposeCameraRig").GetComponent<AutoCam>().m_Target = scientistTrans;
			Main.S.controlScientist = true;
		}

		// Don't do anything if interacting with something
		if (interacting) return;

		// Get velocity vector
		Vector3 vel = rigid.velocity;

		vel.x = moveSpeed * Input.GetAxis("Horizontal");
		vel.y = moveSpeed * Input.GetAxis("Vertical");
		if (vel.magnitude > moveSpeed)
			vel = vel.normalized * moveSpeed;

		if (tooFarAway(Direction.Left) && vel.x < 0f)
			vel.x = 0f;
		else if (tooFarAway(Direction.Right) && vel.x > 0f)
			vel.x = 0f;
		if (tooFarAway(Direction.Up) && vel.y > 0f)
			vel.y = 0f;
		else if (tooFarAway(Direction.Down) && vel.y < 0f)
			vel.y = 0f;

		// Set velocity
		rigid.velocity = vel;
	}

	bool tooFarAway(Direction testDirection) {
		Vector3 curPos = transform.position;
		Vector3 scientistPos = scientistTrans.position;

		switch (testDirection) {
			case Direction.Up: return scientistPos.y + maxDistFromScientist <= curPos.y; // Test if too far up
			case Direction.Down: return scientistPos.y - maxDistFromScientist >= curPos.y; // Test if too far down
			case Direction.Right: return scientistPos.x + maxDistFromScientist <= curPos.x; // Test if too far right
			case Direction.Left: return scientistPos.x - maxDistFromScientist >= curPos.x; // Test if too far left
			default:
				Debug.Log("error: tooFarAway(Direction) received unrecognized test value");
				return true;
		}
	}

	public void OnTriggerEnter(Collider other) {
		if (other.tag == "RoomCamera") {
			Main.S.ShowInteractPopup(other.gameObject, "Press E to disable camera");
		}
		else if (other.tag == "RoomLight") {
			Main.S.ShowInteractPopup(other.gameObject, "Press E to disable lights");
		}
		else if (other.tag == "KeypadTrigger") {
			Main.S.ShowInteractPopup(other.gameObject, "Press E to short-circuit the keypad");
		}
		else if (other.gameObject.layer == enemyLayer) {
			Main.S.ShowInteractPopup(other.gameObject, "Press E to swarm guard");
		}
	}

	public void OnTriggerExit(Collider other) {
		if (other.tag == "RoomCamera" || other.tag == "RoomLight" || other.tag == "KeypadTrigger" || other.gameObject.layer == enemyLayer) {
			Main.S.HideInteractPopup(other.gameObject);

		}
	}

	public void OnTriggerStay(Collider other) {
		if (target == null && Main.S.interact) {
			Interact(other.gameObject);
		}
	}

	public void Interact(GameObject other) {
		if (target != null)
			return;
		// Check what we collided with and see if we are actually able to interact with it.
		target = other;
		if (other.tag == "RoomCamera") {
			other.GetComponentInParent<RoomCamera>().turnedOn = false;
		}
		else if (other.tag == "RoomLight") {
			other.GetComponentInParent<RoomLight>().turnedOn = false;
		}
		else if (other.layer == enemyLayer) {
			other.GetComponent<EnemyBaseBehavior>().swarmed = true;
		}
		else {
            target = null;
            return;
		}

		// If we did interact, then set our parent and start our death timer.
		if (target != null) {
			InvokeRepeating("UpdateDisabledTimer", 0f, 1f);
			targetOffset = transform.position - target.transform.position;
		}
        else
        {
            Debug.Assert(false);
        }

		// Camera control goes back to the scientist
		GameObject.Find("MultipurposeCameraRig").GetComponent<AutoCam>().m_Target = scientistTrans;
		Main.S.controlScientist = interacting = true;
	}

	void UpdateDisabledTimer() {
		if (target != null) {
			// Update the interaction popup to reflect time remaining.
			Main.S.ShowInteractPopup(target, secondsRemaining.ToString());
			secondsRemaining--;
			if (secondsRemaining < 0) {
				// If our time is up, enable whatever we disabled when we started interacting with it.
				if (target.tag == "RoomCamera") {
					target.GetComponentInParent<RoomCamera>().turnedOn = true;
				}
				else if (target.tag == "RoomLight") {
					target.GetComponentInParent<RoomLight>().turnedOn = true;
				}
				else if (target.layer == enemyLayer) {
					target.GetComponent<EnemyBaseBehavior>().swarmed = false;
				}
				Main.S.HideInteractPopup(target);

				Destroy(gameObject);
			}
		}
	}
}