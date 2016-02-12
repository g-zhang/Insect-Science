using UnityEngine;
using System.Collections;

public class HallDoor : MonoBehaviour {
	[SerializeField]
	int linkId;

	public HallDoor linkedDoor;

	void Awake() {
		if (linkedDoor == null) {
			foreach (var door in FindObjectsOfType<HallDoor>()) {
				if (door != this && door.linkId == linkId) {
					linkedDoor = door;
					door.linkedDoor = this;
					break;
				}
			}
		}

		GetComponentInChildren<TextMesh>().text = linkId.ToString();
	}

	public void OnDrawGizmos() {
		Gizmos.matrix = transform.localToWorldMatrix;
		for (int i = 0; i < linkId; ++i) {
			Gizmos.DrawSphere(new Vector3(0.1f * i, 0.75f, -1f), 0.05f);
		}
	}
}
