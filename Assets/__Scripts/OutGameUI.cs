using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OutGameUI : MonoBehaviour {
	public static OutGameUI S;

	Text carbonAmount;
	Text lithiumAmount;

	public void Awake() {
		S = this;

		carbonAmount = transform.Find("CarbonAmount").GetComponent<Text>();
		lithiumAmount = transform.Find("LithiumAmount").GetComponent<Text>();
	}

	public void Start() {
		UpdateAllStats();
	}

	public void UpdateCarbon() {
		carbonAmount.text = Persistent.S.carbon.ToString();
	}

	public void UpdateLithium() {
		lithiumAmount.text = Persistent.S.lithium.ToString();
	}

	public void UpdateAllStats() {
		UpdateCarbon();
		UpdateLithium();
	}

	public void StartLevel() {
		SceneManager.LoadScene("Scene_1");
	}
}
