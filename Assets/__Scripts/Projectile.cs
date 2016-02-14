using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    public float TimeToLive = 0f; //if set to 0, the projectile will not expire based on time

    float currTTL = 0f;

	// Use this for initialization
	void Start () {
        currTTL = TimeToLive;
	}
	
	// Update is called once per frame
	void Update () {
        if(TimeToLive > 0)
        {
            if(currTTL > 0)
            {
                currTTL -= Time.deltaTime;
            } else
            {
                Destroy(this.gameObject);
            }
        }
	}

    void OnTriggerEnter(Collider coll)
    {
		if (coll.tag == "Player") {
			Main.S.FadeOutAndRestart();
		}
        Destroy(this.gameObject);
    }
}
