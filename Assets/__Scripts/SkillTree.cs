﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class Skill {
	public SkillDefinition definition;
	public Skill parent = null;
	public int level;
	public Button button;
	public Text buttonText;
}

public class SkillTree : MonoBehaviour {
	public GameObject skillButtonPrefab;

	Canvas canvas;
	IList<SkillDefinition> definitions;
	Dictionary<SkillId, Skill> skills = new Dictionary<SkillId, Skill>();
	float skillButtonWidth;

	public void OnMenuClick() {
		canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
	}

	void Awake() {
		skillButtonWidth = skillButtonPrefab.GetComponent<RectTransform>().rect.width;

		canvas = transform.Find("Tree").GetComponent<Canvas>();
		canvas.gameObject.SetActive(true);

		definitions = SkillDefinition.GetAllDefinitions();

		for (int i = 0; i < 3; ++i)
			AddSkills(new Vector2(skillButtonWidth, 0f) + new Vector2(2.05f * skillButtonWidth * i, -25f), null, definitions[i]);

		for (int i = 3; i < definitions.Count; ++i)
			AddSkills(new Vector2(skillButtonWidth, 0f) + new Vector2(2.05f * skillButtonWidth * (i - 3), -225f), null, definitions[i]);
	}

	public void OnDestroy() {
		foreach (var skill in skills.Values) {
			Persistent.S.SetSkillLevel(skill.definition.id, skill.level);
		}
	}

	void AddSkills(Vector2 position, Skill parent, SkillDefinition definition) {
		var obj = Instantiate(skillButtonPrefab);
		obj.transform.SetParent(canvas.transform, false);
		var rectTransform = obj.GetComponent<RectTransform>();
		rectTransform.anchorMin = new Vector2(0f, 1f);
		rectTransform.anchorMax = new Vector2(0f, 1f);
		rectTransform.localPosition = position;

		obj.transform.Find("CarbonAmount").GetComponent<Text>().text = definition.carbonCost.ToString();
		obj.transform.Find("LithiumAmount").GetComponent<Text>().text = definition.lithiumCost.ToString();

		var button = obj.GetComponentInChildren<Button>();
		var buttonText = obj.transform.Find("Button").Find("Text").GetComponent<Text>();
		buttonText.text = definition.name;
		var skill = new Skill() {
			definition = definition,
			parent = parent,
			level = 0,
			button = button,
			buttonText = buttonText,
		};
		skills.Add(definition.id, skill);

		button.onClick.AddListener(() => { OnSkillPressed(skill); });
		button.interactable = CanUpgradeSkill(skill);

		Vector2 offset = new Vector2(0f, -50f);
		offset.x -= skillButtonWidth / 2 * (definition.dependents.Length - 1);
		for (int i = 0; i < definition.dependents.Length; ++i) {
			AddSkills(position + offset, skill, definition.dependents[i]);
			offset.x += skillButtonWidth;
		}
	}

	void OnSkillPressed(Skill skill) {
		if (!CanUpgradeSkill(skill))
			return;

		Persistent.S.carbon -= skill.definition.carbonCost;
		Persistent.S.lithium -= skill.definition.lithiumCost;
		skill.level++;
		OutGameUI.S.UpdateAllStats();

		skill.buttonText.text = string.Format("{0} {1}", skill.definition.name, skill.level);

		foreach (var s in skills.Values) {
			s.button.interactable = CanUpgradeSkill(s);
		}
	}

	bool CanUpgradeSkill(Skill skill) {
		if (skill.parent != null && skill.parent.level == 0)
			return false;
		if (skill.level == skill.definition.maxLevel)
			return false;
		if (Persistent.S.carbon < skill.definition.carbonCost || Persistent.S.lithium < skill.definition.lithiumCost)
			return false;
		return true;
	}
}
