using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public enum SkillId {
	MovementSpeed,
	MoveDistance,
	SilentFlying,
	MovementAttack,
	SwarmSize,
	SwarmRegen,
	LightCovering,
	Biting,
	Venom,
	WoodEating,
	HouseFly,
	BlackFly,
	DeerFly,
	HorseFly,
	ScientistHealth,
	Hacking,
	Targetter,
	Shield,
}

public class SkillDefinition {
	public string name;
	public SkillId type;
	public int startLevel = 0;
	public int maxLevel = 1;
	public int carbonCost = 0;
	public int lithiumCost = 0;
	public string description;
	public SkillDefinition[] dependents = new SkillDefinition[0];
}

public class Skill {
	public SkillDefinition definition;
	public Skill parent = null;
	public int level;
	public Button button;
}

public class SkillTree : MonoBehaviour {
	public GameObject skillButtonPrefab;

	Canvas canvas;
	List<SkillDefinition> definitions = new List<SkillDefinition>();
	Dictionary<SkillId, Skill> skills = new Dictionary<SkillId, Skill>();
	float skillButtonWidth;

	public void OnMenuClick() {
		canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
	}

	void Awake() {
		skillButtonWidth = skillButtonPrefab.GetComponent<RectTransform>().rect.width;

		canvas = transform.Find("Tree").GetComponent<Canvas>();
		canvas.gameObject.SetActive(false);

		definitions.Add(new SkillDefinition() {
			name = "Movement Speed",
			type = SkillId.MovementSpeed,
			maxLevel = 3,
			carbonCost = 10,
			lithiumCost = 1,
			description = "Increases insect movement speed",
			dependents = new[] {
				new SkillDefinition() {
					name = "Move Distance",
					type = SkillId.MoveDistance,
					maxLevel = 3,
					carbonCost = 15,
					lithiumCost = 1,
					description = "Increases maximum distance from scientist",
					dependents = new[] {
						new SkillDefinition() {
							name = "Silent Flying",
							type = SkillId.SilentFlying,
							maxLevel = 3,
							carbonCost = 20,
							lithiumCost = 10,
							description = "Insects are less noticeable"
						}
					}
				},
				new SkillDefinition() {
					name = "Movement Attack",
					type = SkillId.MovementAttack,
					maxLevel = 3,
					carbonCost = 15,
					lithiumCost = 3,
					description = "Insects attack as they move",
				}
			}
		});

		definitions.Add(new SkillDefinition() {
			name = "Swarm Size",
			type = SkillId.SwarmSize,
			maxLevel = 3,
			carbonCost = 10,
			lithiumCost = 1,
			description = "Increases number of insects in the swarm",
			dependents = new SkillDefinition[] {
				new SkillDefinition() {
					name = "Swarm Regen.",
					type = SkillId.SwarmRegen,
					maxLevel = 3,
					carbonCost = 15,
					lithiumCost = 1,
					description = "Increases the speed at which lost insects regenerate"
				},
				new SkillDefinition() {
					name = "Light Covering",
					type = SkillId.LightCovering,
					carbonCost = 20,
					lithiumCost = 10,
					description = "Allows insects to cover light fixtures to darken rooms"
				}
			}
		});

		definitions.Add(new SkillDefinition() {
			name = "Biting",
			type = SkillId.Biting,
			maxLevel = 3,
			carbonCost = 10,
			lithiumCost = 1,
			description = "Increases strength of insect bites",
			dependents = new SkillDefinition[] {
				new SkillDefinition() {
					name = "Venom",
					maxLevel = 3,
					carbonCost = 15,
					lithiumCost = 5,
					type = SkillId.Venom,
					description = "Imbues bits with venom, slowing down enemies"
				},
				new SkillDefinition() {
					name = "Wood Eating",
					type = SkillId.WoodEating,
					carbonCost = 20,
					lithiumCost = 10,
					description = "Allows insects to eat through wooden objects"
				}
			}
		});

		definitions.Add(new SkillDefinition() {
			name = "House Fly",
			type = SkillId.HouseFly,
			carbonCost = 10,
			lithiumCost = 5,
			description = "Bigger, stronger insects",
			dependents = new SkillDefinition[] {
				new SkillDefinition() {
					name = "Black Fly",
					type = SkillId.BlackFly,
					carbonCost = 20,
					lithiumCost = 10,
					description = "Bigger, stronger insects",
					dependents = new SkillDefinition[] {
						new SkillDefinition() {
							name = "Deer Fly",
							type = SkillId.DeerFly,
							carbonCost = 30,
							lithiumCost = 15,
							description = "Bigger, stronger insects",
							dependents = new SkillDefinition[] {
								new SkillDefinition() {
									name = "Horse Fly",
									type = SkillId.HorseFly,
									carbonCost = 40,
									lithiumCost = 20,
									description = "Bigger, stronger insects"
								}
							}
						}
					}
				}
			}
		});

		definitions.Add(new SkillDefinition()
		{
			name = "Scientist Health",
			type = SkillId.ScientistHealth,
			maxLevel = 3,
			carbonCost = 20,
			lithiumCost = 1,
			description = "Health of the scientist",
			dependents = new SkillDefinition[] {
				new SkillDefinition() {
					name = "Hacking",
					type = SkillId.Hacking,
					maxLevel = 3,
					carbonCost = 20,
					lithiumCost = 5,
					description = "Hack into higher level terminals"
				},
				new SkillDefinition() {
					name = "Fly Targetter",
					type = SkillId.Targetter,
					carbonCost = 5,
					lithiumCost = 5,
					description = "A gun that lets you target enemies for flies to attack"
				},
				new SkillDefinition() {
					name = "Fly Shield",
					type = SkillId.Shield,
					carbonCost = 20,
					lithiumCost = 10,
					description = "Your flies take hits for you"
				}
			}
		});

		for (int i = 0; i < 3; ++i)
			AddSkills(new Vector2(skillButtonWidth / 2, 0f) + new Vector2(2.05f * skillButtonWidth * i, -25f), null, definitions[i]);

		for (int i = 3; i < definitions.Count; ++i)
			AddSkills(new Vector2(skillButtonWidth / 2, 0f) + new Vector2(2.05f * skillButtonWidth * (i - 3), -225f), null, definitions[i]);
	}

	void AddSkills(Vector2 position, Skill parent, SkillDefinition definition) {
		var obj = Instantiate(skillButtonPrefab);
		obj.transform.SetParent(canvas.transform, false);
		obj.GetComponent<RectTransform>().localPosition = position;

		obj.GetComponentInChildren<Text>().text = definition.name;

		var skill = new Skill() {
			definition = definition,
			parent = parent,
			level = 0,
			button = obj.GetComponent<Button>()
		};
		skills.Add(definition.type, skill);

		obj.GetComponent<Button>().onClick.AddListener(() => { OnSkillPressed(skill); });
		
		Vector2 offset = new Vector2(0f, -50f);
		offset.x -= skillButtonWidth / 2 * (definition.dependents.Length - 1);
		for (int i = 0; i < definition.dependents.Length; ++i) {
			AddSkills(position + offset, skill, definition.dependents[i]);
			offset.x += skillButtonWidth;
		}
	}

	void OnSkillPressed(Skill skill) {
		if (skill.parent != null && skill.parent.level == 0)
			return;
		if (Main.S.carbon < skill.definition.carbonCost || Main.S.lithium < skill.definition.lithiumCost)
			return;

		Main.S.carbon -= skill.definition.carbonCost;
		Main.S.lithium -= skill.definition.lithiumCost;
		skill.level++;

		skill.button.GetComponentInChildren<Text>().text = string.Format("{0} {1}", skill.definition.name, skill.level);
		if (skill.level == skill.definition.maxLevel) {
			skill.button.enabled = false;
		}
	}
}
