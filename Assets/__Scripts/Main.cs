using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour {
	enum FadeState {
		None,
		In,
		Out,
	}

	static public Main S;                   // Main Singleton.

	public GameObject interactTextPrefab;   // Prefab for the interaction popup text.
	public GameObject flyChargePrefab;      // Prefab for the fly charge image.
	public Sprite screenFadeSprite;			// Sprite for the screen fade image.

	bool _controlScientist = true;    // The player starts the game controlling the scientist.
	public bool controlScientist {
		get { return _controlScientist; }
		set {
			_controlScientist = value;
			ignoreInteraction = true;
		}
	}

	float endIgnoreInteractionTime = 0f;
	public bool ignoreInteraction {
		get { return endIgnoreInteractionTime > Time.time; }
		set {
			if (value) {
				endIgnoreInteractionTime = Time.time + 0.5f;
			}
			else {
				endIgnoreInteractionTime = 0f;
			}
		}
	}

	public bool interact {
		get { return Input.GetAxis("Interact") > 0f && !ignoreInteraction; }
	}

	// Holds all the currently shown popups.  There can be multiple, so this is a Dictionary.
	Dictionary<GameObject, Text> interactTexts = new Dictionary<GameObject, Text>();
	GameObject interactCanvas;
	GameObject dimmer;
	List<GameObject> flyChargeObjs = new List<GameObject>();

	float fadeStartTime;
	FadeState fadeState = FadeState.In;

	public int flyCharges {
		get { return flyChargeObjs.Count; }
		set {
			int oldCharges = flyChargeObjs.Count;
			for (int i = oldCharges; i < value; ++i) {
				var charge = Instantiate(flyChargePrefab);
				charge.transform.SetParent(interactCanvas.transform);
				flyChargeObjs.Add(charge);
			}
			for (int i = value; i < oldCharges; ++i) {
				var obj = flyChargeObjs[flyChargeObjs.Count - 1];
				Destroy(obj);
				flyChargeObjs.RemoveAt(flyChargeObjs.Count - 1);
			}
		}
	}

	void Awake() {
		// Set the singleton
		S = this;

		// Create a canvas for interact popups to be on.
		interactCanvas = new GameObject("InteractPopupCanvas");
		interactCanvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
		interactCanvas.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;

		var dimCanvas = new GameObject("DimCanvas");
		dimCanvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
		dimCanvas.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;

		dimmer = new GameObject("ScreenDimmer");
		dimmer.transform.SetParent(dimCanvas.transform, false);
		dimmer.AddComponent<Image>().sprite = screenFadeSprite;
		dimmer.GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
		dimmer.GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);

		flyCharges = 3;

		FadeIn();
	}

	void Update() {
		if (fadeState != FadeState.None) {
			const float fadeDuration = 0.5f;

			bool doneFading = Time.time > fadeStartTime + fadeDuration;
			float alpha = 0f;
			if (fadeState == FadeState.In) {
				if (doneFading) {
					alpha = 0f;
				}
				else {
					alpha = 1f - (Time.time - fadeStartTime) / fadeDuration;
				}
			}
			else if (fadeState == FadeState.Out) {
				if (doneFading) {
					alpha = 1f;
					if (nextSceneName != null) {
						SceneManager.LoadScene(nextSceneName);
					}
				}
				else {
					alpha = (Time.time - fadeStartTime) / fadeDuration;
				}
			}

			if (fadeState != FadeState.None) {
				dimmer.GetComponent<Image>().color = new Color(1f, 1f, 1f, alpha);
			}
			if (doneFading) {
				fadeState = FadeState.None;
			}
		}

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

		float xOffset = 0f;
		foreach (var obj in flyChargeObjs) {
			var screenPos = Camera.main.WorldToScreenPoint(Swarm.S.transform.position) + new Vector3(0f, -20f);
			screenPos.x += xOffset;
			xOffset += flyChargePrefab.GetComponent<Image>().rectTransform.rect.width;

			obj.transform.position = screenPos;
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

	void FadeIn() {
		fadeState = FadeState.In;
		fadeStartTime = Time.time;
	}

	string nextSceneName;
	public void FadeOutAndExit(string nextSceneName) {
		fadeState = FadeState.Out;
		this.nextSceneName = nextSceneName;
		fadeStartTime = Time.time;
	}
}
