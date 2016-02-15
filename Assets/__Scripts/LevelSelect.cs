using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour {
	public void StartTutorial1() {
		SceneManager.LoadScene("Scene_Prototype");
	}

	public void StartTutorial2() {
		SceneManager.LoadScene("Scene_Tutorial_VentsKeypad");
	}

	public void StartLevel1() {
		SceneManager.LoadScene("Scene_Nick");
	}
	
	public void StartLevel2() {
		SceneManager.LoadScene("Robbie_Tutorial_Level");
	}
}
