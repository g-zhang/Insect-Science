using UnityEngine;
using System.Collections;

public class TutorialDisplay : MonoBehaviour {

    // Set in inspector
    public string displayText = "";

	void OnTriggerStay(Collider coll) {
        if (coll.gameObject.tag != "Player") return;
        Main.S.ShowInteractPopup(this.gameObject, displayText);
    }

    void OnTriggerExit(Collider coll) {
        if (coll.gameObject.tag != "Player") return;
        Main.S.HideInteractPopup(this.gameObject);
    }
}