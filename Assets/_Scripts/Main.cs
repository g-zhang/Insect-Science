using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {
    static public Main S;                   // Main Singleton

    public bool controlScientist = true;    // The player starts the game controlling the scientist

	void Awake () {
        // Set the singleton
        S = this;
	}
	
	// Update is called once per frame
	void Update () {
	    // Player wants to switch control between scientist and swarm
        if (Input.GetKeyDown(KeyCode.Q)) {
            // Switch control between the scientist and the swarm
            controlScientist = !controlScientist;
        }
	}
}
