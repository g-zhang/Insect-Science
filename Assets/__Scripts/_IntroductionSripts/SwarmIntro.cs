using UnityEngine;
using System.Collections;

public class SwarmIntro : MonoBehaviour {

    public GameObject textSplit;
    public GameObject textSwarmMove;
    public GameObject keypad;

    private bool completed = false;
    private bool splitComplete = false;

	void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag != "Player" || splitComplete) return;
        textSplit.SetActive(true);
        splitComplete = true;
    }
    
    void OnTriggerStay(Collider other) {
       if(other.gameObject.tag != "Player") return;
       float iQ = Input.GetAxis("Split");
        
        if (iQ > 0) {
           textSplit.SetActive(false);
           textSwarmMove.SetActive(true);
        }
        if (!keypad.active)
        {
            completed = true;
        }
    }
    
	void OnTriggerExit(Collider other) {
        if (completed) {
            Destroy(gameObject);
        }
    }
}
