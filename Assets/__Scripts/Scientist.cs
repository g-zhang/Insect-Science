using UnityEngine;
using System.Collections;

public class Scientist : MonoBehaviour {

    public static Scientist S;

	bool usedDoor = false;

	void Awake () {
	   S = this;
	}

	void OnTriggerStay(Collider other) {
		var door = other.GetComponentInParent<HallDoor>();
		if (Input.GetKeyDown(KeyCode.Z) && door != null && !usedDoor) {
			var diff = transform.position - other.transform.position;
			transform.position = diff + door.linkedDoor.transform.position + new Vector3(0f, 0.1f, 0f);
			usedDoor = true;
			Invoke("ResetUsedDoor", 0.1f);
		}
	}

	void ResetUsedDoor() {
		usedDoor = false;
	}
}
