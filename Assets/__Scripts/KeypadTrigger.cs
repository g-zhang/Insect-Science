using UnityEngine;
using System.Collections;

public class KeypadTrigger : MonoBehaviour {

    public float duration = 10f;
    public bool active = true;



    void OnTriggerStay(Collider other) {
        if(other.gameObject.tag == "SmallSwarm")
        {
            if (Main.S.interact && active) {
                DeactivateDoor();
                Destroy(other.gameObject);
                Main.S.HideInteractPopup(this.gameObject);
                Invoke("ActivateDoor", duration);
            }       
        }
    }
 
    void DeactivateDoor() {
        transform.parent.gameObject.SetActive(false);
        active = false;
    }
    void ActivateDoor() {
        active = true;
        transform.parent.gameObject.SetActive(true);
    }
 
}
