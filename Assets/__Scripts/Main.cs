using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Main : MonoBehaviour {
    static public Main S;                   // Main Singleton.

	public GameObject interactTextPrefab;	// Prefab for the interaction popup text.
    public bool controlScientist = true;    // The player starts the game controlling the scientist.

	// Holds all the currently shown popups.  There can be multiple, so this is a Dictionary.
	Dictionary<GameObject, Text> interactTexts = new Dictionary<GameObject, Text>();
	GameObject interactCanvas;

	void Awake () {
        // Set the singleton
        S = this;

		// Create a canvas for interact popups to be on.
		interactCanvas = new GameObject("InteractPopupCanvas");
		interactCanvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
		interactCanvas.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
	}

	void Update() {
		foreach (var kvp in interactTexts) {
			// Place the text next to the object it is anchored to.
			var screenPos = Camera.main.WorldToScreenPoint(kvp.Key.transform.position) + new Vector3(0f, -20f);

			float width = kvp.Value.preferredWidth;
			float height = kvp.Value.preferredHeight;
			// Make sure the text doesn't go offscreen by clamping it to each edge.
			screenPos.x = Mathf.Clamp(screenPos.x, width / 2, Screen.width - width / 2);
			screenPos.y = Mathf.Clamp(screenPos.y, height / 2, Screen.height - height / 2);

			kvp.Value.transform.position = screenPos;
		}
	}

	// Show an interaction popup for the given anchor object.  Replaces an existing one if the object
	// already has a popup attached to it.
	public void ShowInteractPopup(GameObject anchor, string text) {
		if (interactTexts.ContainsKey(anchor)) {
			// Replace the text on an existing popup.
			interactTexts[anchor].text = text;
		}
		else {
			// Create a new popup and set its text.
			var interactObj = Instantiate(interactTextPrefab);
			interactObj.transform.SetParent(interactCanvas.transform, false);

			var interactText = interactObj.GetComponent<Text>();
			interactText.text = text;

			interactTexts[anchor] = interactText;
		}
	}

	// Hides the interaction popup for the given anchor object if it is shown.
	public void HideInteractPopup(GameObject anchor) {
		if (interactTexts.ContainsKey(anchor)) {
			Destroy(interactTexts[anchor].gameObject);
			interactTexts.Remove(anchor);
		}
	}
}
