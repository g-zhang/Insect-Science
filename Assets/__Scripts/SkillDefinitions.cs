using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
	public SkillId id;
	public int startLevel = 0;
	public int maxLevel = 1;
	public int carbonCost = 0;
	public int lithiumCost = 0;
	public string description;
	public SkillDefinition[] dependents = new SkillDefinition[0];

	public static SkillDefinition[] GetAllDefinitions() {
		return new[] {
			new SkillDefinition() {
				name = "Movement Speed",
				id = SkillId.MovementSpeed,
				maxLevel = 3,
				carbonCost = 10,
				lithiumCost = 1,
				description = "Increases insect movement speed",
				dependents = new[] {
					new SkillDefinition() {
						name = "Move Distance",
						id = SkillId.MoveDistance,
						maxLevel = 3,
						carbonCost = 15,
						lithiumCost = 1,
						description = "Increases maximum distance from scientist",
						dependents = new[] {
							new SkillDefinition() {
								name = "Silent Flying",
								id = SkillId.SilentFlying,
								maxLevel = 3,
								carbonCost = 20,
								lithiumCost = 10,
								description = "Insects are less noticeable"
							}
						}
					},
					new SkillDefinition() {
						name = "Movement Attack",
						id = SkillId.MovementAttack,
						maxLevel = 3,
						carbonCost = 15,
						lithiumCost = 3,
						description = "Insects attack as they move",
					}
				}
			},
			new SkillDefinition() {
				name = "Swarm Size",
				id = SkillId.SwarmSize,
				maxLevel = 3,
				carbonCost = 10,
				lithiumCost = 1,
				description = "Increases number of insects in the swarm",
				dependents = new SkillDefinition[] {
					new SkillDefinition() {
						name = "Swarm Regen.",
						id = SkillId.SwarmRegen,
						maxLevel = 3,
						carbonCost = 15,
						lithiumCost = 1,
						description = "Increases the speed at which lost insects regenerate"
					},
					new SkillDefinition() {
						name = "Light Covering",
						id = SkillId.LightCovering,
						carbonCost = 20,
						lithiumCost = 10,
						description = "Allows insects to cover light fixtures to darken rooms"
					}
				}
			},
			new SkillDefinition() {
				name = "Biting",
				id = SkillId.Biting,
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
						id = SkillId.Venom,
						description = "Imbues bits with venom, slowing down enemies"
					},
					new SkillDefinition() {
						name = "Wood Eating",
						id = SkillId.WoodEating,
						carbonCost = 20,
						lithiumCost = 10,
						description = "Allows insects to eat through wooden objects"
					}
				}
			},
			new SkillDefinition() {
				name = "House Fly",
				id = SkillId.HouseFly,
				carbonCost = 10,
				lithiumCost = 5,
				description = "Bigger, stronger insects",
				dependents = new SkillDefinition[] {
					new SkillDefinition() {
						name = "Black Fly",
						id = SkillId.BlackFly,
						carbonCost = 20,
						lithiumCost = 10,
						description = "Bigger, stronger insects",
						dependents = new SkillDefinition[] {
							new SkillDefinition() {
								name = "Deer Fly",
								id = SkillId.DeerFly,
								carbonCost = 30,
								lithiumCost = 15,
								description = "Bigger, stronger insects",
								dependents = new SkillDefinition[] {
									new SkillDefinition() {
										name = "Horse Fly",
										id = SkillId.HorseFly,
										carbonCost = 40,
										lithiumCost = 20,
										description = "Bigger, stronger insects"
									}
								}
							}
						}
					}
				}
			},
			new SkillDefinition() {
				name = "Scientist Health",
				id = SkillId.ScientistHealth,
				maxLevel = 3,
				carbonCost = 20,
				lithiumCost = 1,
				description = "Health of the scientist",
				dependents = new SkillDefinition[] {
					new SkillDefinition() {
						name = "Hacking",
						id = SkillId.Hacking,
						maxLevel = 3,
						carbonCost = 20,
						lithiumCost = 5,
						description = "Hack into higher level terminals"
					},
					new SkillDefinition() {
						name = "Fly Targetter",
						id = SkillId.Targetter,
						carbonCost = 5,
						lithiumCost = 5,
						description = "A gun that lets you target enemies for flies to attack"
					},
					new SkillDefinition() {
						name = "Fly Shield",
						id = SkillId.Shield,
						carbonCost = 20,
						lithiumCost = 10,
						description = "Your flies take hits for you"
					}
				}
			}
		};
	}
}