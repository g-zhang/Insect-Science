using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour {
	public void StartTutorial1() {
		SceneManager.LoadScene("Gordon_Tutorial_Level");
	}

	public void StartTutorial2() {
		SceneManager.LoadScene("Robbie_Tutorial_Level");
	}

	public void StartLevel1() {
		SceneManager.LoadScene("Scene_Nick");
	}
	
	public void StartLevel2() {

	}
}
