using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fly : MonoBehaviour {

    // Static list of flies to keep track of them all
    static List<Fly> flies = new List<Fly>();
    // Parent references
    Transform swarmParent;
    Rigidbody parentRigid;
    // Own variables
    Rigidbody rigid;
    float forceStrength = 7f;
    float radius;

    void Start() {
        // Reference to the rigid body and parent
        rigid = GetComponent<Rigidbody>();
        swarmParent = transform.parent;
        parentRigid = swarmParent.GetComponent<Rigidbody>();
        // Ignore collisions with scientist
        Physics.IgnoreCollision(GetComponent<SphereCollider>(), GameObject.Find("Scientist").GetComponent<CapsuleCollider>());

        radius = (swarmParent.name == "SmallSwarmPrefab(Clone)") ? (1f / (float)Swarm.S.maxCharges) :
            ((float)Swarm.S.charges / (float)Swarm.S.maxCharges);

        // Add fly to list
        flies.Add(this);

        // Set initial random velocity vector
        rigid.velocity = Random.insideUnitSphere;
    }

    void FixedUpdate() {
        Vector3 parentPos = swarmParent.position;
        Vector3 flyPos = transform.position;
        if (parentPos.x + radius < flyPos.x ||
            parentPos.x - radius > flyPos.x ||
            parentPos.y + radius < flyPos.y ||
            parentPos.y - radius > flyPos.y) {
            transform.localPosition = Vector3.zero;
        }
        // Add more randomness
        rigid.velocity = Random.insideUnitSphere + parentRigid.velocity;

        Vector3 force = (swarmParent.position - transform.position) * forceStrength;
        rigid.AddForce(force);

    }
}