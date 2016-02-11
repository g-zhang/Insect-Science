using UnityEngine;
using System.Collections;

public class RoomCamera : MonoBehaviour {
	// Z (up/down) angle from where the camera is facing that the lines will point.
	public float lineAngle = 8f;

	[SerializeField]
	bool _turnedOn = true;
	public bool turnedOn {
		get {
			return _turnedOn;
		}
		set {
			_turnedOn = value;

			leftLine.enabled = _turnedOn;
			rightLine.enabled = _turnedOn;
		}
	}

	GameObject leftLineObj;
	GameObject rightLineObj;
	LineRenderer leftLine;
	LineRenderer rightLine;

	// Mask for determining what each line hits.
	int lineLayerMask;

	public void Awake() {
		leftLineObj = transform.Find("LeftLine").gameObject;
		rightLineObj = transform.Find("RightLine").gameObject;
		leftLine = leftLineObj.GetComponent<LineRenderer>();
		rightLine = rightLineObj.GetComponent<LineRenderer>();

		// Rotate both line objects using lineAngle.
		leftLineObj.transform.localRotation = Quaternion.Euler(0f, 0f, -lineAngle);
		rightLineObj.transform.localRotation = Quaternion.Euler(0f, 0f, lineAngle);

		// Set the start position for both lines to their object's position in world coordinates.
		leftLine.SetPosition(0, leftLineObj.transform.position);
		rightLine.SetPosition(0, rightLineObj.transform.position);

		// Set a temporary end position.  This will be overwritten in update.
		leftLine.SetPosition(1, leftLineObj.transform.position + leftLineObj.transform.right * 5f);
		rightLine.SetPosition(1, rightLineObj.transform.position + rightLineObj.transform.right * 5f);

		// The line can hit the ground, walls (Default), and the player (Scientist).
		lineLayerMask = LayerMask.GetMask("Default", "Ground", "Scientist");

		turnedOn = _turnedOn;
	}

	public void Update() {
		if (turnedOn) {
			UpdateLineObject(leftLineObj, leftLine);
			UpdateLineObject(rightLineObj, rightLine);
		}
	}

	// Updates the ending position of the given lineObj and line and also checks if the player is
	// within the field of view.
	void UpdateLineObject(GameObject lineObj, LineRenderer line) {
		var transform = lineObj.transform;

		const float castDistance = 10f;
		RaycastHit hit;
		if (Physics.Raycast(new Ray(transform.position, transform.right), out hit, castDistance, lineLayerMask, QueryTriggerInteraction.Ignore)) {
			line.SetPosition(1, hit.point);

			if (hit.collider.tag == "Player") {

			}
		}
		else
			line.SetPosition(1, transform.position + transform.right * castDistance);
	}
}
