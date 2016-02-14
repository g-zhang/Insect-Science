﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Scientist : MonoBehaviour {

	public static Scientist S;
    public int maxHP = 3;

    [Header("Status")]
    public int currHP;

	bool usedDoor = false;

	void Awake() {
		S = this;
        currHP = maxHP;
	}

    void Update()
    {
        if (currHP <= 0)
        {
            Main.S.FadeOutAndExit(SceneManager.GetActiveScene().name);
        }
    }

	public void OnTriggerEnter(Collider other) {
		var door = other.GetComponentInParent<HallDoor>();
		if (door != null) {
			Main.S.ShowInteractPopup(door.gameObject, "Press E to use the hallway");
		}
		var elevator = other.GetComponent<ElevatorTrigger>();
		if (elevator != null) {
			Main.S.ShowInteractPopup(elevator.gameObject, "Press E to use the elevator");
		}
		if (other.tag == "EndZone") {
			Main.S.ShowInteractPopup(other.gameObject, "Press E to retrieve the launch codes");
	    }
	}

	void OnTriggerStay(Collider other) {
		if (!Main.S.controlScientist) {
			return;
		}
		if (Main.S.interact) {
			var door = other.GetComponentInParent<HallDoor>();
				if (door != null && !usedDoor) {
				var diff = transform.position - other.transform.position;
				var swarmDiff = Swarm.S.transform.position - transform.position;
				transform.position = diff + door.linkedDoor.transform.position + new Vector3(0f, 0.1f, 0f);
				Swarm.S.transform.position = transform.position + swarmDiff;
				Main.S.ignoreInteraction = true;
			}

			var elevator = other.GetComponent<ElevatorTrigger>();
				if (elevator != null && !usedDoor) {
				Vector3 sciPos = transform.position;
				Vector3 swarmPos = Swarm.S.transform.position;
				sciPos.y = elevator.destination.transform.position.y + 0.1f;
				swarmPos.y = elevator.destination.transform.position.y + 1.1f;
				transform.position = sciPos;
				Swarm.S.transform.position = swarmPos;
				Main.S.ignoreInteraction = true;
			}

			if (other.tag == "EndZone") {
				Main.S.FadeOutAndExit(Persistent.S.nextSceneName);
			}
		}
	}

	public void OnTriggerExit(Collider other) {
		var door = other.GetComponentInParent<HallDoor>();
		if (door != null) {
			Main.S.HideInteractPopup(door.gameObject);
		}
		var elevator = other.GetComponent<ElevatorTrigger>();
		if (elevator != null) {
			Main.S.HideInteractPopup(elevator.gameObject);
		}
		if (tag == "EndZone") {
			Main.S.HideInteractPopup(other.gameObject);
		}
	}
}
