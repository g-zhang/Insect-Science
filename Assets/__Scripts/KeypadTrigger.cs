using UnityEngine;
using System.Collections;

public class KeypadTrigger : MonoBehaviour {

    public float duration = 10f;

	bool _active = true;
    public bool active {
		get { return _active; }
		set {
			_active = value;
			transform.parent.gameObject.SetActive(value);
		}
	}
 
    void DeactivateDoor() {
        transform.parent.gameObject.SetActive(false);
        active = false;
    }
    void ActivateDoor() {
        active = true;
        transform.parent.gameObject.SetActive(true);
    }
 
}
