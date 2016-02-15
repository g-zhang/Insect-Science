using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour {
	public void StartTutorial1() {
		SceneManager.LoadScene("Scene_Tutorial_VentsKeypad");
	}

	public void StartTutorial2() {
		SceneManager.LoadScene("Gordon_Tutorial_Level");
	}

	public void StartLevel1() {
		SceneManager.LoadScene("Robbie_Tutorial_Level");
	}

	public void StartLevel2() {
		SceneManager.LoadScene("Gordon_Tutorial_Level 1");
	}
	
	public void StartLevel3() {
		SceneManager.LoadScene("Scene_Nick");
	}

	public void StartLevel4() {
		SceneManager.LoadScene("Scene_Rob");
	}

	public void StartLevel5() {
		SceneManager.LoadScene("Scene_Nick_2");
	}
}
