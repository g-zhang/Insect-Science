using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {
    static public Main S;                   // Main Singleton

    public bool controlScientist = true;    // The player starts the game controlling the scientist

	// Properties for setting game information

	int _health = 0;
	public int health {
		get { return _health; }
		set {
			_health = value;
			InGameUI.S.UpdateHealth();
		}
	}

	int _flies = 0;
	public int flies {
		get { return _flies; }
		set {
			_flies = value;
			InGameUI.S.UpdateFlies();
		}
	}

	int _carbon = 0;
	public int carbon {
		get { return _carbon; }
		set {
			_carbon = value;
			InGameUI.S.UpdateCarbon();
		}
	}

	int _lithium = 0;
	public int lithium {
		get { return _lithium; }
		set {
			_lithium = value;
			InGameUI.S.UpdateLithium();
		}
	}

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
