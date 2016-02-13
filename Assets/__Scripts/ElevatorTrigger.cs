using UnityEngine;
using System.Collections;

public class ElevatorTrigger : MonoBehaviour {

    public ElevatorTrigger destination;
    public bool active = true;
	// Use this for initialization
	void Start () {
	
	}
	
	
    void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player")
        {
            ShowUI();
        }

    }
    void OnTriggerStay(Collider other) {
        if(other.gameObject.tag == "Player")
        {
            float interact = Input.GetAxis("Interact");
            if (interact > 0 && active && Main.S.controlScientist) {
                UseElevator(); 
            }       
        }
    }
    void OnTriggerExit(Collider other) {
        if(other.gameObject.tag == "Player")
        {
            HideUI();
        }
    }
    // Shows the "Press e to use elevator" UI
    public void ShowUI() {
        this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }
    public void HideUI() {
        this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
    public void UseElevator() {
           Vector3 sciPos = Scientist.S.transform.position;
           Vector3 swarmPos = Swarm.S.transform.position;
           sciPos.y = destination.transform.position.y + 0.1f;
           swarmPos.y = destination.transform.position.y + 1.1f;
           Scientist.S.transform.position = sciPos;
           Swarm.S.transform.position = swarmPos;
           destination.active = false;
           Invoke("ActivateDestinationElevator", 1);
    }
    
    public void ActivateDestinationElevator() {
        destination.active = true;
    }
    
}
