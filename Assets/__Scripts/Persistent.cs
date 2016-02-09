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

	public int carbon = 100;		// Amount of carbon we have.
	public int lithium = 50;     // Amount of lithium we have.

	// The current level of each skill.
	Dictionary<SkillId, int> skillLevels = new Dictionary<SkillId, int>();
	// The max level of each skill.
	Dictionary<SkillId, int> maxSkillLevels = new Dictionary<SkillId, int>();

	public int GetSkillLevel(SkillId id) {
		return skillLevels[id];
	}

	public void SetSkillLevel(SkillId id, int level) {
		skillLevels[id] = Mathf.Clamp(level, 0, maxSkillLevels[id]);
	}

	// Set the level of each skill to its starting level and save the max level.
	void LoadSkillDefinitions(SkillDefinition definition) {
		skillLevels.Add(definition.id, definition.startLevel);
		maxSkillLevels.Add(definition.id, definition.maxLevel);

		foreach (var child in definition.dependents)
			LoadSkillDefinitions(child);
	}

	// One-time initialization.  Awake and Start will be called each time a new scene starts.
	void Init() {
		var definitions = SkillDefinition.GetAllDefinitions();
		foreach (var definition in definitions)
			LoadSkillDefinitions(definition);
	}
}
