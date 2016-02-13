using UnityEngine;
using System.Collections;

public class Scientist : MonoBehaviour {

	public static Scientist S;

	bool usedDoor = false;

	void Awake() {
		S = this;
	}

	public void OnTriggerEnter(Collider other) {
		var door = other.GetComponentInParent<HallDoor>();
		if (door != null) {
			Main.S.ShowInteractPopup(door.gameObject, "Press E to use the hallway");
		}
	}

	void OnTriggerStay(Collider other) {
		if (!Main.S.controlScientist) {
			return;
		}
		var door = other.GetComponentInParent<HallDoor>();
		if (door != null && Input.GetAxis("Interact") > 0 && !usedDoor) {
			var diff = transform.position - other.transform.position;
			var swarmDiff = Swarm.S.transform.position - transform.position;
			transform.position = diff + door.linkedDoor.transform.position + new Vector3(0f, 0.1f, 0f);
			Swarm.S.transform.position = transform.position + swarmDiff;
			usedDoor = true;
			Invoke("ResetUsedDoor", 0.5f);
		}
	}

	public void OnTriggerExit(Collider other) {
		var door = other.GetComponentInParent<HallDoor>();
		if (door != null) {
			Main.S.HideInteractPopup(door.gameObject);
		}
	}

	void ResetUsedDoor() {
		usedDoor = false;
	}
}
