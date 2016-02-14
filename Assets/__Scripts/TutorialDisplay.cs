using UnityEngine;
using System.Collections;

public class TutorialDisplay : MonoBehaviour {

    // Set in inspector
    public string displayText = "";
    public GameObject tutorialGUIgameObject;
    public bool italicized;

    GUIText tutorialGUI;

    void Start() {
        tutorialGUI = tutorialGUIgameObject.GetComponent<GUIText>();
    }

	void OnTriggerStay(Collider coll) {
        if (coll.gameObject.tag != "Player") return;
        tutorialGUI.text = displayText;
        if (italicized) tutorialGUI.fontStyle = FontStyle.Italic;
    }

    void OnTriggerExit(Collider coll) {
        if (coll.gameObject.tag != "Player") return;
        tutorialGUI.GetComponent<GUIText>().text = "";
        tutorialGUI.fontStyle = FontStyle.Normal;
    }
}