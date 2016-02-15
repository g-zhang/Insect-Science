using UnityEngine;
using System.Collections;

public class MovementIntro : MonoBehaviour {

	void OnTriggerExit(Collider other) {
        Destroy(gameObject);
    }
}
