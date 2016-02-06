using UnityEngine;
using System.Collections;

public class Swarm : MonoBehaviour {
    static public Swarm S;                              // The Swarm Singleton

    public Transform    poi;                            // Transform of the poi the swarm is going to follow
    public Vector3      offset = new Vector3(0f, 3f);   // Offset so the swarm always above the poi
    public float        easingU = 0.03f;                // Used for linear interpolation
    public float        boxBuffer = 1f;                 // Box buffer around the poi
                                                        // Buffer used so that the swarm is not directly over the poi
    public float        moveSpeed = 1f;                 // Movement speed of the swarm when the player is controlling them

    public Rigidbody    rigid;                          // Rigidbody component of the swarm


	// Use this for initialization
	void Awake () {
        // Set the singleton
        S = this;

        // Get components
        rigid = GetComponent<Rigidbody>();
        rigid.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

	}
	
	void FixedUpdate () {
        // Swarm follows the scientist if the play is controlling the scientist
        if (Main.S.controlScientist) {
            // Get the positions of the poi and the swarm
            Vector3 swarmPos = transform.position;
            Vector3 poiPos = poi.position + offset;

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
        // Player is controlling the swarm
        else {
            // Get velocity vector
            Vector3 vel = rigid.velocity;

            // Y-Axis movement
            bool up = Input.GetKey(KeyCode.W);
            bool down = Input.GetKey(KeyCode.S);
            if (up && !down) vel.y = moveSpeed; // Going up
            else if (!up && down) vel.y = -moveSpeed; // Going down
            else vel.y = 0f; // No y-axis movement

            // X-Axis movement
            bool right = Input.GetKey(KeyCode.D);
            bool left = Input.GetKey(KeyCode.A);
            if (right && !left) vel.x = moveSpeed; // Going right
            else if (!right && left) vel.x = -moveSpeed; // Going left
            else vel.x = 0f; // No x-axis movement

            // Set velocity
            rigid.velocity = vel;
        }
	}
}
