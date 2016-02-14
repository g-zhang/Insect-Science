using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomLight : MonoBehaviour {
	Renderer dimmer;
    // Put lights in Inspector
    public List<Light> lights;

	[SerializeField]
	bool _turnedOn = true;
	public bool turnedOn {
		get {
			return _turnedOn;
		}
		set {
			_turnedOn = value;
            foreach (var light in lights) {
                light.enabled = _turnedOn;
            }

            dimmer.enabled = !_turnedOn;
		}
	}

	public void Awake() {
		dimmer = transform.Find("Dimmer").GetComponent<Renderer>();

		turnedOn = _turnedOn;
	}
}
