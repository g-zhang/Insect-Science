using UnityEngine;
using System.Collections;

public class Scientist : MonoBehaviour {

    public static Scientist S;

	bool usedDoor = false;

	void Awake () {
	   S = this;
	}

	void OnTriggerStay(Collider other) {
		if (!Main.S.controlScientist) {
			return;
		}
		var door = other.GetComponentInParent<HallDoor>();
		if (door != null && Input.GetAxis("Interact") > 0 && !usedDoor) {
			var diff = transform.position - other.transform.position;
			transform.position = diff + door.linkedDoor.transform.position + new Vector3(0f, 0.1f, 0f);
			usedDoor = true;
			Invoke("ResetUsedDoor", 0.5f);
		}
	}

	void ResetUsedDoor() {
		usedDoor = false;
	}
}
