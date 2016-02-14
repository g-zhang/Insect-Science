using UnityEngine;
using System.Collections.Generic;

// Keeps data safe between scenes.  Stores stuff that shouldn't be lost when a level ends.
[DisallowMultipleComponent]
public class Persistent : MonoBehaviour {
	// GameObject that holds the only instance of this class.  Created at runtime so that
	// scenes don't try to load multiple copies.
	static GameObject instanceHolder = null;
	static Persistent instance;

	public static Persistent S {
		get {
			if (instanceHolder == null) {
				instanceHolder = new GameObject("Persistent Holder");
				DontDestroyOnLoad(instanceHolder);
				instance = instanceHolder.AddComponent<Persistent>();
				instance.Init();
			}
			return instance;
		}
	}

	public string nextSceneName = "Level_Select";

	// One-time initialization.  Awake and Start will be called each time a new scene starts.
	void Init() {
	}
}
