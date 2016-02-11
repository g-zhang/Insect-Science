using UnityEngine;
using System.Collections;

public class RoomLight : MonoBehaviour {
	Renderer dimmer;

	[SerializeField]
	bool _turnedOn = true;
	public bool turnedOn {
		get {
			return _turnedOn;
		}
		set {
			_turnedOn = value;

			dimmer.enabled = !_turnedOn;
		}
	}

	public void Awake() {
		dimmer = transform.Find("Dimmer").GetComponent<Renderer>();

		turnedOn = _turnedOn;
	}
}
