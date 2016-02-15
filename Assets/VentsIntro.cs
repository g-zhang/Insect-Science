using UnityEngine;
using System.Collections;

public class VentsIntro : MonoBehaviour {

	    public GameObject textVents;
        
        void OnTriggerEnter(Collider other) {
            if (other.gameObject.tag != "Player") return;
         
            textVents.SetActive(true);
        }
}
