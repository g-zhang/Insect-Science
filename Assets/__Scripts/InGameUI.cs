using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour {
	public static InGameUI S;

	Text healthAmount;
	Text flyAmount;
	Text carbonAmount;
	Text lithiumAmount;

	public void Awake() {
		S = this;

		healthAmount = transform.Find("HealthAmount").GetComponent<Text>();
		flyAmount = transform.Find("FlyAmount").GetComponent<Text>();
		carbonAmount = transform.Find("CarbonAmount").GetComponent<Text>();
		lithiumAmount = transform.Find("LithiumAmount").GetComponent<Text>();
	}

	public void UpdateHealth() {
		healthAmount.text = Main.S.health.ToString();
	}

	public void UpdateFlies() {
		flyAmount.text = Main.S.flies.ToString();
	}

	public void UpdateCarbon() {
		carbonAmount.text = Main.S.carbon.ToString();
	}

	public void UpdateLithium() {
		lithiumAmount.text = Main.S.lithium.ToString();
	}

	public void UpdateAllStats() {
		UpdateHealth();
		UpdateFlies();
		UpdateCarbon();
		UpdateLithium();
	}
}
