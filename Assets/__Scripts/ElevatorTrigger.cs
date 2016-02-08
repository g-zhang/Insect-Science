using UnityEngine;
using System.Collections;

public class ElevatorTrigger : MonoBehaviour {

    public ElevatorTrigger destination;
    public bool active = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter(Collider other) {
        ShowUI();

    }
    void OnTriggerStay(Collider other) {
        float interact = Input.GetAxis("Interact");
        if (interact > 0 && active) {
            UseElevator(); 
        }
    }
    // Shows the "Press e to use elevator" UI
    public void ShowUI() {
        
    }
    
    public void UseElevator() {
           Vector3 pos = Scientist.S.transform.position;
           pos.y = destination.transform.position.y + 0.1f;
           Scientist.S.transform.position = pos;
           destination.active = false;
           Invoke("ActivateDestinationElevator", 1);
    }
    
    public void ActivateDestinationElevator() {
        destination.active = true;
    }
    
}
